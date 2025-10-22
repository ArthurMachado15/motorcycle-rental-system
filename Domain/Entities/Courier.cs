using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Courier : BaseEntity
{
    public string Identifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CNPJ { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string DriverLicenseNumber { get; set; } = string.Empty;
    public DriverLicenseType DriverLicenseType { get; set; }
    public string? DriverLicenseImagePath { get; set; }
    
    // Navigation properties
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
