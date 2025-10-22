using Domain.Entities;

namespace Domain.Interfaces;

public interface ICourierRepository
{
    Task<Courier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Courier?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
    Task<Courier?> GetByCNPJAsync(string cnpj, CancellationToken cancellationToken = default);
    Task<Courier?> GetByDriverLicenseNumberAsync(string driverLicenseNumber, CancellationToken cancellationToken = default);
    Task<Courier> AddAsync(Courier courier, CancellationToken cancellationToken = default);
    Task UpdateAsync(Courier courier, CancellationToken cancellationToken = default);
}
