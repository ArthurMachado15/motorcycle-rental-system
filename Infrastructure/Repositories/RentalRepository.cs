using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RentalRepository : IRentalRepository
{
    private readonly ApplicationDbContext _context;

    public RentalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Rental?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Rentals
            .Include(r => r.Courier)
            .Include(r => r.Motorcycle)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Rental?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return await _context.Rentals
            .Include(r => r.Courier)
            .Include(r => r.Motorcycle)
            .FirstOrDefaultAsync(r => r.Identifier == identifier, cancellationToken);
    }

    public async Task<Rental> AddAsync(Rental rental, CancellationToken cancellationToken = default)
    {
        await _context.Rentals.AddAsync(rental, cancellationToken);
        return rental;
    }

    public Task UpdateAsync(Rental rental, CancellationToken cancellationToken = default)
    {
        _context.Rentals.Update(rental);
        return Task.CompletedTask;
    }

    public async Task<bool> HasActiveCourierRentalAsync(Guid courierId, CancellationToken cancellationToken = default)
    {
        return await _context.Rentals
            .AnyAsync(r => r.CourierId == courierId && r.ActualEndDate == null, cancellationToken);
    }
}
