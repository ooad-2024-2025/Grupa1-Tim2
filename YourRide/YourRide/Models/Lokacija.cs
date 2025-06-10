using System.ComponentModel.DataAnnotations;
using System.Runtime;

namespace YourRide.Models
{
    public class Lokacija
    {
        [Key]
        public int ID { get; set; }
        public String Grad { get; set; }
        public String Naziv { get; set; }

        public double? Latituda {get;set;}
    public double? Longituda {get;set;}


public Lokacija() { }
    }
}
