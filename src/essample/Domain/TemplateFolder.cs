using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace essample.Domain
{
    public abstract record CreateTemplateCommand {}
    public abstract record CreateTemplateEvent {}

    public class FolderExistsException : Exception {}
    public record CreateTemplateFolder(string Name) : CreateTemplateCommand {}
    public record TemplateFolderCreated(string Name) : CreateTemplateEvent {}
    public record TemplateFolder(string Name) {
        public static TemplateFolder Initial = new TemplateFolder("");
        public static ReadOnlyCollection<CreateTemplateEvent> Decide(CreateTemplateCommand command, TemplateFolder state)
        {
            switch(command)
            {
                case CreateTemplateFolder createTemplateFolder:
                    if(state.Name != "") {
                        throw new FolderExistsException();
                    }
                    return new List<CreateTemplateEvent> { 
                        new TemplateFolderCreated(createTemplateFolder.Name)
                    }.AsReadOnly();
                default:
                    throw new NotImplementedException($"Invalid command {command.GetType().FullName}");
            }
        }

        public static TemplateFolder Build(TemplateFolder state, CreateTemplateEvent @event)
        {
            switch(@event) {
                case TemplateFolderCreated templateFolderCreated:
                    return state with { Name = templateFolderCreated.Name};
                default:
                    throw new NotImplementedException($"Invalid event {@event.GetType().FullName}");
            }            
        }

        public static TemplateFolder Build(TemplateFolder state, ReadOnlyCollection<CreateTemplateEvent> events)
        {
            return events.Aggregate(state, Build);
        }
    }
}