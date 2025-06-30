#nullable disable

using System.ComponentModel.DataAnnotations;

namespace AdBoard.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel
    {
        public class InputModel
        {
            //nazwa użytkownika i numer telefonu są nieobowiązkowe
            [StringLength(100, ErrorMessage = "{0} musi mieć od {2} do {1} znaków.", MinimumLength = 2)]
            [Display(Name = "Nazwa użytkownika:")]
            public string Username { get; set; }

            [StringLength(20, ErrorMessage = "{0} musi mieć od {2} do {1} znaków.", MinimumLength = 5)]
            [Phone(ErrorMessage = "Podany nr telefonu jest nieprawidłowy.")]
            [Display(Name = "Numer telefonu:")]
            public string PhoneNumber { get; set; }
        }
    }
}
