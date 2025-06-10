using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourRide.Models
{
    public class Ruta
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("PocetnaLokacija")]
        public int PocetnaLokacijaId { get; set; }
        public Lokacija PocetnaLokacija { get; set; }

        [ForeignKey("KrajnjaLokacija")]
        public int? KrajnjaLokacijaId { get; set; }
        public Lokacija? KrajnjaLokacija { get; set; }
        public Ruta()
        { }
    }
}
