using AdBoard.Core.Models.Domains;
using AdBoard.Persistence.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdBoard.Controllers
{
    public class BrowseController(IBrowseService browseService, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly IBrowseService _browseService = browseService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<IActionResult> ListOfAds(string UserId = "others")
        {
            string currentUserId = _userManager.GetUserId(User);
            List<Ad> ads = await _browseService.GetAdsForListingAsync(UserId, currentUserId);

            if (UserId == "current")
            {
                ViewBag.Title = "Zarządzaj ogłoszeniami";
                ViewBag.ShowManageButtons = true;
                ViewBag.ShowEmptyMessage = (ads.Count == 0);
            }
            else
            {
                ViewBag.Title = "Przeglądaj ogłoszenia";
                ViewBag.ShowManageButtons = false;
            }

            ViewBag.UserId = UserId;
            return View(ads);
        }

        public async Task<IActionResult> Details(int id)
        {
            Ad ad = await _browseService.GetAdDetailsAsync(id);

            if (ad == null)
                return NotFound();

            return View(ad);
        }
    }
}