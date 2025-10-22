using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class MotorcycleEventLogRepository : IMotorcycleEventLogRepository
{
    private readonly ApplicationDbContext _context;

    public MotorcycleEventLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(MotorcycleEventLog eventLog, CancellationToken cancellationToken = default)
    {
        await _context.MotorcycleEventLogs.AddAsync(eventLog, cancellationToken);
    }
}
