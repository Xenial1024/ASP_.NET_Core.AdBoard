#nullable disable

using System.ComponentModel.DataAnnotations;

namespace AdBoard.Areas.Identity.Pages.Account.Manage
{
    public partial class ChangePasswordModel
    {
        public class InputModel
        {
            [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
            [DataType(DataType.Password)]
            [Display(Name = "Obecne hasło")]
            public string OldPassword { get; set; }

            [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
            [StringLength(100, ErrorMessage = "{0} musi mieć od {2} do {1} znaków.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Nowe hasło")]
            public string NewPassword { get; set; }

            [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
            [DataType(DataType.Password)]
            [Display(Name = "Potwierdź nowe hasło")]
            [Compare("NewPassword", ErrorMessage = "Hasło i jego potwierdzenie różnią się.")]
            public string ConfirmPassword { get; set; }
        }
    }
}
