using System.ComponentModel.DataAnnotations;

namespace YourRide.Models
{
    public class PodrskaPorukaDto
    {
        [Required(ErrorMessage = "Naslov je obavezan.")]
        public string Naslov { get; set; }

        [Required(ErrorMessage = "Poruka je obavezna.")]
        public string Poruka { get; set; }
    }
}
