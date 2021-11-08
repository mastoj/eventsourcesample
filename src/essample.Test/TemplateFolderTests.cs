using System;
using System.Collections.Generic;
using essample.Domain;
using FluentAssertions;
using Xunit;

namespace essample.Test
{
    public class TemplateFolderTests : Spec<CreateTemplateCommand, TemplateFolder, CreateTemplateEvent>
    {
        public TemplateFolderTests() : base(TemplateFolder.Initial, TemplateFolder.Decide, TemplateFolder.Build)
        {
        }

        [Fact]
        public void CreateTemplateFolder_Returns_FolderCreated()
        {
            Given(new List<CreateTemplateEvent> {});
            When(new CreateTemplateFolder("MyFolder"));
            Then(new List<CreateTemplateEvent> {
                new TemplateFolderCreated("MyFolder")
            });
            ThenState(new TemplateFolder("MyFolder"));
        }

        [Fact]
        public void CreateTemplateFolder_Fails_If_Folder_Exists()
        {
            Action action = () => {
                Given(new List<CreateTemplateEvent> {
                    new TemplateFolderCreated("MyFolder")
                });
                When(new CreateTemplateFolder("MyFolder"));
            };
            action.Should().Throw<FolderExistsException>();
        }
    }
}
