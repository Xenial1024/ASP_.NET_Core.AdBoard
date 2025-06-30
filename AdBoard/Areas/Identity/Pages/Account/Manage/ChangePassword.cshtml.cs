#nullable disable
using AdBoard.Core.Models.Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdBoard.Areas.Identity.Pages.Account.Manage
{
    public partial class ChangePasswordModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager) : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound($"Nie udało się załadować użytkownika o id '{_userManager.GetUserId(User)}'.");

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
                return RedirectToPage("./SetPassword");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound($"Nie udało się załadować użytkownika o id '{_userManager.GetUserId(User)}'.");

            if (await _userManager.CheckPasswordAsync(user, Input.NewPassword))
            {
                ModelState.AddModelError("Input.NewPassword", "Nowe hasło musi się różnić od obecnego.");
                return Page();
            }

            if (!ValidatePasswordComplexity(Input.NewPassword))
            {
                ModelState.AddModelError("Input.NewPassword", "Hasło musi zawierać co najmniej jedną małą literę, jedną dużą literę, jedną cyfrę i jeden znak niealfanumeryczny.");
                return Page();
            }

            IdentityResult changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    if (error.Code == "PasswordMismatch")
                        ModelState.AddModelError("Input.OldPassword", "Podane obecne hasło jest nieprawidłowe.");
                    else
                        ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);

            _logger.Info("Użytkownik o id {UserId} pomyślnie zmienił swoje hasło.", _userManager.GetUserId(User));
            StatusMessage = "Hasło zostało zmienione.";

            return RedirectToPage();
        }
        private static bool ValidatePasswordComplexity(string password)
        {
            bool hasLower = password.Any(char.IsLower);
            bool hasUpper = password.Any(char.IsUpper);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasNonAlphanumeric = password.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));

            Console.WriteLine($"Password complexity check: Lower={hasLower}, Upper={hasUpper}, Digit={hasDigit}, NonAlphanumeric={hasNonAlphanumeric}");

            return hasLower && hasUpper && hasDigit && hasNonAlphanumeric;
        }
    }
}
