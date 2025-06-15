using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using YourRide.Data;
using YourRide.Models;

namespace YourRide.Controllers
{
    [Authorize]
    public class PodrskaPorukaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;
        private readonly ILogger<PodrskaPorukaController> _logger;

        public PodrskaPorukaController(ApplicationDbContext context, UserManager<Korisnik> userManager, ILogger<PodrskaPorukaController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }



        [HttpGet]
        public IActionResult NovaPoruka()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NovaPoruka(PodrskaPorukaDto dto)
        {
            _logger.LogInformation("NovaPoruka (POST) akcija pozvana.");

            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null)
            {
                _logger.LogWarning("Korisnik nije prijavljen.");
                return RedirectToPage("/Account/Login");
            }

            if (ModelState.IsValid)
            {
                var poruka = new PodrskaPoruka
                {
                    Naslov = dto.Naslov,
                    Poruka = dto.Poruka,
                    KorisnikId = korisnik.Id,
                    DatumSlanja = DateTime.Now
                };

                try
                {
                    _context.PorukePodrske.Add(poruka);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Poruka uspješno spremljena.");
                    return RedirectToAction("NovaPoruka");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Greška pri spremanju poruke.");
                    ModelState.AddModelError("", "Greška pri slanju poruke. Pokušajte ponovo.");
                }
            }
            else
            {
                _logger.LogWarning("ModelState nije validan.");
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        _logger.LogError($"Greška za polje '{kv.Key}': {error.ErrorMessage}");
                    }
                }
            }

            return View(dto);
        }

        public async Task<IActionResult> MojePoruke()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null)
                return RedirectToPage("/Account/Login");

            var poruke = await _context.PorukePodrske
                .Where(p => p.KorisnikId == korisnik.Id)
                .ToListAsync();

            return View(poruke);
        }

        public async Task<IActionResult> MojePorukePartial()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            var poruke = await _context.PorukePodrske
                .Where(p => p.KorisnikId == korisnik.Id)
                .ToListAsync();

            return PartialView("_MojePorukePartial", poruke);
        }
    }
}
