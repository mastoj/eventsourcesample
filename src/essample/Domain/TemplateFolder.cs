using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace essample.Infra.Domain
{
    public abstract record TemplateFolderCommand {}
    public abstract record TemplateFolderEvent {}

    public class FolderExistsException : Exception {}
    public record CreateTemplateFolder(string Name) : TemplateFolderCommand {}
    public record TemplateFolderCreated(string Name) : TemplateFolderEvent {}
    public record TemplateFolder(string Name) {
        public static TemplateFolder Initial = new TemplateFolder("");

        public static ReadOnlyCollection<TemplateFolderEvent> Handle(CreateTemplateFolder command, TemplateFolder state)
        {
            if(state.Name != "") {
                throw new FolderExistsException();
            }
            return new List<TemplateFolderEvent> { 
                new TemplateFolderCreated(command.Name)
            }.AsReadOnly();
        }

        public static ReadOnlyCollection<TemplateFolderEvent> Decide(TemplateFolderCommand command, TemplateFolder state)
        {
            switch(command)
            {
                case CreateTemplateFolder createTemplateFolder:
                    return Handle(createTemplateFolder, state);
                default:
                    throw new NotImplementedException($"Invalid command {command.GetType().FullName}");
            }
        }

        public static TemplateFolder Apply(TemplateFolderCreated @event, TemplateFolder state)
        {
            return state with { Name = @event.Name};
        }

        public static TemplateFolder Build(TemplateFolder state, TemplateFolderEvent @event)
        {
            switch(@event) {
                case TemplateFolderCreated templateFolderCreated:
                    return Apply(templateFolderCreated, state);
                default:
                    throw new NotImplementedException($"Invalid event {@event.GetType().FullName}");
            }            
        }

        public static TemplateFolder Build(TemplateFolder state, ReadOnlyCollection<TemplateFolderEvent> events)
        {
            return events.Aggregate(state, Build);
        }
    }
}