using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ManageDocuments.Api.ModelValidation
{
    public class ValidateDocumentAttribute : ValidationAttribute
    {
        public string GetInvalidDocumentTypeErrorMessage() => "Uploaded document must be a pdf ";
        public string SizeExceedsErrorMessage() => "Size of the file is greater than 5MB";

        protected override ValidationResult IsValid(object fileToUpload,
            ValidationContext validationContext)
        {
            if (fileToUpload != null)
            {
                var file = (IFormFile)fileToUpload;

                if (!file.ContentType.Equals("application/pdf"))
                {
                    return new ValidationResult(GetInvalidDocumentTypeErrorMessage());
                }

                if (file.Length > 5242880)
                {
                    return new ValidationResult(SizeExceedsErrorMessage());
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("File not specified");
        }
    }
}
