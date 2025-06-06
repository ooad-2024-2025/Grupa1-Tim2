using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourRide.Data;

namespace YourRide.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using YourRide.Models;

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
    }


}
