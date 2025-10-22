using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Motorcycle> Motorcycles => Set<Motorcycle>();
    public DbSet<Courier> Couriers => Set<Courier>();
    public DbSet<Rental> Rentals => Set<Rental>();
    public DbSet<MotorcycleEventLog> MotorcycleEventLogs => Set<MotorcycleEventLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
