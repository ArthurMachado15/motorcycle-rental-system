using Domain.Entities;

namespace Domain.Interfaces;

public interface IRentalRepository
{
    Task<Rental?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Rental?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
    Task<Rental> AddAsync(Rental rental, CancellationToken cancellationToken = default);
    Task UpdateAsync(Rental rental, CancellationToken cancellationToken = default);
    Task<bool> HasActiveCourierRentalAsync(Guid courierId, CancellationToken cancellationToken = default);
}
