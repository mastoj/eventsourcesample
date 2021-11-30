using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace essample.Infra
{
    public interface IEventStore
    {
        Task AppendEvents<TEvent>(string streamId, ulong? expectedVersion, ReadOnlyCollection<TEvent> events, Func<TEvent, string> getName = null);
        Task<ReadResult<TEvent>> ReadEvents<TEvent>(string streamId, Func<string, string, TEvent> parseEvent) where TEvent : class;
    }
}
