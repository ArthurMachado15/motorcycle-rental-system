using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class CourierConfiguration : IEntityTypeConfiguration<Courier>
{
    public void Configure(EntityTypeBuilder<Courier> builder)
    {
        builder.ToTable("Couriers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Identifier)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(c => c.Identifier)
            .IsUnique();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.CNPJ)
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(c => c.BirthDate)
            .IsRequired();

        builder.Property(c => c.DriverLicenseNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.DriverLicenseType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(c => c.DriverLicenseImagePath)
            .HasMaxLength(500);

        builder.HasIndex(c => c.CNPJ)
            .IsUnique();

        builder.HasIndex(c => c.DriverLicenseNumber)
            .IsUnique();

        builder.HasMany(c => c.Rentals)
            .WithOne(r => r.Courier)
            .HasForeignKey(r => r.CourierId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
