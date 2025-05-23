using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace YourRide.Models
{
    public class Notifikacija
    {
        [Key]
        public int ID { get; set; }

        public String poruka { get; set; }

        public DateTime VrijemeSlanja { get; set; }
        bool procitano { get; set; } = false;

        [ForeignKey("Posiljalac")]
        public int PosiljalacId { get; set; }
        public Korisnik Posiljalac { get; set; }

        [ForeignKey("Primalac")]
        public int PrimalacId { get; set; }
        public Korisnik Primalac { get; set; }

        public Notifikacija() { }
    }
}
