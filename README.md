# DocumentManagementAPi 
### Restful Api project to manage pdf documents

The solution ManageDocuments has been developed using VS2019 (Community Edition) & .Net Core 3.1

### Please follow the below instructions to the setup the solution locally. 
1. Download the code as .zip file to you pc.
2. Make sure the code is extracted to a location **other than the "Downloads"** folder to prevent any restrictions when the solution loads.
3. Open Visual Studio 2019 and navigate to the solution file **ManageDocuments.sln** and open and wait for it to load completely
4. Now Build the solution.
5. Open the Test Explorer to see Specflow Tests. 

### The Solution  contain two projects
#### 1. ManageDocuments.Api

   The project contians api controller (documents) which provides action methods for 
   1. Getting a specified document
   2. Getting a list of documents 
   3. Uploading a document
   4. Deleting a document
   
   The api service is configured to run at url http://localhost.com:5000/api/documents

   To keep the project simple, file system folder has been used for storing the documents.
   
   Uploaded documents are stored under **"C:\temp\DocumentStore\UploadedDocuments".** 
   The location is specified in the appsettings.json using key **"DocumentStoreLocation"** 
   
   Repository pattern has been used to keep the interaction with the file system separate to the controller class. 
   This should allow the api service to switch to a different data store without many changes.
   
#### 2. ManageDocuments.Specflow.Tests

   The project has one Specflow feature (ManageDocumentApiService) that has most of the scenarios given in the test.
   The project creates and depend on 2 folders **(DocumentSourceFolder, DocumentDownloadFolder)** which are set in app.config file
   
   1. DocumentSourceFolder: Any documents for upload to the ManageDocuments Api service should exist here. 
      The location is set to **C:\temp\DocumentStore\SourceDocuments**
	  
   2. DocumentDownloadFolder: Any documents downloaded from the ManageDocuments Api service are stored here.
	  The location is set to **C:\temp\DocumentStore\DownloadedDocuments**
	  
   3. DocumentStoreLocation: Documents uploaded are stored here. 
      Please make sure this setting has the same path provided in appSetting.Json file in ManageDocuments.Api project

### Note: Before you run Specflow tests. 
  Some of the Specflow scenarios use files SmallDocument.pdf and BigDocument.pdf 
  Please Copy them to the path specified in the setting **DocumentSourceFolder** i.e. **C:\temp\DocumentStore\SourceDocuments** before any tests are run
