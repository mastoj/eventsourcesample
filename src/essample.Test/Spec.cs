using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace essample.Infra.Test
{
    public delegate ReadOnlyCollection<TEvent> Decide<TCommand, TState, TEvent>(TCommand command, TState state);
    public delegate TState Build<TState, TEvent>(TState state, ReadOnlyCollection<TEvent> events);

    public record SpecState<TState, TEvent>(TState State, ReadOnlyCollection<TEvent> Outcome)
    {
    }

    public static class SpecState
    {
        public static SpecState<TState, TEvent> Create<TState, TEvent>(TState state, ReadOnlyCollection<TEvent> events)
        {
            return new SpecState<TState, TEvent>(state, events);
        }
    }

    public abstract class Spec<TCommand, TState, TEvent>
    {
        public SpecState<TState, TEvent> SpecState { get; private set; }
        public Decide<TCommand, TState, TEvent> Decide { get; private set; }
        public Build<TState, TEvent> Build { get; set; }

        public Spec(TState initialState, Decide<TCommand, TState, TEvent> decide, Build<TState, TEvent> build)
        {
            SpecState = Test.SpecState.Create(initialState, new List<TEvent> { }.AsReadOnly());
            Decide = decide;
            Build = build;
        }

        public void Given(List<TEvent> events)
        {
            var newState = Build(SpecState.State, events.AsReadOnly());
            SpecState = SpecState with { State = newState };
        }

        public void When(TCommand command)
        {
            var result = Decide(command, SpecState.State);
            var newState = Build(SpecState.State, result);
            SpecState = SpecState with { Outcome = result, State = newState };
        }

        public void Then(List<TEvent> expectedEvents)
        {
            Assert.Equal(expectedEvents, SpecState.Outcome);
        }

        public void ThenState(TState expectedState)
        {
            Assert.Equal(expectedState, SpecState.State);
        }
    }
}
