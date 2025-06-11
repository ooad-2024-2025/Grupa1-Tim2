using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourRide.Data;
using YourRide.Models;
using Microsoft.AspNetCore.SignalR;
using YourRide.Hubs;

namespace YourRide.Controllers
{
    [Authorize(Roles = "Vozac")]
    public class Vozac2Controller : Controller
    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public Vozac2Controller(UserManager<Korisnik> userManager, ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _userManager = userManager;
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Dashboard()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound($"Nije pronađen korisnik s ID-om '{_userManager.GetUserId(User)}'.");
            }

            var aktivneVoznje = await _context.Voznja
                .Where(v => v.VozacId == currentUser.Id && (v.status == Status.Prihvacena)) // Fokusiramo se na prihvaćene vožnje
                .Include(v => v.Ruta)
                    .ThenInclude(r => r.PocetnaLokacija)
                .Include(v => v.Ruta)
                    .ThenInclude(r => r.KrajnjaLokacija)
                .Include(v => v.Putnik)
                .ToListAsync();

            ViewBag.DostupnostStatus = currentUser.Dostupnost.ToString();

            return View(aktivneVoznje);
        }

        // NOVA AKCIJA: ZavrsiVoznju
        [HttpPost]
        [ValidateAntiForgeryToken] // Vrlo važno za sigurnost
        public async Task<IActionResult> ZavrsiVoznju([FromBody] int voznjaId)
        {
            var voznja = await _context.Voznja
                .Include(v => v.Vozac) // Uključi vozača da bismo mu promijenili status
                .Include(v => v.Putnik) // Uključi putnika za notifikaciju
                .FirstOrDefaultAsync(v => v.ID == voznjaId);

            if (voznja == null)
            {
                return NotFound(new { message = "Vožnja nije pronađena." });
            }

            // Provjeri da li je prijavljeni vozač zaista vozač te vožnje
            var currentDriver = await _userManager.GetUserAsync(User);
           

            // Provjeri status vožnje prije završetka
            if (voznja.status != Status.Prihvacena)
            {
                return BadRequest(new { message = "Vožnja mora biti u statusu 'Prihvaćena' da bi se mogla završiti." });
            }

            // Ažuriraj status vožnje
            voznja.status = Status.Zavrsena; // Pretpostavljam da imaš enum Status.Zavrsena
            voznja.VrijemeZavrsetka = DateTime.Now; // Postavi vrijeme završetka
            _context.Voznja.Update(voznja);

            // Postavi vozača kao dostupnog (Dostupnost.Dostupan)
            // Uvijek ažuriramo vozačev status ako je upravo završio vožnju.
            currentDriver.Dostupnost = Dostupnost.Dostupan; // Pretpostavljam da imaš enum Dostupnost.Dostupan
            _context.Users.Update(currentDriver);

            await _context.SaveChangesAsync();

            // Opcionalno: Pošalji SignalR notifikaciju putniku da je vožnja završena
            if (voznja.Putnik != null)
            {
                await _hubContext.Clients.User(voznja.Putnik.Id).SendAsync(
                    "VoznjaZavrsena", // Naziv metode na klijentskoj strani putnika
                    new
                    {
                        RideId = voznja.ID,
                        VozacUserName = currentDriver.UserName,
                        Poruka = "Vaša vožnja je uspješno završena!"
                    }
                );
            }

            return Ok(new { message = "Vožnja uspješno završena, status vozača promijenjen u 'Dostupan'." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrihvatiVoznju([FromBody] int voznjaId)
        {
            var voznja = await _context.Voznja
                .Include(v => v.Putnik)
                .FirstOrDefaultAsync(v => v.ID == voznjaId);

            if (voznja == null)
                return NotFound(new { message = "Vožnja nije pronađena." });

            var currentDriver = await _userManager.GetUserAsync(User);

            if (voznja.status != Status.naCekanju)
                return BadRequest(new { message = "Vožnja nije dostupna za prihvatanje." });

            // Postavljanje vozača
            voznja.VozacId = currentDriver.Id;
            voznja.status = Status.Prihvacena;
            currentDriver.Dostupnost = Dostupnost.Zauzet;

            _context.Voznja.Update(voznja);
            _context.Users.Update(currentDriver);
            await _context.SaveChangesAsync();

            // Notifikacija putniku
            if (voznja.Putnik != null)
            {
                await _hubContext.Clients.User(voznja.Putnik.Id).SendAsync(
                    "VoznjaPrihvacena",
                    new
                    {
                        RideId = voznja.ID,
                        VozacUserName = currentDriver.UserName,
                        Poruka = "Vaša vožnja je prihvaćena i vozač dolazi po vas!"
                    });
            }

            return Ok(new { message = "Vožnja prihvaćena i vozač postavljen kao zauzet." });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OdbijVoznju([FromBody] int voznjaId)
        {
            var voznja = await _context.Voznja
                .Include(v => v.Putnik)
                .FirstOrDefaultAsync(v => v.ID == voznjaId);

            if (voznja == null)
            {
                return NotFound(new { message = "Vožnja nije pronađena." });
            }

            var currentDriver = await _userManager.GetUserAsync(User);
            if (voznja.VozacId != currentDriver.Id)
            {
                return Forbid(); // Neovlašten pristup
            }

            if (voznja.status != Status.naCekanju)
            {
                return BadRequest(new { message = "Vožnja se može odbiti samo dok je u statusu 'Na čekanju'." });
            }

            // Odbijanje vožnje – možeš ili obrisati vozača ili samo promijeniti status
            voznja.VozacId = null;
            voznja.status = Status.Odbijena;

            _context.Voznja.Update(voznja);
            await _context.SaveChangesAsync();

            // Notifikacija putniku
            if (voznja.Putnik != null)
            {
                await _hubContext.Clients.User(voznja.Putnik.Id).SendAsync(
                    "VoznjaOdbijena",
                    new
                    {
                        RideId = voznja.ID,
                        Poruka = "Vozač je odbio vašu vožnju."
                    }
                );
            }

            return Ok(new { message = "Vožnja je odbijena i putnik je obaviješten." });
        }


    }
}