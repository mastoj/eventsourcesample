using System;
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
            var application = new Application(eventStore);
            var streamId = Guid.NewGuid().ToString("N");
            var result = application.Handle(streamId, new CreateTemplateFolder("Hello")).Result;
            result.ToList().ForEach(e => Console.WriteLine($"Event: {e}"));

            var result2 = application.Handle(streamId, new CreateTemplateFolder("Hello")).Result;
        }
    }
}
