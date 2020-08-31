using ManageDocuments.Api.ModelValidation;
using Microsoft.AspNetCore.Http;

namespace ManageDocuments.Api.Requests
{
    public class UploadDocumentRequest
    {
        [ValidateDocument]
        public IFormFile FileToUpload { get; set; }
    }
}
