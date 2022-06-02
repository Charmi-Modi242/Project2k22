using System.ComponentModel.DataAnnotations;

namespace physioCard.Domain
{
    public class Login : BaseEntity
    {
        [Required(ErrorMessage = "ENTER YOUR E-MAIL")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "E-MAIL IS INVALID")]
        public string? email { get; set; }

        [Required(ErrorMessage = "ENTER YOUR PASSWORD")]
        public string? password { get; set; }

        [Display(Name = "Remember Me")]
        public bool rememberMe { get; set; }
    }
}
