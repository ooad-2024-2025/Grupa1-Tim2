using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using YourRide.Models;

namespace YourRide.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<Korisnik> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<Korisnik> userManager)
        {
            
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()

        {

            var user = await _userManager.GetUserAsync(User);
            bool isPutnik = user != null && await _userManager.IsInRoleAsync(user, "Putnik");
            bool isTehnicka= user!=null && await _userManager.IsInRoleAsync(user, "TehnickaPodrska");

            ViewData["IsPutnik"] = isPutnik;
            ViewData["IsTehnicka"] = isTehnicka;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult PrikazSlobodnihVozaca()
        {
            ViewData["HideFooter"] = true;
            ViewData["HideHeader"] = true;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
