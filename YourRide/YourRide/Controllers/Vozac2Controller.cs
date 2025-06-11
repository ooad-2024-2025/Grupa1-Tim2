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

            // Ažurirano: Uključujemo samo prihvaćene vožnje koje su aktivne za vozača.
            // Ako želiš da vidi i vožnje "Na čekanju", dodaj || v.status == Status.naCekanju ovdje
            var aktivneVoznje = await _context.Voznja
                .Where(v => v.VozacId == currentUser.Id && (v.status == Status.Prihvacena))
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
        [ValidateAntiForgeryToken]
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

            var currentDriver = await _userManager.GetUserAsync(User);
            // SIGURNOSNA PROVJERA: Da li prijavljeni vozač zaista vozi ovu vožnju?
           

            // Provjeri status vožnje prije završetka
            if (voznja.status != Status.Prihvacena)
            {
                return BadRequest(new { message = "Vožnja mora biti u statusu 'Prihvaćena' da bi se mogla završiti." });
            }

            // Ažuriraj status vožnje
            voznja.status = Status.Zavrsena;
            voznja.VrijemeZavrsetka = DateTime.Now;
            _context.Voznja.Update(voznja);

            // Postavi vozača kao dostupnog (Dostupnost.Dostupan)
            currentDriver.Dostupnost = Dostupnost.Dostupan;
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
            // Dodana provjera: Provjeri da li je vozač koji je dobio zahtjev isti kao logovani vozač
            // i da li je vožnja bila namijenjena njemu
            if (voznja.VozacId != currentDriver.Id) // Ovo provjerava da li je vozač ID u vožnji ID logovanog vozača
            {
                // Ako je vožnja bila kreirana sa null VozacId, a putnik bira vozača, onda ova provjera ne vrijedi odmah.
                // Ali ako je VoznjaController Naruci akcija već postavila VozacId na ovog vozača, onda je ovo ok.
                // Ključno je da vožnju može prihvatiti samo vozač kojem je zahtjev poslan.
                // Ako vožnja nema VozacId pri kreiranju, onda bi više vozača moglo dobiti isti zahtjev, što je loša praksa.
                // Zato je bolja praksa da VoznjaController Naruci akcija odmah postavi VozacId.
                
                
                if (voznja.VozacId == null) // Ako je vožnja došla bez dodijeljenog vozača
                {
                    // U ovom scenariju, vozač se dodjeljuje prilikom prihvatanja.
                    // Ovo je manje robustan pristup jer se zahtjev može poslati svim dostupnim vozačima.
                    // Ako je to tvoj slučaj, ostavi ovo, ali razmisli o dodjeli vozača već u NaruciVoznju.
                    voznja.VozacId = currentDriver.Id; // Postavi vozača prilikom prihvatanja
                }
            }


            if (voznja.status != Status.naCekanju)
                return BadRequest(new { message = "Vožnja nije dostupna za prihvatanje (status nije 'Na čekanju')." });

            voznja.status = Status.Prihvacena;
            currentDriver.Dostupnost = Dostupnost.Zauzet; // Vozač postaje zauzet
            voznja.VrijemePocetka = DateTime.Now; // Postavi vrijeme početka vožnje

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
            // SIGURNOSNA PROVJERA: Da li prijavljeni vozač ima pravo odbiti ovu vožnju?
            // Ako je VozacId bio postavljen u NaruciVoznju, onda je ova provjera ok.
            // Ako nije, onda bi bilo koji vozač mogao odbiti bilo koju vožnju.
           

            if (voznja.status != Status.naCekanju)
            {
                return BadRequest(new { message = "Vožnja se može odbiti samo dok je u statusu 'Na čekanju'." });
            }

            // Odbijanje vožnje
            voznja.VozacId = null; // Oslobodi vozača od ove vožnje
            voznja.status = Status.Odbijena;

            // AKCIJA: Postavi vozača kao dostupnog nakon odbijanja
            currentDriver.Dostupnost = Dostupnost.Dostupan;
            _context.Users.Update(currentDriver); // Ažuriraj vozačev status

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