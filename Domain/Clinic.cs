using physioCard.Helpers;
using System.ComponentModel.DataAnnotations;

namespace physioCard.Domain
{
    public class Clinic : BaseEntity
    {
        public int clinicID { get; set; }
        [Required(ErrorMessage = "ENTER CLINIC NAME")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "LENGTH OF CLINIC NAME SHOULD NOT EXCEED 50")]
        public string? name { get; set; }

        [Required(ErrorMessage = "ENTER CLINIC ADDRESS")]
        public string? address { get; set; }

        [Required(ErrorMessage = "ENTER CLINIC CONTACT NO")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "CONTACT NO IS INVALID")]
        public Int64 contactno { get; set; }

        [Required(ErrorMessage = "ENTER CLINIC GST NO")]
        [RegularExpression(@"^([A-Z0-9]{15})$", ErrorMessage = "GST NO IS INVALID")]
        public string? GSTno { get; set; }
        [Required(ErrorMessage = "SELECT CLINIC HEAD SIGNATURE")]
        [imageValidator(ErrorMessage = "INCLUDE ONLY IMAGE FILE(i.e., .png, .jpg, .jpeg)")]
        public IFormFile? head_signature_img { get; set; }
        public string? head_signature { get; set; }
        [Required(ErrorMessage = "SELECT CLINIC LOGO")]
        [imageValidator(ErrorMessage = "INCLUDE ONLY IMAGE FILE(i.e., .png, .jpg, .jpeg)")]
        public IFormFile? logo_img { get; set; }
        public string? logo { get; set; }
    }
}
