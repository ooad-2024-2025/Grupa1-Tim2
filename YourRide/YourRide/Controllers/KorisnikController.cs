using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using YourRide.Data;
using YourRide.Models;
using YourRide.Models.ViewModels;

namespace YourRide.Controllers
{
    public class KorisnikController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KorisnikController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Korisnik/Registracija
        public IActionResult Registracija()
        {
            return View();
        }

        // POST: Korisnik/Registracija
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registracija(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var korisnik = new Korisnik
            {
                UserName = model.UserName,
                Email = model.Email,
                VrstaRacuna = model.VrstaRacuna,
                Dostupnost = model.VrstaRacuna == VrstaRacuna.Vozac ? Dostupnost.Zauzet : null
            };

            // Hashiraj lozinku
            var hasher = new PasswordHasher<Korisnik>();
            korisnik.Password = hasher.HashPassword(korisnik, model.Password);

            _context.Korisnik.Add(korisnik);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
        // GET: Prijava
        public IActionResult Prijava()
        {
            return View();
        }

        // POST: Prijava
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Prijava(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var korisnik = await _context.Korisnik
    .FirstOrDefaultAsync(k => k.UserName == model.UserName);

            if (korisnik == null)
            {
                ModelState.AddModelError(string.Empty, "Neispravno korisničko ime ili lozinka.");
                return View(model);
            }

            var hasher = new PasswordHasher<Korisnik>();
            var result = hasher.VerifyHashedPassword(korisnik, korisnik.Password, model.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Neispravno korisničko ime ili lozinka.");
                return View(model);
            }

            // Prijava uspješna
            TempData["Poruka"] = "Prijava uspješna!";
            return RedirectToAction("Index", "Home");
        }
        // GET: Korisnik
        public async Task<IActionResult> Index()
        {
            return View(await _context.Korisnik.ToListAsync());
        }

        // GET: Korisnik/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var korisnik = await _context.Korisnik
                .FirstOrDefaultAsync(m => m.ID == id);
            if (korisnik == null) return NotFound();

            return View(korisnik);
        }

        // GET: Korisnik/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Korisnik/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,UserName,Password,Email,VrstaRacuna,Dostupnost")] Korisnik korisnik)
        {
            if (ModelState.IsValid)
            {
                _context.Add(korisnik);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(korisnik);
        }

        // GET: Korisnik/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var korisnik = await _context.Korisnik.FindAsync(id);
            if (korisnik == null) return NotFound();

            return View(korisnik);
        }

        // POST: Korisnik/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,UserName,Password,Email,VrstaRacuna,Dostupnost")] Korisnik korisnik)
        {
            if (id != korisnik.ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(korisnik);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KorisnikExists(korisnik.ID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(korisnik);
        }

        // GET: Korisnik/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var korisnik = await _context.Korisnik
                .FirstOrDefaultAsync(m => m.ID == id);
            if (korisnik == null) return NotFound();

            return View(korisnik);
        }

        // POST: Korisnik/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var korisnik = await _context.Korisnik.FindAsync(id);
            if (korisnik != null)
            {
                _context.Korisnik.Remove(korisnik);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool KorisnikExists(int id)
        {
            return _context.Korisnik.Any(e => e.ID == id);
        }
    }
}
