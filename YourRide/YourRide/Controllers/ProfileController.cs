using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YourRide.Data;
using YourRide.Models;

namespace YourRide.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<Korisnik> _userManager;

        public ProfileController(UserManager<Korisnik> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDriverProfile(bool dostupnost, double latitude, double longitude)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            double lat = Math.Round(latitude / 1_000_0000.0, 4);
            double lon = Math.Round(longitude / 1_000_0000.0, 4);

            user.Dostupnost = dostupnost ? Dostupnost.Zauzet : Dostupnost.Dostupan;
            user.Latitude = lat;
            user.Longitude = lon;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {

                return RedirectToAction("Index");
            }


            ModelState.AddModelError("", "Failed to update user profile");
            return View("Index", user);
        }
    }
}
