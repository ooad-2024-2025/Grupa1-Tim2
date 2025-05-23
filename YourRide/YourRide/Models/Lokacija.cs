using System.ComponentModel.DataAnnotations;

namespace YourRide.Models
{
    public class Lokacija
    {
        [Key]
        public int ID { get; set; }
        public String Grad { get; set; }
        public String Naziv { get; set; }
        public double Udaljenost { get; set; }



        public Lokacija() { }
    }
}
