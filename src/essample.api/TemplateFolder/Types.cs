using essample.Domain;

namespace essample.api.TemplateFolder.Types
{
    public record CreateTemplateFolderRequest(string name)
    {
        public CreateTemplateFolder ToCommand() {
            return new CreateTemplateFolder(name);
        }
    }
}