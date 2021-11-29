using System;
using System.Collections.Generic;
using essample.Domain;
using FluentAssertions;
using Xunit;

namespace essample.Infra.Test
{
    public class TemplateFolderTests : Spec<TemplateFolderCommand, TemplateFolder, TemplateFolderEvent>
    {
        public TemplateFolderTests() : base(new Handler<TemplateFolderCommand, TemplateFolder, TemplateFolderEvent>(TemplateFolder.Initial, TemplateFolderBuilder.Create(), TemplateFolderDecider.Create(), new InMemoryStore(), TemplateFolderEvent.EventParser))
        {
        }

        [Fact]
        public void CreateTemplateFolder_Returns_FolderCreated()
        {
            Given(new List<TemplateFolderEvent> {});
            When(new CreateTemplateFolder("MyFolder"));
            Then(new List<TemplateFolderEvent> {
                new TemplateFolderCreated("MyFolder")
            });
            ThenState(new TemplateFolder("MyFolder"));
        }

        [Fact]
        public void CreateTemplateFolder_Fails_If_Folder_Exists()
        {
            Action action = () => {
                Given(new List<TemplateFolderEvent> {
                    new TemplateFolderCreated("MyFolder")
                });
                When(new CreateTemplateFolder("MyFolder"));
            };
            action.Should().Throw<FolderExistsException>();
        }

        [Fact]
        public void UpdateTemplateFolder_Fails_If_Folder_Is_Missing()
        {
            Action action = () => {
                Given(new List<TemplateFolderEvent> { });
                When(new UpdateTemplateFolder("MyFolder"));
            };
            action.Should().Throw<FolderMissingException>();
        }

        [Fact]
        public void UpdateTemplateFolder_Updates_Name_If_Exists()
        {
            Given(new List<TemplateFolderEvent> { new TemplateFolderCreated("MyFolder") });
            When(new UpdateTemplateFolder("MyFolder2"));
            Then(new List<TemplateFolderEvent> {
                new TemplateFolderUpdated("MyFolder2")
            });
        }
    }
}
