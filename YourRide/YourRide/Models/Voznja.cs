using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourRide.Models
{
    public class Voznja
    {
        [Key]
        public int ID { get; set; }

        [Display(Name ="Status vožnje")]
        public Status status { get; set; }

        [Display(Name = "Putnik")]

        [ForeignKey("Putnik")]
        public string PutnikId { get; set; }
        public Korisnik Putnik { get; set; }

        [Display(Name = "Vozač")]

        [ForeignKey("Vozac")]
        public string VozacId { get; set; }
        public Korisnik Vozac { get; set; }

        [Display(Name = "RutaID")]

        [ForeignKey("Ruta")]
        public int RutaId { get; set; }
        public Ruta Ruta { get; set; }

        [Display(Name = "Vrijeme početka")]
        public DateTime VrijemePocetka { get; set; }

        [Display(Name = "Vrijeme završetka")]
        public DateTime? VrijemeZavrsetka { get; set; }

       
        public Voznja() { }
    }
}
