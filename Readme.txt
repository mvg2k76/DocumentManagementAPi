The solution ManageDocuments has been developed using VS2019 (Community Edition) & ASP.Net Core 3.1 and contain two projects

1. ManageDocuments.Api

   The project contians api controller (documents) which provides action method for 
   1. Getting a specified document
   2. Getting a list of documents 
   3. Uploading a document
   4. Deleting a document
   
   The api service is configured to run at url http://localhost.com:5000/api/documents

   To keep the project simple, file system folder has been used for storing the documents.
   Uploaded documents are stored under "C:\temp\DocumentStore\UploadedDocuments". 
   The location is specified in the appsettings.json using key "DocumentStoreLocation" 
   
   Repository pattern has been used to keep the interaction with the file system separate to the controller class. 
   This should allow the api service to switch to a different data store without many changes.
   
2. ManageDocuments.Specflow.Tests

   The project has one Specflow feature (ManageDocumentApiService) that has most of the scenarios given in the test
   The tests create and depend on 2 folders which are set in app.config file
   
   1. DocumentSourceFolder: Any documents for upload to the ManageDocuments Api service should exist here. 
      The location is set to C:\temp\DocumentStore\SourceDocuments
	  
   2. DocumentDownloadFolder: Any documents downloaded from the ManageDocuments Api service are stored here.
	  The location is set to C:\temp\DocumentStore\DownloadedDocuments
	  
   3. DocumentStoreLocation: Documents uploaded are stored here. 
      Please make sure this setting has the same path provided in appSetting.Json file in ManageDocuments.Api project

Note: Before you run Specflow tests please 
      Copy the SmallDocument.pdf and BigDocument.pdf provided to the path specified 
	  in DocumentSourceFolder (i.e. C:\temp\DocumentStore\SourceDocuments) 
