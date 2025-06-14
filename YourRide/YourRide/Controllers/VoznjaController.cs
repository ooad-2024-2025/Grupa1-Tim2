using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR; // <-- OVO MORA BITI TU
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YourRide.Data;
using YourRide.Hubs;
using YourRide.Hubs;
using YourRide.Models;

namespace YourRide.Controllers
{
    public class VoznjaController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<Korisnik> _userManager; // <--- DODANO OVDJE

        private readonly IHubContext<NotificationHub> _hubContext;

        public VoznjaController(ApplicationDbContext context, UserManager<Korisnik> userManager, IHubContext<NotificationHub> hubContext) // <--- DODANO I OVDJE
        {
            _context = context;
            _userManager = userManager;
            // <--- INICIJALIZOVANO OVDJE
            _hubContext = hubContext;
        }

        // GET: Voznja
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Voznja.Include(v => v.Putnik).Include(v => v.Ruta).Include(v => v.Vozac);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Voznja/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voznja = await _context.Voznja
                .Include(v => v.Putnik)
                .Include(v => v.Ruta)
                .Include(v => v.Vozac)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (voznja == null)
            {
                return NotFound();
            }

            return View(voznja);
        }

        // GET: Voznja/Create
        [Authorize(Roles = "Nepostojeci")]
        public IActionResult Create()
        {
            ViewData["PutnikId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["RutaId"] = new SelectList(_context.Ruta, "ID", "ID");
            ViewData["VozacId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Voznja/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,status,PutnikId,VozacId,RutaId,VrijemePocetka,VrijemeZavrsetka")] Voznja voznja)
        {
            if (ModelState.IsValid)
            {
                _context.Add(voznja);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PutnikId"] = new SelectList(_context.Users, "Id", "Id", voznja.PutnikId);
            ViewData["RutaId"] = new SelectList(_context.Ruta, "ID", "ID", voznja.RutaId);
            ViewData["VozacId"] = new SelectList(_context.Users, "Id", "Id", voznja.VozacId);
            return View(voznja);
        }

        // GET: Voznja/Edit/5
        [Authorize(Roles = "Nepostojeci")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voznja = await _context.Voznja.FindAsync(id);
            if (voznja == null)
            {
                return NotFound();
            }
            ViewData["PutnikId"] = new SelectList(_context.Users, "Id", "Id", voznja.PutnikId);
            ViewData["RutaId"] = new SelectList(_context.Ruta, "ID", "ID", voznja.RutaId);
            ViewData["VozacId"] = new SelectList(_context.Users, "Id", "Id", voznja.VozacId);
            return View(voznja);
        }

        // POST: Voznja/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,status,PutnikId,VozacId,RutaId,VrijemePocetka,VrijemeZavrsetka")] Voznja voznja)
        {
            if (id != voznja.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(voznja);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VoznjaExists(voznja.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PutnikId"] = new SelectList(_context.Users, "Id", "Id", voznja.PutnikId);
            ViewData["RutaId"] = new SelectList(_context.Ruta, "ID", "ID", voznja.RutaId);
            ViewData["VozacId"] = new SelectList(_context.Users, "Id", "Id", voznja.VozacId);
            return View(voznja);
        }

        // GET: Voznja/Delete/5
        [Authorize(Roles = "Nepostojeci")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voznja = await _context.Voznja
                .Include(v => v.Putnik)
                .Include(v => v.Ruta)
                .Include(v => v.Vozac)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (voznja == null)
            {
                return NotFound();
            }

            return View(voznja);
        }

        // POST: Voznja/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var voznja = await _context.Voznja.FindAsync(id);
            if (voznja != null)
            {
                _context.Voznja.Remove(voznja);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VoznjaExists(int id)
        {
            return _context.Voznja.Any(e => e.ID == id);
        }

        [HttpPost]
        [Route("Voznja/Naruci")]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> NaruciVoznju([FromBody] RideRequestDto request)
        {
            // Validacija obaveznih polja
            if (string.IsNullOrWhiteSpace(request.PutnikId) ||
                string.IsNullOrWhiteSpace(request.DriverId) ||
                string.IsNullOrWhiteSpace(request.PocetnaAdresa))
            {
                return BadRequest("Nedostaju obavezni podaci (putnik ID, vozač ID, početna adresa).");
            }
            // Model.IsValid će i dalje biti true ako je OdredisnaAdresa null jer nije [Required]
            if (!ModelState.IsValid) // Možeš ostaviti ovu provjeru ako ti odgovara za druga potencijalna validacijska pravila
            {
                return BadRequest(ModelState);
            }


            try
            {
                var putnik = await _userManager.GetUserAsync(User);
                if (putnik == null || putnik.Id != request.PutnikId)
                {
                    return Unauthorized("Korisnik nije prijavljen ili neovlašten pristup.");
                }

                var vozac = await _userManager.FindByIdAsync(request.DriverId);
                if (vozac == null || vozac.Dostupnost != Dostupnost.Dostupan)
                {
                    return BadRequest("Odabrani vozač nije pronađen ili nije dostupan.");
                }

                // 1. Obrada početne lokacije (obavezno)
                // Provjeravamo da li lokacija već postoji u bazi po nazivu.
                // Ako postoji, koristimo nju. Ako ne, kreiramo novu.
                var pocetnaLokacija = await _context.Lokacija.FirstOrDefaultAsync(l => l.Naziv == request.PocetnaAdresa);
                if (pocetnaLokacija == null)
                {
                    pocetnaLokacija = new Lokacija { Naziv = request.PocetnaAdresa,
                        Grad = "Sarajevo"
                    };
                    _context.Lokacija.Add(pocetnaLokacija);
                    await _context.SaveChangesAsync();
                }

                // 2. Obrada krajnje lokacije (opcionalno)
                Lokacija? krajnjaLokacija = null;
                if (!string.IsNullOrWhiteSpace(request.OdredisnaAdresa))
                {
                    // Provjeravamo da li lokacija već postoji u bazi po nazivu.
                    // Ako postoji, koristimo nju. Ako ne, kreiramo novu.
                    krajnjaLokacija = await _context.Lokacija.FirstOrDefaultAsync(l => l.Naziv == request.OdredisnaAdresa);
                    if (krajnjaLokacija == null)
                    {
                        krajnjaLokacija = new Lokacija { Naziv = request.OdredisnaAdresa, Grad = "Sarajevo" };
                        _context.Lokacija.Add(krajnjaLokacija);
                        await _context.SaveChangesAsync();
                    }
                }

                // 3. Kreiraj NOVU rutu za ovu vožnju
                var ruta = new Ruta
                {
                    PocetnaLokacijaId = pocetnaLokacija.ID,
                    PocetnaLokacija = pocetnaLokacija,
                    KrajnjaLokacijaId = krajnjaLokacija?.ID,
                    KrajnjaLokacija = krajnjaLokacija
                    // DuzinaKM i OcekivanoVrijeme se mogu popuniti naknadno
                };
                _context.Ruta.Add(ruta); // Dodajemo novu rutu u kontekst
                await _context.SaveChangesAsync(); // Sačuvamo je da dobije ID

                var novaVoznja = new Voznja
                {
                    PutnikId = putnik.Id,
                    VozacId = vozac.Id,
                    RutaId = ruta.ID,
                    Ruta = ruta,
                    VrijemePocetka = DateTime.Now,
                    status = Status.naCekanju
                };

                _context.Voznja.Add(novaVoznja);
                await _context.SaveChangesAsync(); // <-- VAŽNO: Spremi prije slanja, jer ti treba novaVoznja.ID

             

                // 6. ŠALJI NOTIFIKACIJU VOZAČU PUTEM SIGNALR-a
                // Ključno: Clients.User(vozac.Id) šalje poruku samo konekcijama koje pripadaju tom korisniku
                // Na klijentskoj strani, vozač mora biti prijavljen.
                string putnikUserName = putnik.UserName ?? putnik.Email; // Koristi UserName ili Email kao identifikator putnika
                string odredisnaAdresaTekst = string.IsNullOrWhiteSpace(request.OdredisnaAdresa) ? "Nema određene krajnje adrese" : request.OdredisnaAdresa;

                await _hubContext.Clients.User(vozac.Id).SendAsync(
                    "ReceiveRideRequest", // Ime JavaScript metode koju će klijent pozvati
                    new
                    {
                        RideId = novaVoznja.ID,
                        PutnikUserName = putnikUserName,
                        PocetnaAdresa = request.PocetnaAdresa,
                        OdredisnaAdresa = odredisnaAdresaTekst
                    }
                );

                return Ok(new { message = "Vožnja uspješno naručena!", voznjaId = novaVoznja.ID, status = novaVoznja.status.ToString() });
            }
            catch (Exception ex)
            {
                string errorMessage = $"Interna greška servera: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $" Detalji: {ex.InnerException.Message}";
                }

                // Dodatni detalji za razvojni mod (nemoj ovo u produkciju)
#if DEBUG
                errorMessage += $" StackTrace: {ex.StackTrace}";
#endif

                // Vrati JSON odgovor sa detaljnijom porukom
                return StatusCode(500, new { message = errorMessage });
            }
        }
    }
}

  
