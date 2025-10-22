using Domain.Common;

namespace Domain.Entities;

public class MotorcycleEventLog : BaseEntity
{
    public Guid MotorcycleId { get; set; }
    public int Year { get; set; }
    public string Model { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
}
