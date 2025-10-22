using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CourierRepository : ICourierRepository
{
    private readonly ApplicationDbContext _context;

    public CourierRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Courier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Couriers.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Courier?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return await _context.Couriers
            .FirstOrDefaultAsync(c => c.Identifier == identifier, cancellationToken);
    }

    public async Task<Courier?> GetByCNPJAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        return await _context.Couriers
            .FirstOrDefaultAsync(c => c.CNPJ == cnpj, cancellationToken);
    }

    public async Task<Courier?> GetByDriverLicenseNumberAsync(string driverLicenseNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Couriers
            .FirstOrDefaultAsync(c => c.DriverLicenseNumber == driverLicenseNumber, cancellationToken);
    }

    public async Task<Courier> AddAsync(Courier courier, CancellationToken cancellationToken = default)
    {
        await _context.Couriers.AddAsync(courier, cancellationToken);
        return courier;
    }

    public Task UpdateAsync(Courier courier, CancellationToken cancellationToken = default)
    {
        _context.Couriers.Update(courier);
        return Task.CompletedTask;
    }
}
