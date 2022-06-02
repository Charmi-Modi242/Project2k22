using physioCard.Helpers;
using System.ComponentModel.DataAnnotations;

namespace physioCard.Domain
{
    public class Patient : BaseEntity
    {
        public int patientID { get; set; }

        [Required(ErrorMessage = "ENTER PATIENT FIRST NAME")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "LENGTH OF FIRST NAME SHOULD NOT EXCEED 50")]
        public string? fname { get; set; }

        [Required(ErrorMessage = "ENTER PATIENT LAST NAME")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "LENGTH OF LAST NAME SHOULD NOT EXCEED 50")]
        public string? lname { get; set; }

        [Required(ErrorMessage = "CHOOSE PATIENT GENDER")]
        public string? gender { get; set; }

        [Required(ErrorMessage = "SELECT DATE OF BIRTH OF PATIENT")]
        public DateTime dob { get; set; }

        [Required(ErrorMessage = "ENTER PATIENT ADDRESS")]
        public string? address { get; set; }

        [Required(ErrorMessage = "ENTER PATIENT E-MAIL")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "E-MAIL IS INVALID")]
        public string? email { get; set; }

        [Required(ErrorMessage = "ENTER PATIENT CONTACT NO")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "CONTACT NO IS INVALID")]
        public Int64? contactno { get; set; }

        [Required(ErrorMessage = "ENTER CHIEF COMPLAINS OF PATIENT")]
        public string? chief_complains { get; set; }

        public string? medical_History { get; set; }

        [Required(ErrorMessage = "ENTER DATE OF PATIENT FIRST VISIT")]
        public DateTime registerdate { get; set; }

        //[Required(ErrorMessage = "SELECT PROFILE PHOTO OF PATIENT")]
        // custom validation stored in Helper
        [imageValidator(ErrorMessage = "INCLUDE ONLY IMAGE FILE(i.e., .png, .jpg, .jpeg)")]
        public IFormFile? photo_img { get; set; }
        public string? photo { get; set; }

        public int docid { get; set; }

        [Required(ErrorMessage = "SELECT CLINIC")]
        public int clinicid { get; set; }

        public double age { get; set; }

        public string? clinicname { get; set; }
    }
}
