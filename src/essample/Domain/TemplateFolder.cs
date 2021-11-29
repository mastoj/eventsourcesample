using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;

namespace essample.Domain
{
    public abstract record TemplateFolderCommand {}
    public abstract record TemplateFolderEvent {
        public static TemplateFolderEvent EventParser(string eventType, string jsonData)
        {
            switch(eventType) {
                case "TemplateFolderCreated":
                    return JsonSerializer.Deserialize<TemplateFolderCreated>(jsonData);
                default: 
                    throw new ArgumentException("Invalid event type");
            }
        }
    }

    public class FolderExistsException : Exception {}
    public class FolderMissingException : Exception {}
    public record CreateTemplateFolder : TemplateFolderCommand {
        public string Name { get; }
        public CreateTemplateFolder(string name)
        {
            if(String.IsNullOrEmpty(name)) {
                throw new ArgumentException("Name can't be null or empty");
            }
            Name = name;
        }

    }
    public record UpdateTemplateFolder : TemplateFolderCommand {
        public string Name { get; }
        public UpdateTemplateFolder(string name)
        {
            if(String.IsNullOrEmpty(name)) {
                throw new ArgumentException("Name can't be null or empty");
            }
            Name = name;
        }

    }
    public record TemplateFolderCreated(string Name) : TemplateFolderEvent;
    public record TemplateFolderUpdated(string Name) : TemplateFolderEvent;

    public static class TemplateFolderDecider {
        public static ReadOnlyCollection<TemplateFolderEvent> Handle(CreateTemplateFolder command, TemplateFolder state)
        {
            if(state.Name != "") {
                throw new FolderExistsException();
            }
            return new List<TemplateFolderEvent> { 
                new TemplateFolderCreated(command.Name)
            }.AsReadOnly();
        }

        public static ReadOnlyCollection<TemplateFolderEvent> Handle(UpdateTemplateFolder command, TemplateFolder state)
        {
            if(String.IsNullOrEmpty(state.Name)) {
                throw new FolderMissingException();
            }
            return new List<TemplateFolderEvent> { 
                new TemplateFolderUpdated(command.Name)
            }.AsReadOnly();
        }

        public static Func<TemplateFolderCommand, TemplateFolder, ReadOnlyCollection<TemplateFolderEvent>> Create() {
            return (command, state) => {
                switch(command)
                {
                    case CreateTemplateFolder createTemplateFolder:
                        return Handle(createTemplateFolder, state);
                    case UpdateTemplateFolder updateTemplateFolder:
                        return Handle(updateTemplateFolder, state);
                    default:
                        throw new NotImplementedException($"Invalid command {command.GetType().FullName}");
                };
            };
        }
    }

    public static class TemplateFolderBuilder {
        public static TemplateFolder Apply(TemplateFolderCreated @event, TemplateFolder state)
        {
            return state with { Name = @event.Name};
        }

        public static TemplateFolder Apply(TemplateFolderUpdated @event, TemplateFolder state)
        {
            return state with { Name = @event.Name};
        }

        public static TemplateFolder Build(TemplateFolder state, TemplateFolderEvent @event)
        {
            switch(@event) {
                case TemplateFolderCreated templateFolderCreated:
                    return Apply(templateFolderCreated, state);
                case TemplateFolderUpdated templateFolderUpdated:
                    return Apply(templateFolderUpdated, state);
                default:
                    throw new NotImplementedException($"Invalid event {@event.GetType().FullName}");
            }            
        }

        public static Func<TemplateFolder, ReadOnlyCollection<TemplateFolderEvent>, TemplateFolder> Create() {
            return (state, events) => {
                return events.Aggregate(state, Build);
            };
        }
    }
    public record TemplateFolder(string Name) {
        public static TemplateFolder Initial = new TemplateFolder("");
    }
}