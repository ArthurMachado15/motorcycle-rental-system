using Domain.Entities;

namespace Domain.Interfaces;

public interface IMotorcycleEventLogRepository
{
    Task AddAsync(MotorcycleEventLog eventLog, CancellationToken cancellationToken = default);
}
