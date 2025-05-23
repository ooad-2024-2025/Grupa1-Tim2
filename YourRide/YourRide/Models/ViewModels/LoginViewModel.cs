using System.ComponentModel.DataAnnotations;

namespace YourRide.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Korisničko ime je obavezno")]
        [Display(Name = "Korisničko ime")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Lozinka je obavezna")]
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        public string Password { get; set; }
    }
}
