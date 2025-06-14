using System.ComponentModel.DataAnnotations;

namespace YourRide.Models.ViewModels
{
    public class RegisterViewModel
    {
        
        [Display(Name = "Korisničko ime")]
        public string UserName { get; set; }

       
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-mail adresa")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Vrsta računa")]
        public VrstaRacuna VrstaRacuna { get; set; }
    }
}
