using System.ComponentModel.DataAnnotations;

namespace YourRide.Models
{
    public class Korisnik
    {
        [Key]
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public VrstaRacuna VrstaRacuna { get; set; }
        public Dostupnost? Dostupnost { get; set; }
        public Korisnik() { }



    }
}
