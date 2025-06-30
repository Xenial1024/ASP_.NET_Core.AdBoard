using AdBoard.Core.Models.Domains;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AdBoard.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) : PageModel
    {
        readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        readonly UserManager<ApplicationUser> _userManager = userManager;
        static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
            [EmailAddress(ErrorMessage = "Podany email jest nieprawidłowy.")]
            [StringLength(254, ErrorMessage = "{0} nie może mieć więcej niż {1} znaków.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Pole Hasło jest wymagane.")]
            [StringLength(100, ErrorMessage = "{0} musi mieć od {2} do {1} znaków.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Hasło")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
            [DataType(DataType.Password)]
            [Display(Name = "Potwierdź hasło")]
            [Compare("Password", ErrorMessage = "Hasło i jego potwierdzenie różnią się.")]
            public string ConfirmPassword { get; set; }

            [StringLength(100, ErrorMessage = "{0} musi mieć od {2} do {1} znaków.", MinimumLength = 2)]
            [Display(Name = "Nazwa użytkownika")]
            public string? Name { get; set; }

            [StringLength(20, ErrorMessage = "{0} musi mieć od {2} do {1} znaków.", MinimumLength = 5)]
            [Phone(ErrorMessage = "Podany nr telefonu jest nieprawidłowy.")]
            [Display(Name = "Numer telefonu")]
            public string? PhoneNumber { get; set; }

            [Display(Name = "Zapamiętaj mnie")]
            public bool RememberMe { get; set; }
        }

        public async System.Threading.Tasks.Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = [.. (await _signInManager.GetExternalAuthenticationSchemesAsync())];
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = [.. (await _signInManager.GetExternalAuthenticationSchemesAsync())];
            if (ModelState.IsValid)
            {
                ApplicationUser user = new() 
                { 
                    UserName = Input.Email, 
                    Email = Input.Email,
                    Name = Input.Name,           
                    PhoneNumber = Input.PhoneNumber
                };
                IdentityResult result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.Info("Użytkownik stworzył nowe hasło.");

                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                    await _signInManager.SignInAsync(user, isPersistent: Input.RememberMe);
                    return LocalRedirect(returnUrl);
                }
                foreach (IdentityError error in result.Errors)
                {
                    if (error.Code == "DuplicateUserName")
                        ModelState.AddModelError("Input.Email", "Ten adres email jest już zajęty.");
                    else
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}