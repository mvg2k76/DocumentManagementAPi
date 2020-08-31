using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using ManageDocuments.Api.Infrastructure;
using ManageDocuments.Api.Requests;
using ManageDocuments.Api.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ManageDocuments.Api.Controllers
{
    [Route("api/documents")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDocumentRepository _documentRepository;

        public DocumentsController(IConfiguration configuration, IDocumentRepository documentRepository)
        {
            _configuration = configuration;
            _documentRepository = documentRepository;
            _documentRepository.DocumentStore = _configuration["DocumentStoreLocation"];

            EnsureDocumentStoreExists(_documentRepository.DocumentStore);
         }

        [HttpGet("{fileName}", Name = "GetDocument")]
        public async Task<IActionResult> Get(string fileName)
        {
            var fileStream = await _documentRepository.GetDocument(fileName);

            if (fileStream != null)
            {
                return new FileStreamResult(fileStream, "application/pdf");
            }

            return NotFound($"Request file {fileName} not found");
        }

        [HttpGet]
		public IActionResult GetDocuments()
        {
            return Ok(_documentRepository.GetDocumentList());
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentRequest request) 
        {
            var response = await _documentRepository.UploadDocument(request);

            return CreatedAtRoute("GetDocument", new { fileName = request.FileToUpload.FileName },null);
        }

        [HttpDelete]
        public IActionResult DeleteDocument(string fileName)
        {
            if (_documentRepository.DeleteDocument(fileName))
            {
                return NoContent();
            }

            return NotFound($"File {fileName} not found");
        }

        private void EnsureDocumentStoreExists(string documentRepositoryDocumentStore)
        {
            if (!Directory.Exists(documentRepositoryDocumentStore))
            {
                Directory.CreateDirectory(documentRepositoryDocumentStore);
            }
        }
    }
}