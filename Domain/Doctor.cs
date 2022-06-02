using physioCard.Helpers;
using System.ComponentModel.DataAnnotations;

namespace physioCard.Domain
{
    public class Doctor : BaseEntity
    {
        public int docID { get; set; }

        [Required(ErrorMessage = "ENTER FIRST NAME")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "LENGTH OF FIRST NAME SHOULD NOT EXCEED 50")]
        public string? fname { get; set; }

        [Required(ErrorMessage = "ENTER LAST NAME")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "LENGTH OF LAST NAME SHOULD NOT EXCEED 50")]
        public string? lname { get; set; }

        [Required(ErrorMessage = "ENTER E-MAIL")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "E-MAIL IS INVALID")]
        public string? email { get; set; }

        [Required(ErrorMessage = "ENTER PASSWORD")]
        // custom validation stored in Helper
        [PasswordValidator]
        public string? password { get; set; }

        [Compare("password", ErrorMessage = "PASSWORD AND CONFIRM PASSWORD MUST MATCH")]
        public string? confirmPassword { get; set; }

        [Required(ErrorMessage = "ENTER CONTACT NO")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "CONTACT NO IS INVALID")]
        public Int64 contactno { get; set; }

        //[Required(ErrorMessage = "SELECT YOUR PROFILE PHOTO")]
        // custom validation stored in Helper
        [imageValidator(ErrorMessage = "INCLUDE ONLY IMAGE FILE(i.e., .png, .jpg, .jpeg)")]
        public IFormFile? photo_img { get; set; }
        public string? photo { get; set; }
        //[Required(ErrorMessage = "SELECT ANY CLINIC")]
        //public int[]? clinicID { get; set; }
    }
}
