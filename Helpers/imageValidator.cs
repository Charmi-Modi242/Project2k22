using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace physioCard.Helpers
{
    public class imageValidator : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // checks if no file selected -- not possible as we have used required attribute
            //if (value == null)
            //{
            //    return new ValidationResult("SELECT YOUR PHOTO");
            //}
            if (value != null)
            {
                // creating file object and storing file
                var theFile = value as IFormFile;
                // getting the file extension
                string extension = Path.GetExtension(theFile.FileName);
                string[] fileMimeName = { "image/png", "image/jpg", "image/jpeg" };
                string[] fileExt = { ".png", ".jpg", ".jpeg" };

                // checking if the file is of image type or not
                if (!(Array.IndexOf(fileMimeName, theFile.ContentType) >= 0) && !(Array.IndexOf(fileExt, extension) >= 0))
                {
                    return new ValidationResult(ErrorMessage);
                }
                return ValidationResult.Success;
            }
            return ValidationResult.Success;
        }
    }
}
