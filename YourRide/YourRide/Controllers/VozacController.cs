using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourRide.Data;
using YourRide.Models;


public class VozacController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Korisnik> _userManager;


    public VozacController(ApplicationDbContext context, UserManager<Korisnik> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> PrikazSlobodnihVozaca(string location)
    {
        
        if (string.IsNullOrWhiteSpace(location))
        {
            return BadRequest("Lokacija nije unijeta.");
        }

      
        var vozaci = await _context.Users
            .Where(u => u.Dostupnost == Dostupnost.Dostupan &&
                        u.Dostupnost != null && 
                        u.Latitude.HasValue &&
                        u.Longitude.HasValue)
            .ToListAsync();

        
        List<object> vozaciLista = new List<object>();

        foreach (var vozac in vozaci)
        {
          

           
            vozaciLista.Add(new
            {

                Name = vozac.UserName,
                Dostupnost = vozac.Dostupnost,
                Latitude = vozac.Latitude,
                Longitude = vozac.Longitude
            });
        }

       
        return Json(vozaciLista);
    }
}
