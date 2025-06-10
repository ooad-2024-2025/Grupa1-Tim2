namespace YourRide.Models
{
    public class RideRequestDto
    {
        public string DriverId { get; set; }

        public string PutnikId { get; set; }
        public string PocetnaAdresa { get; set; }

        public string? OdredisnaAdresa { get; set; }
    }
}
