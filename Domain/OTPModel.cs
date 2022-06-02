using System.ComponentModel.DataAnnotations;

namespace physioCard.Domain
{
    public class OTPModel : BaseEntity
    {
        [Required(ErrorMessage = "ENTER YOUR E-MAIL")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "E-MAIL IS INVALID")]
        public string? email { get; set; }

        //[Required(ErrorMessage = "Enter OTP")]
        public string? OTP { get; set; }

        //[PasswordValidator]
        public string? password { get; set; }

        [Compare("password", ErrorMessage = "PASSWORD AND CONFIRM PASSWORD MUST MATCH")]
        public string? confirmpassword { get; set; }
        public string? oldpassword { get; set; }
    }
}
