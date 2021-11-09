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

    public abstract class Spec<TCommand, TState, TEvent> where TEvent : class
    {
        public SpecState<TState, TEvent> SpecState { get; private set; }
        public Handler<TCommand, TState, TEvent> Handler { get; }

        public Spec(Handler<TCommand, TState, TEvent> handler)
        {
            SpecState = Test.SpecState.Create(handler.InitialState, new List<TEvent> { }.AsReadOnly());
            Handler = handler;
        }

        public void Given(List<TEvent> events)
        {
            var newState = Handler.Build(SpecState.State, events.AsReadOnly());
            SpecState = SpecState with { State = newState };
        }

        public void When(TCommand command)
        {
            var result = Handler.Decide(command, SpecState.State);
            var newState = Handler.Build(SpecState.State, result);
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
