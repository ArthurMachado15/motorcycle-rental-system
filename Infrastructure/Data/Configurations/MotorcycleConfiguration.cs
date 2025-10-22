using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class MotorcycleConfiguration : IEntityTypeConfiguration<Motorcycle>
{
    public void Configure(EntityTypeBuilder<Motorcycle> builder)
    {
        builder.ToTable("Motorcycles");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Identifier)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(m => m.Identifier)
            .IsUnique();

        builder.Property(m => m.Year)
            .IsRequired();

        builder.Property(m => m.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.LicensePlate)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(m => m.LicensePlate)
            .IsUnique();

        builder.HasMany(m => m.Rentals)
            .WithOne(r => r.Motorcycle)
            .HasForeignKey(r => r.MotorcycleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
