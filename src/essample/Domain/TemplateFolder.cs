using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace essample.Domain
{
    public abstract record TemplateFolderCommand {}
    public abstract record TemplateFolderEvent {}

    public class FolderExistsException : Exception {}
    public record CreateTemplateFolder(string Name) : TemplateFolderCommand {}
    public record TemplateFolderCreated(string Name) : TemplateFolderEvent {}
    public record TemplateFolder(string Name) {
        public static TemplateFolder Initial = new TemplateFolder("");
        public static ReadOnlyCollection<TemplateFolderEvent> Decide(TemplateFolderCommand command, TemplateFolder state)
        {
            switch(command)
            {
                case CreateTemplateFolder createTemplateFolder:
                    if(state.Name != "") {
                        throw new FolderExistsException();
                    }
                    return new List<TemplateFolderEvent> { 
                        new TemplateFolderCreated(createTemplateFolder.Name)
                    }.AsReadOnly();
                default:
                    throw new NotImplementedException($"Invalid command {command.GetType().FullName}");
            }
        }

        public static TemplateFolder Build(TemplateFolder state, TemplateFolderEvent @event)
        {
            switch(@event) {
                case TemplateFolderCreated templateFolderCreated:
                    return state with { Name = templateFolderCreated.Name};
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