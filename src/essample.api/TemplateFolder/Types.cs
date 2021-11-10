using essample.Domain;

namespace essample.api.TemplateFolder.Types
{
    public record CreateTemplateFolderRequest(string name)
    {
        public CreateTemplateFolder ToCommand() {
            return new CreateTemplateFolder(name);
        }
    }

    public record UpdateTemplateFolderRequest(string name)
    {
        public UpdateTemplateFolder ToCommand() {
            return new UpdateTemplateFolder(name);
        }
    }
}