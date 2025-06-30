using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AdBoard.Core.Models.Domains
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Ad> Ads { get; set; } = [];

        [StringLength(100, ErrorMessage = "{0} musi mieć od {2} do {1} znaków.", MinimumLength = 2)]
        [Display(Name = "Nazwa użytkownika:")]
        public string? Name { get; set; }

        [StringLength(20, ErrorMessage = "{0} musi mieć od {2} do {1} znaków.", MinimumLength = 5)]
        [Phone(ErrorMessage = "Podany numer telefonu jest nieprawidłowy.")]
        [Display(Name = "Numer telefonu:")]
        public override string? PhoneNumber { get; set; }
    }
}