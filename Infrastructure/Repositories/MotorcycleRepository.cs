using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MotorcycleRepository : IMotorcycleRepository
{
    private readonly ApplicationDbContext _context;

    public MotorcycleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Motorcycle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Motorcycles.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Motorcycle?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return await _context.Motorcycles
            .FirstOrDefaultAsync(m => m.Identifier == identifier, cancellationToken);
    }

    public async Task<Motorcycle?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default)
    {
        return await _context.Motorcycles
            .FirstOrDefaultAsync(m => m.LicensePlate == licensePlate, cancellationToken);
    }

    public async Task<IEnumerable<Motorcycle>> GetAllAsync(string? licensePlateFilter = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Motorcycles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(licensePlateFilter))
        {
            query = query.Where(m => m.LicensePlate.Contains(licensePlateFilter));
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Motorcycle> AddAsync(Motorcycle motorcycle, CancellationToken cancellationToken = default)
    {
        await _context.Motorcycles.AddAsync(motorcycle, cancellationToken);
        return motorcycle;
    }

    public Task UpdateAsync(Motorcycle motorcycle, CancellationToken cancellationToken = default)
    {
        _context.Motorcycles.Update(motorcycle);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Motorcycle motorcycle, CancellationToken cancellationToken = default)
    {
        _context.Motorcycles.Remove(motorcycle);
        return Task.CompletedTask;
    }

    public async Task<bool> HasRentalHistoryAsync(Guid motorcycleId, CancellationToken cancellationToken = default)
    {
        return await _context.Rentals.AnyAsync(r => r.MotorcycleId == motorcycleId, cancellationToken);
    }
}
