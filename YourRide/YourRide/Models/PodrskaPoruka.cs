using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourRide.Models
{
    public class PodrskaPoruka


    {

        public int ID { get; set; }

        [Required]
        public string Naslov { get; set; }

        [Required]
        public string Poruka { get; set; }

        public DateTime DatumSlanja { get; set; } = DateTime.Now;

        public string KorisnikId { get; set; }
        [ForeignKey("KorisnikId")]
        public Korisnik Korisnik { get; set; }
    }
}
