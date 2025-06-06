using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace YourRide.Models
{
    public class Korisnik:IdentityUser
    {
       
       public Dostupnost? Dostupnost { get; set; }

        public Korisnik() { }



    }
}
