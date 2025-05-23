using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourRide.Models
{
    public class Ocjena
    {
        [Key] 
        public int ID{ get; set; }
        public int ocjena { get; set; }
        public string komentar { get; set; }

        [ForeignKey("Korisnik")]
        public int KorisnikId { get; set; }

        public Korisnik Korisnik { get; set; }
        public Ocjena() { }

        
    }
}
