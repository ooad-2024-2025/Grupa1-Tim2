using System.ComponentModel.DataAnnotations;

namespace YourRide.Models
{
    public enum VrstaRacuna
    {
        Putnik,
        [Display(Name = "Vozač")]
        Vozac,
        Administrator,
        [Display(Name = "Tehnička podrška")]
        TehnickaPodrska,
    }
}
