using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourRide.Models
{
    public class Ocjena
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int ocjena { get; set; }

      
        public string komentar { get; set; }

        [Required]
        [ForeignKey("Korisnik")]
        public string KorisnikId { get; set; }

        public Korisnik? Korisnik { get; set; } // Navigacija – nije obavezna za API
    }
}
