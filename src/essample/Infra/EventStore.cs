using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EventStore.Client;

namespace essample.Infra
{
    public record ReadResult<TEvent>(UInt64? CurrentRevision, ReadOnlyCollection<TEvent> Events);

    public class EventStore : IEventStore
    {
        private static string ConnectionString = "esdb://127.0.0.1:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000";
        private EventStoreClient client;

        public EventStore()
        {
            var eventStoreSettings = EventStoreClientSettings.Create(ConnectionString);
            client = new EventStoreClient(eventStoreSettings);
        }

        public async Task<ReadResult<TEvent>> ReadEvents<TEvent>(string streamId, Func<string, string, TEvent> parseEvent) where TEvent: class
        {
            try
            {
                var stream = client.ReadStreamAsync(Direction.Forwards, streamId, StreamPosition.Start, resolveLinkTos: true);
                var readEvents = await stream
                        .Select(e => (e.Event.EventType, e.Event.EventNumber.ToUInt64(), System.Text.UTF8Encoding.UTF8.GetString(e.Event.Data.ToArray())))
                        .Select(data => (data.Item2, parseEvent(data.Item1, data.Item3) as TEvent))
                        .ToListAsync();
                return new ReadResult<TEvent>(readEvents.Last().Item1, readEvents.Select(y => y.Item2).ToList().AsReadOnly());
            }
            catch(StreamNotFoundException ex) {
                return new ReadResult<TEvent>(null, new List<TEvent>().AsReadOnly());
            }
        }

        public async Task AppendEvents<TEvent>(string streamId, UInt64? expectedVersion, ReadOnlyCollection<TEvent> events) 
        {
            var data = events.Select(e => {
                return new EventData(
                    Uuid.NewUuid(),
                    e.GetType().Name,
                    Encoding.UTF8.GetBytes(JsonSerializer.Serialize(e))
                );
            });
            if(expectedVersion.HasValue) {
                await client.AppendToStreamAsync(streamId, expectedVersion.Value, data);
            }
            else {
                await client.AppendToStreamAsync(streamId, StreamState.NoStream, data);
            }
        }
    }
}
