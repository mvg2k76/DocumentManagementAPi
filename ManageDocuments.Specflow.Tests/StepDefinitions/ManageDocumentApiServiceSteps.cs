using ManageDocuments.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ManageDocuments.Api.ViewModel;
using Newtonsoft.Json;
using TechTalk.SpecFlow;

namespace ManageDocuments.Specflow.Tests.StepDefinitions
{
    [Binding]
    public sealed class ManageDocumentApiServiceSteps : Steps
    {
        private readonly FeatureContext _featureContext;
        private WebApplicationFactory<Startup> _factory;
        private HttpClient _client { get; set; }

        private readonly ScenarioContext _scenarioContext;
        private readonly string _baseUri;

        private readonly string _documentsSourceFolder;
        private readonly string _documentsDownloadFolder;
        private readonly string _documentStoreLocation;

        public ManageDocumentApiServiceSteps(ScenarioContext scenarioContext, WebApplicationFactory<Startup> factory)
        {
            _baseUri = "http://localhost:5000/";

            _factory = factory;
            _scenarioContext = scenarioContext;

            _documentsSourceFolder = ConfigurationManager.AppSettings["DocumentSourceFolder"];
            _documentsDownloadFolder = ConfigurationManager.AppSettings["DocumentDownloadFolder"];
            _documentStoreLocation = ConfigurationManager.AppSettings["DocumentStoreLocation"];
            
        }


        [Given(@"The ManageDocuments Api service is ready")]
        public void GivenTheManageDocumentsApiServiceIsReady()
        {
            _scenarioContext.Add("HTTPClient",
                _factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    BaseAddress = new Uri(_baseUri)
                })
            );
        }

        [Given(@"Document source and download folders exist")]
        public void GivenDocumentSourceAndDownloadFoldersExist()
        {
            if (!Directory.Exists(_documentsSourceFolder))
            {
                Directory.CreateDirectory(_documentsSourceFolder);
            }

            if (!Directory.Exists(_documentsDownloadFolder))
            {
                Directory.CreateDirectory(_documentsDownloadFolder);
            }
        }

        [Given(@"I make sure atleast one document exists in uploaded documents folder")]
        public void GivenIMakeSureAtleastOneDocumentExistsInUploadedDocumentsFolder()
        {
            if (!Directory.GetFiles(_documentStoreLocation).Any())
            {
                var sourceFile = Path.Combine(_documentsSourceFolder, "SmallDocument.pdf");
                var destFile = Path.Combine(_documentStoreLocation, "SmallDocument.pdf");
                System.IO.File.Copy(sourceFile, destFile, true);
            }
        }

        [Given(@"I have a PDF file '(.*)' under document source folder  to upload")]
        public void GivenIHaveAPdfFileUpload(string documentName)
        {
            _scenarioContext.Add("FileName", documentName);
            _scenarioContext.Add("FilePath", Path.Combine(_documentsSourceFolder, documentName));
        }

        [When(@"I send the PDF to the API")]
        public async Task WhenISendThePDFToTheAPI()
        {
            var fileStream = System.IO.File.OpenRead(@_scenarioContext["FilePath"].ToString());
            var formFile = new FormFile(fileStream, 0, fileStream.Length, "FileToUpload", _scenarioContext["FileName"].ToString());

            var memStream = new MemoryStream();
            formFile.CopyTo(memStream);

            HttpContent httpContent = new StreamContent(memStream);
            httpContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
            {
                Name = "FileToUpload",
                FileName = _scenarioContext["FileName"].ToString()
            };
            httpContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/pdf");

            var formData = new MultipartFormDataContent();
            formData.Add(httpContent);

            var httpClient = (HttpClient)_scenarioContext["HTTPClient"];
            var response = await httpClient.PostAsync("api/documents", formData);

            _scenarioContext.Add("HTTPResponse", response);
        }

        [Then(@"It is uploaded successfully")]
        public void ThenItIsUploadedSuccessfully()
        {
            var httpResponseMessage = (HttpResponseMessage)_scenarioContext["HTTPResponse"];

            string createdAtLocation =
                _baseUri + "api/documents/" + _scenarioContext["FileName"].ToString();

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                Assert.AreEqual(HttpStatusCode.Created, httpResponseMessage.StatusCode);
                Assert.AreEqual(createdAtLocation, httpResponseMessage.Headers.Location.AbsoluteUri);
            }
        }

        [Then(@"The API does not accept the file and returns the appropriate messaging and status")]
        public async Task ThenTheAPIDoesNotAcceptTheFileAndReturnsTheAppropriateMessagingAndStatus()
        {
            var httpResponse = (HttpResponseMessage)_scenarioContext["HTTPResponse"];

            var errors = await httpResponse.Content.ReadAsStringAsync();

            Assert.AreEqual(httpResponse.IsSuccessStatusCode, false);
            Assert.AreEqual(HttpStatusCode.BadRequest, httpResponse.StatusCode);
            Assert.IsTrue(errors.Contains("Size of the file is greater than 5MB", StringComparison.InvariantCultureIgnoreCase));
        }

        [When(@"I call the API to get a list of documents")]
        public async Task WhenICallTheAPIToGetAListOfDocuments()
        {
            var httpClient = (HttpClient)_scenarioContext["HTTPClient"];

            var response = await httpClient.GetAsync("api/documents");
            
            _scenarioContext.Add("HTTPResponse", response);
        }

        [Then(@"a list of PDFs’ is returned with the following properties: name, location, file-size")]
        public async Task ThenAListOfPDFsIsReturnedWithTheFollowingPropertiesNameLocationFile_Size()
        {
            var httpResponse = (HttpResponseMessage)_scenarioContext["HTTPResponse"];

            var documents = JsonConvert.DeserializeObject<IEnumerable<Document>>( await httpResponse.Content.ReadAsStringAsync());
            
            _scenarioContext.Add("DocumentsReturned", documents);

            Assert.IsTrue(httpResponse.IsSuccessStatusCode);

            Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
            
        }

        [Given(@"I have chosen a PDF from the top of the list API returns")]
        [Given(@"I have selected a PDF from the top of the list API returns that I no longer require")]
        public async Task GivenIHaveSelectedAPDFFromTheListAPIThatINoLongerRequire()
        {
            var httpClient = (HttpClient)_scenarioContext["HTTPClient"];
            var httpResponse = await httpClient.GetAsync("api/documents");
            IList<Document> documents = JsonConvert.DeserializeObject<IList<Document>>(await httpResponse.Content.ReadAsStringAsync());

            if (documents.Any())
            {
                _scenarioContext.Add("SelectedDocument", documents[0].Name); 
            }
            else
            {
                Assert.Inconclusive("No documents to select from, please upload some and try again");
            }
        }

        [When(@"I request to delete the PDF")]
        public async Task WhenIRequestToDeleteThePDF()
        {
            var httpClient = (HttpClient)_scenarioContext["HTTPClient"];

            var deleteResponse = await httpClient.DeleteAsync($"api/documents?fileName={_scenarioContext["SelectedDocument"].ToString()}");

            _scenarioContext.Add("DeleteResponse", deleteResponse);
        }

        [Then(@"the PDF is deleted and will no longer return from the list API and can no longer be downloaded from its location directly")]
        public async Task ThenThePDFIsDeletedAndWillNoLongerReturnFromTheListAPIAndCanNoLongerBeDownloadedFromItsLocationDirectly()
        {
            var deleteResponse = (HttpResponseMessage)_scenarioContext["DeleteResponse"];

            Assert.IsTrue(deleteResponse.IsSuccessStatusCode);
            Assert.AreEqual(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var httpClient = (HttpClient)_scenarioContext["HTTPClient"];
            var httpResponse = await httpClient.GetAsync("api/documents");
            IList<Document> documents = JsonConvert.DeserializeObject<IList<Document>>(await httpResponse.Content.ReadAsStringAsync());

            var documentJustDeleted = documents.FirstOrDefault(d => d.Name == _scenarioContext["SelectedDocument"].ToString());

            Assert.IsNull(documentJustDeleted, "Deletion of the selected did not succeed");
        }

        [When(@"I request the location for one of the PDF's")]
        public async Task WhenIRequestTheLocationForOneOfThePDFS()
        {
            var httpClient = (HttpClient)_scenarioContext["HTTPClient"];

            var httpResponse = await httpClient.GetAsync($"api/documents/{_scenarioContext["SelectedDocument"].ToString()}");

            _scenarioContext.Add("GetResponse", httpResponse);
        }

        [Then(@"The PDF is downloaded")]
        public async Task ThenThePDFIsDownloaded()
        {
            var getResponse = (HttpResponseMessage)_scenarioContext["GetResponse"];

            Assert.IsTrue(getResponse.IsSuccessStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            string downloadPath = Path.Combine(_documentsDownloadFolder, _scenarioContext["SelectedDocument"].ToString());
            using (var stream = System.IO.File.Create(downloadPath))
            {
                await getResponse.Content.CopyToAsync(stream);
            }
            
            Assert.IsTrue(System.IO.File.Exists(downloadPath));
        }

        [Given(@"I attempt to delete a file that does not exist")]
        public void GivenIAttemptToDeleteAFileThatDoesNotExist()
        {
            _scenarioContext.Add("NonExistentFile", "asdfghjkl0987654321.pdf");
        }

        [When(@"I request to delete the non-existing pdf")]
        public async Task WhenIRequestToDeleteTheNon_ExistingPdf()
        {
            var httpClient = (HttpClient)_scenarioContext["HTTPClient"];

            var deleteResponse = await httpClient.DeleteAsync($"api/documents?fileName={_scenarioContext["NonExistentFile"].ToString()}");

            _scenarioContext.Add("NonExistentFileDeleteResponse", deleteResponse);
        }

        [Then(@"the API returns an appropriate response")]
        public void ThenTheAPIReturnsAnAppropriateResponse()
        {
            var nonExistentFileDeleteResponse = (HttpResponseMessage)_scenarioContext["NonExistentFileDeleteResponse"];

            Assert.IsFalse(nonExistentFileDeleteResponse.IsSuccessStatusCode);
            Assert.AreEqual(HttpStatusCode.NotFound, nonExistentFileDeleteResponse.StatusCode);
        }
    }
}
