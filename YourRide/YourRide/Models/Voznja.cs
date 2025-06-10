using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourRide.Models
{
    public class Voznja
    {
        [Key]
        public int ID { get; set; }

        public Status status { get; set; }

        [ForeignKey("Putnik")]
        public string PutnikId { get; set; }
        public Korisnik Putnik { get; set; }

        [ForeignKey("Vozac")]
        public string VozacId { get; set; }
        public Korisnik Vozac { get; set; }

        [ForeignKey("Ruta")]
        public int RutaId { get; set; }
        public Ruta Ruta { get; set; }

        public DateTime VrijemePocetka { get; set; }
        public DateTime? VrijemeZavrsetka { get; set; }

       
        public Voznja() { }
    }
}
