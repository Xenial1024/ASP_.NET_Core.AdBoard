using AdBoard.Core.Models.Domains;
using AdBoard.Persistence.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdBoard.Controllers
{
    public class AddEditDeleteController(IAddEditDeleteService addEditDeleteService, UserManager<ApplicationUser> userManager) : Controller
    {
        readonly IAddEditDeleteService _addEditDeleteService = addEditDeleteService;
        readonly UserManager<ApplicationUser> _userManager = userManager;
        static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [HttpGet]
        [Authorize]
        public IActionResult Add()
        {
            ViewBag.Title = "Dodaj ogłoszenie";
            ViewBag.Categories = Ad.Categories;
            ViewBag.Units = Ad.Units;
            return View("AddOrEdit", new Ad());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Ad model, IFormFileCollection newImages)
        {
            ViewBag.Title = "Dodaj ogłoszenie";
            ViewBag.Categories = Ad.Categories;
            ViewBag.Units = Ad.Units;

            string[] freeCategories = ["Oddam za darmo", "Przyjmę za darmo", "Poznam panią", "Poznam pana"];

            if (freeCategories.Contains(model.Category))
                model.Value = null;

            model.UserId = _userManager.GetUserId(User);

            ModelState.Remove("UserId"); //żeby nie pojawił się błąd The UserId field is required.
            ModelState.Remove("User");

            if (!ModelState.IsValid)
            {
                _logger.Warn($"Nieudana próba dodania ogłoszenia przez użytkownika {model.UserId}. Błędy: {string.Join("; ", ViewBag.ModelErrors)}"); 
                return View("AddOrEdit", model);
            }

            try
            {
                await _addEditDeleteService.AddAdAsync(model, newImages, model.UserId);
                return RedirectToAction("ListOfAds", "Browse", new { UserId = "current" });
            }

            catch (Exception ex)
            {
                _logger.Error($"Błąd podczas dodawania ogłoszenia: {ex.Message}");
                if (ex.InnerException != null)
                    _logger.Error($"Inner exception: {ex.InnerException.Message}");
                return View("AddOrEdit", model);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            string userId = _userManager.GetUserId(User);
            Ad ad = await _addEditDeleteService.GetAdByIdAsync(id, userId);
            if (ad == null || ad.UserId != _userManager.GetUserId(User))
                return NotFound();

            ViewBag.Title = "Edycja ogłoszenia";
            ViewBag.Categories = Ad.Categories;
            ViewBag.Units = Ad.Units;
            return View("AddOrEdit", ad);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Ad model, IFormFileCollection newImages, int[] removeImages)
        {
            ViewBag.Title = "Edycja ogłoszenia";
            ViewBag.Categories = Ad.Categories;
            ViewBag.Units = Ad.Units;

            ModelState.Remove("User");

            if (!ModelState.IsValid)
            {
                var modelErrors = new List<string>();

                foreach (var kvp in ModelState)
                {
                    var key = kvp.Key;
                    var value = kvp.Value;

                    if (value.Errors.Count > 0)
                    {
                        foreach (var error in value.Errors)
                        {
                            var errorMsg = !string.IsNullOrEmpty(error.ErrorMessage)
                                ? error.ErrorMessage
                                : error.Exception?.Message ?? "Unknown error";
                            modelErrors.Add($"{key}: {errorMsg}");
                        }
                    }
                }

                _logger.Warn($"Nieudana próba edycji ogłoszenia przez użytkownika o id {model.UserId}.");
                _logger.Warn($"Błędy walidacji: {(modelErrors.Count > 0 ? string.Join("; ", modelErrors) : "Brak szczegółowych błędów")}");
                return View("AddOrEdit", model);
            }

            try
            {
                string userId = _userManager.GetUserId(User);
                model.UserId = userId;

                Ad updatedAd = await _addEditDeleteService.EditAdAsync(model, newImages, removeImages);
                if (updatedAd == null)
                    return NotFound();

                return RedirectToAction("ListOfAds", "Browse", new { UserId = "current" });
            }
            catch (Exception ex)
            {
                _logger.Error($"Błąd podczas edycji ogłoszenia o id {model.Id}: {ex.Message}");
                if (ex.InnerException != null)
                    _logger.Error($"Inner exception: {ex.InnerException.Message}");
                return View("AddOrEdit", model);
            }
            
        }
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            string userId = _userManager.GetUserId(User);
            Ad ad = await _addEditDeleteService.GetAdByIdAsync(id, userId);

            if (ad == null || ad.UserId != _userManager.GetUserId(User))
                return NotFound();

            return View(ad);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                try
                {
                    string userId = _userManager.GetUserId(User);
                    bool deleted = await _addEditDeleteService.DeleteAdAsync(id, userId);

                    if (!deleted)
                        return NotFound();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Błąd podczas usuwania ogłoszenia: {ex.Message}");
                }

                return RedirectToAction("ListOfAds", "Browse", new { UserId = "current" });

            }
            catch (Exception ex)
            {
                _logger.Error($"Błąd podczas usuwania ogłoszenia: {ex.Message}");
            }

            return RedirectToAction("ListOfAds", "Browse", new { UserId = "current" });
        }
    }
}