Feature: ManageDocumentApiService
	The ManageDocument API service provides
	document management functions for pdf files

Background: Make sure the ManageDocuments Api service is ready to call
	Given The ManageDocuments Api service is ready
	And Document source and download folders exist
	
Scenario: Upload a pdf document less than 5MB
	Given I have a PDF file 'SmallDocument.pdf' under document source folder  to upload
	When I send the PDF to the API
	Then It is uploaded successfully

Scenario: Upload a pdf document greater than 5MB
	Given I have a PDF file 'BigDocument.pdf' under document source folder  to upload
	When I send the PDF to the API
	Then The API does not accept the file and returns the appropriate messaging and status

Scenario: Get the list of documents
	When I call the API to get a list of documents
	Then a list of PDFs’ is returned with the following properties: name, location, file-size

Scenario: Delete a selected pdf file
	Given I make sure atleast one document exists in uploaded documents folder
	And I have selected a PDF from the top of the list API returns that I no longer require
	When I request to delete the PDF
	Then the PDF is deleted and will no longer return from the list API and can no longer be downloaded from its location directly

Scenario: Download a selected pdf file
	Given I make sure atleast one document exists in uploaded documents folder
	And I have chosen a PDF from the top of the list API returns
	When I request the location for one of the PDF's
	Then The PDF is downloaded

Scenario: Delete a non-existent file
	Given I attempt to delete a file that does not exist
	When I request to delete the non-existing pdf
	Then the API returns an appropriate response