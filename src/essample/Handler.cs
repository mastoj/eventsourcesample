using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace essample.Infra
{
    public class Handler<TCommand, TState, TEvent> where TEvent : class
    {
        public TState InitialState { get; }
        public Func<TState, ReadOnlyCollection<TEvent>, TState> Build { get; }
        public Func<TCommand, TState, ReadOnlyCollection<TEvent>> Decide { get; }
        public IEventStore EventStore { get; }
        public Func<string, string, TEvent> EventParser { get; }

        public Handler(
            TState initialState,
            Func<TState, ReadOnlyCollection<TEvent>, TState> build,
            Func<TCommand, TState, ReadOnlyCollection<TEvent>> decide,
            IEventStore eventStore,
            Func<string, string, TEvent> eventParser        
        )
        {
            InitialState = initialState;
            Build = build;
            Decide = decide;
            EventStore = eventStore;
            EventParser = eventParser;
        }

        public async Task<ReadOnlyCollection<Object>> Handle(string streamId, TCommand command)
        {
            var readResult = EventStore.ReadEvents<TEvent>(streamId, EventParser).Result;
            var expectedVersion = readResult.CurrentRevision;
            var events = readResult.Events;
            var currentState = Build(InitialState, events);
            var outcome = Decide(command, currentState);
            await EventStore.AppendEvents(streamId, expectedVersion, outcome);
            return outcome.Cast<Object>().ToList().AsReadOnly();
        }
    }
}
