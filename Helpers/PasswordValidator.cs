using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace physioCard.Helpers
{
    public class PasswordValidator : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            // checking the password strength
            string str = value.ToString();
            if (!Regex.IsMatch(str, @"(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^A-Za-z0-9])") || str.Length != 8)
            {
                string error = "RULES FOR PASSWORD: ";
                if (!Regex.IsMatch(str, @"^(?=.*[A-Z])"))
                {
                    error += "ONE UPPER-CASE CHARACTER, ";
                }
                if (!Regex.IsMatch(str, @"^(?=.*[a-z])"))
                {
                    error += "ONE LOWER-CASE CHARACTER, ";
                }
                if (!Regex.IsMatch(str, @"^(?=.*[0-9])"))
                {
                    error += "ONE NUMBER, ";
                }
                if (!Regex.IsMatch(str, @"^(?=.*[^a-zA-Z0-9])"))
                {
                    error += "ONE SPECIAL CHARACTER, ";
                }
                if (str.Length != 8)
                {
                    error += "LENGTH MUST BE 8";
                }
                return new ValidationResult(error);
            }
            return ValidationResult.Success;
        }
    }
}
