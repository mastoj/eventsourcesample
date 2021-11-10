using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace essample.Infra
{
    public class InMemoryStore : IEventStore
    {
        private Dictionary<string, List<object>> Store { get; init; }
        
        public InMemoryStore()
        {
            Console.WriteLine("Creating store");
            Store = new Dictionary<string, List<object>>();
        }

        public Task AppendEvents<TEvent>(string streamId, ulong? expectedVersion, ReadOnlyCollection<TEvent> events)
        {
            if(expectedVersion.HasValue)
            {
                Console.WriteLine("==> Expected verison: " + expectedVersion);
                Console.WriteLine("==> Actual version: " + Store[streamId].Count);
                if(expectedVersion.Value.CompareTo(Convert.ToUInt64(Store[streamId].Count)) != 0)
                {
                    throw new Exception($"Unexpected version, expected {expectedVersion} got {Store[streamId].Count}");
                }
                Store[streamId].AddRange(events.ToList().Cast<Object>());
            }
            else {
                Console.WriteLine("Adding events: " + streamId);
                Store.Add(streamId, new List<Object>(events.ToList().Cast<Object>()));
            }
            return Task.FromResult<object>(null);
        }

        public Task<ReadResult<TEvent>> ReadEvents<TEvent>(string streamId, Func<string, string, TEvent> parseEvent) where TEvent : class
        {
            Console.WriteLine("StreamId: " + streamId);
            Console.WriteLine("Aggregate count: " + Store.Keys.Count);
            Console.WriteLine("First id: " + Store.Keys.FirstOrDefault());
            if(Store.ContainsKey(streamId)) {
                Console.WriteLine("Stream is missing: " + streamId);
                return Task.FromResult(new ReadResult<TEvent>(Convert.ToUInt64(Store[streamId].Count), Store[streamId].Cast<TEvent>().ToList().AsReadOnly()));
            }
            else {
                return Task.FromResult(new ReadResult<TEvent>(null, new List<TEvent>().AsReadOnly()));
            }
        }
    }
}
