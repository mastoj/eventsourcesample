using System.Threading.Tasks;
using essample.Infra;
using essample.Domain;
using Microsoft.AspNetCore.Mvc;
using essample.api.TemplateFolder.Types;

namespace essample.api.TemplateFolder
{
    [ApiController]
    [Route("api/templatefolder")]
    public class TemplateFolderController
    {
        public Application Application { get; }

        public TemplateFolderController(Application application)
        {
            Application = application;
        }

        [HttpGet]
        public string GetTemplateFolder() {
            return "Hello";
        }

        /*
POST http://localhost:5000/api/templatefolder
Content-Type: application/json

{
    "name": "tomas-folder"
}
        */
        [HttpPost]
        public async Task<object> CreateTemplateFolder(CreateTemplateFolderRequest request)
        {
            
            return await Application.Handle(Application.CreateStreamId(), request.ToCommand());
        }
    }
}