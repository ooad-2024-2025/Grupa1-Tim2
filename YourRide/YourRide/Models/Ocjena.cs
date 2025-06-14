using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourRide.Models
{
    public class Ocjena
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name ="Ocjena")]
        public int ocjena { get; set; }

        [Display(Name = "Komentar")]
        public string komentar { get; set; }

        [Required]
        [Display(Name = "Vozač")]
        [ForeignKey("Korisnik")]
        public string KorisnikId { get; set; }

        public Korisnik? Korisnik { get; set; } // Navigacija – nije obavezna za API
    }
}
