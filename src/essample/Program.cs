using System;
using System.Collections.Generic;
using System.Linq;
using essample.Domain;

namespace essample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            // Get events
            var events = new List<CreateTemplateEvent>().AsReadOnly();
            var expectedVersion = events.Count;
            var currentState = TemplateFolder.Build(TemplateFolder.Initial, events);
            var outcome = TemplateFolder.Decide(new CreateTemplateFolder("Hello"), currentState);
            var newState = TemplateFolder.Build(currentState, outcome);
            // Save events
            Console.WriteLine($"==> Out: {String.Join(", ", outcome.Select(y => y.ToString()))}");
            var outcome2 = TemplateFolder.Decide(new CreateTemplateFolder("Hello"), newState);
        }
    }
}
