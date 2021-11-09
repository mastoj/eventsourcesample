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
            Func<string, string, TemplateFolderEvent> parseEvent = (eventType, jsonData) => {
                switch(eventType) {
                    case "TemplateFolderCreated":
                        return JsonSerializer.Deserialize<TemplateFolderCreated>(jsonData);
                    default: 
                        throw new ArgumentException("Invalid event type");
                }
            };

            // Bootstrap application
            IEventStore eventStore = new InMemoryStore();
            var application = new Application(eventStore);

            // Execute example command
            var streamId = Guid.NewGuid().ToString("N");
            var result = application.Handle(streamId, new CreateTemplateFolder("Hello")).Result;
            result.ToList().ForEach(e => Console.WriteLine($"Event: {e}"));

            // Execute failing command (same stream id when it is expected to be empyt)
            var result2 = application.Handle(streamId, new CreateTemplateFolder("Hello")).Result;
        }
    }
}
