using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageDocuments.Api.Requests;
using ManageDocuments.Api.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace ManageDocuments.Api.Infrastructure
{
    public class FileSystemDocumentRepository : IDocumentRepository
    {
        public string DocumentStore { get; set; }

        public async Task<MemoryStream> GetDocument(string fileName)
        {
            string fullPath = Path.Combine(DocumentStore, fileName);

            if (System.IO.File.Exists(fullPath))
            {
                var memory = new MemoryStream();
                using (var stream = new FileStream(fullPath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0;
                return memory;
            }

            return null;
        }

        public IList<Document> GetDocumentList()
        {
            var documents = new List<Document>();
            var directory = new DirectoryInfo(DocumentStore);

            foreach (var fileInfo in directory.GetFiles())
            {
                documents.Add(new Document()
                {
                    Name = fileInfo.Name,
                    Location = @fileInfo.DirectoryName,
                    SizeInBytes = fileInfo.Length
                });
            }

            return documents;
        }

        public async Task<string> UploadDocument(UploadDocumentRequest request)
        {
            string uploadPath = Path.Combine(DocumentStore, request.FileToUpload.FileName);
            using (var stream = System.IO.File.Create(uploadPath))
            {
                await request.FileToUpload.CopyToAsync(stream);
            }

            return uploadPath;
        }

        public bool DeleteDocument(string fileName)
        {
            string filePath = Path.Combine(DocumentStore, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                return true;
            }

            return false;
        }
    }
}
