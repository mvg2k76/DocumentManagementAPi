using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ManageDocuments.Api.Requests;
using ManageDocuments.Api.ViewModel;

namespace ManageDocuments.Api.Infrastructure
{
    public interface IDocumentRepository
    {
        string DocumentStore { get; set; }

        Task<string> UploadDocument(UploadDocumentRequest request);

        Task<MemoryStream> GetDocument(string fileName);

        IList<Document> GetDocumentList();
        
        bool DeleteDocument(string fileName);
    }
}
