using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using essample.Infra.Domain;

namespace essample.Infra
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Func<string, string, TemplateFolderEvent> parseEvent = (eventType, jsonData) => {
                switch(eventType) {
                    case "TemplateFolderCreated":
                        return JsonSerializer.Deserialize<TemplateFolderCreated>(jsonData);
                    default: 
                        throw new ArgumentException("Invalid event type");
                }
            };

            IEventStore eventStore = new InMemoryStore();

            // Get events
            var readResult = eventStore.ReadEvents<TemplateFolderEvent>("my-folder", parseEvent).Result;
            var expectedVersion = readResult.CurrentRevision;
            var events = readResult.Events;
            var currentState = TemplateFolder.Build(TemplateFolder.Initial, events);
            var outcome = TemplateFolder.Decide(new CreateTemplateFolder("Hello"), currentState);
            eventStore.AppendEvents("my-folder", expectedVersion, outcome).Wait();

            // var newState = TemplateFolder.Build(currentState, outcome);
            // Save events
            Console.WriteLine($"==> Out: {String.Join(", ", outcome.Select(y => y.ToString()))}");
            // var outcome2 = TemplateFolder.Decide(new CreateTemplateFolder("Hello"), newState);
        }
    }
}
