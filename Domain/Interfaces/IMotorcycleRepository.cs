using Domain.Entities;

namespace Domain.Interfaces;

public interface IMotorcycleRepository
{
    Task<Motorcycle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Motorcycle?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
    Task<Motorcycle?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Motorcycle>> GetAllAsync(string? licensePlateFilter = null, CancellationToken cancellationToken = default);
    Task<Motorcycle> AddAsync(Motorcycle motorcycle, CancellationToken cancellationToken = default);
    Task UpdateAsync(Motorcycle motorcycle, CancellationToken cancellationToken = default);
    Task DeleteAsync(Motorcycle motorcycle, CancellationToken cancellationToken = default);
    Task<bool> HasRentalHistoryAsync(Guid motorcycleId, CancellationToken cancellationToken = default);
}
