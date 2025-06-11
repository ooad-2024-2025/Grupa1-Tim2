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

            // Uključujemo samo prihvaćene vožnje koje su aktivne za vozača.
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
           
            if (voznja.VozacId != currentDriver.Id)
            {
                return Unauthorized(new { message = "Nemate ovlaštenje za završetak ove vožnje." });
            }

            if (voznja.status != Status.Prihvacena)
            {
                return BadRequest(new { message = "Vožnja mora biti u statusu 'Prihvaćena' da bi se mogla završiti." });
            }

            // Ažuriraj status vožnje
            voznja.status = Status.Zavrsena;
            voznja.VrijemeZavrsetka = DateTime.Now;
            _context.Voznja.Update(voznja);

           
            currentDriver.Dostupnost = Dostupnost.Dostupan;
            _context.Users.Update(currentDriver);

            await _context.SaveChangesAsync();

            
            if (voznja.Putnik != null)
            {
                await _hubContext.Clients.User(voznja.Putnik.Id).SendAsync(
                    "VoznjaZavrsena", // Naziv metode na klijentskoj strani putnika
                    new
                    {
                        rideId = voznja.ID,
                        vozacUserName = currentDriver.UserName,
                        poruka = "Vaša vožnja je uspješno završena!"
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
            {
                return NotFound(new { message = "Vožnja nije pronađena." });
            }

            var currentDriver = await _userManager.GetUserAsync(User);
            if (currentDriver == null)
            {
                return Unauthorized(new { message = "Vozač nije prijavljen." });
            }

            
            if (voznja.VozacId != null && voznja.VozacId != currentDriver.Id)
            {
                
                return BadRequest(new { message = "Ova vožnja je već dodijeljena drugom vozaču ili je u međuvremenu prihvaćena." });
            }
            else if (voznja.VozacId == null)
            {
                
                voznja.VozacId = currentDriver.Id;
            }
            

            if (voznja.status != Status.naCekanju)
            {
                return BadRequest(new { message = "Vožnja nije dostupna za prihvatanje (status nije 'Na čekanju')." });
            }

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
                        rideId = voznja.ID,
                        vozacUserName = currentDriver.UserName,
                        poruka = "Vaša vožnja je prihvaćena i vozač dolazi po vas!"
                    });
            }

            return Ok(new { message = "Vožnja prihvaćena i vozač postavljen kao zauzet." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OdbijVoznju([FromBody] int voznjaId)
        {
            try 
            {
                var voznja = await _context.Voznja
                    .Include(v => v.Putnik)
                    .FirstOrDefaultAsync(v => v.ID == voznjaId);

                if (voznja == null)
                {
                    return NotFound(new { message = "Vožnja nije pronađena." });
                }

                var currentDriver = await _userManager.GetUserAsync(User);
                if (currentDriver == null)
                {
                    return Unauthorized(new { message = "Vozač nije prijavljen." });
                }

                if (voznja.VozacId != null && voznja.VozacId != currentDriver.Id)
                {
                    return Unauthorized(new { message = "Nemate ovlaštenje za odbijanje ove vožnje." });
                }

                
                if (voznja.status != Status.naCekanju)
                {
                    return BadRequest(new { message = "Vožnja se može odbiti samo dok je u statusu 'Na čekanju'." });
                }

                voznja.VozacId = null;
                voznja.status = Status.Odbijena;

                currentDriver.Dostupnost = Dostupnost.Dostupan;

                _context.Voznja.Update(voznja);
                _context.Users.Update(currentDriver);
                await _context.SaveChangesAsync(); // <-- Posebno pažljivo prati ovu liniju

                if (voznja.Putnik != null)
                {
                    await _hubContext.Clients.User(voznja.Putnik.Id).SendAsync(
                        "VoznjaOdbijena",
                        new
                        {
                            rideId = voznja.ID,
                            poruka = "Vozač je odbio vašu vožnju."
                        }
                    );
                }

                return Ok(new { message = "Vožnja je odbijena i putnik je obaviješten." });
            }
            catch (Exception ex) 
            {
          
                Console.Error.WriteLine($"Greška prilikom odbijanja vožnje: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.Error.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

               
                return StatusCode(500, new { message = $"Interna greška servera prilikom odbijanja vožnje: {ex.Message}" });
            }
        }
    }
}