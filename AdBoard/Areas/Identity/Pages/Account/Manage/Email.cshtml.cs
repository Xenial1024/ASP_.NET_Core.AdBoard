#nullable disable

using AdBoard.Core.Models.Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AdBoard.Areas.Identity.Pages.Account.Manage
{
    public class EmailModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender) : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly IEmailSender _emailSender = emailSender;

        [Display(Name = "Obecny email")]
        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Pole Email jest wymagane.")]
            [EmailAddress(ErrorMessage = "Podany adres email jest nieprawidłowy.")]
            [StringLength(254, ErrorMessage = "{0} nie może mieć więcej niż {1} znaków.")]
            [Display(Name = "Nowy email")]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound($"Nie udało się załadować użytkownika o id '{_userManager.GetUserId(User)}'.");

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound($"Nie udało się załadować użytkownika o id '{_userManager.GetUserId(User)}'.");

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                var existingUser = await _userManager.FindByEmailAsync(Input.NewEmail);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    ModelState.AddModelError("Input.NewEmail", "Ten adres email jest już zajęty.");
                    await LoadAsync(user);
                    return Page();
                }
                var result = await _userManager.SetEmailAsync(user, Input.NewEmail);
                if (result.Succeeded)
                {
                    await _userManager.SetUserNameAsync(user, Input.NewEmail);

                    StatusMessage = "Adres email został pomyślnie zmieniony.";
                }
                else
                    StatusMessage = "Błąd podczas zmiany adresu email.";

                return RedirectToPage();
            }

            StatusMessage = "Nowy adres email musi różnić się od obecnego.";
            return RedirectToPage();
        }
    }
}