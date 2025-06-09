using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace YourRide.Models
{
    public class Korisnik:IdentityUser
    {
       
       public Dostupnost? Dostupnost { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public Korisnik() { }



    }
}
