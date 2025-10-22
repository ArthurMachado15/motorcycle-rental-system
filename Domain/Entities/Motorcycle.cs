using Domain.Common;

namespace Domain.Entities;

public class Motorcycle : BaseEntity
{
    public string Identifier { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Model { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
