using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using essample.Infra.Domain;

namespace essample.Infra
{
    public class Application
    {
        private Handler<TemplateFolderCommand, TemplateFolder, TemplateFolderEvent> TemplateFolderHandler { get; }

        public Application(IEventStore eventStore)
        {
            TemplateFolderHandler = new Handler<TemplateFolderCommand, TemplateFolder, TemplateFolderEvent>(TemplateFolder.Initial, TemplateFolder.Build, TemplateFolder.Decide, eventStore, TemplateFolderEvent.EventParser);
        }

        public async Task<ReadOnlyCollection<object>> Handle(string streamId, object command)
        {
            switch(command)
            {
                case TemplateFolderCommand templateFolderCommand:
                    return await TemplateFolderHandler.Handle(streamId, templateFolderCommand);
                default:
                    throw new ArgumentException($"Invalid command: {command.GetType().FullName}");
            }
        }
    }
}
