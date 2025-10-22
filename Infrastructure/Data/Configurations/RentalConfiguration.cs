using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class RentalConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        builder.ToTable("Rentals");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Identifier)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(r => r.Identifier)
            .IsUnique();

        builder.Property(r => r.CourierId)
            .IsRequired();

        builder.Property(r => r.MotorcycleId)
            .IsRequired();

        builder.Property(r => r.PlanType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(r => r.StartDate)
            .IsRequired();

        builder.Property(r => r.ExpectedEndDate)
            .IsRequired();

        builder.Property(r => r.ActualEndDate);

        builder.Property(r => r.DailyCost)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(r => r.TotalCost)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.HasOne(r => r.Courier)
            .WithMany(c => c.Rentals)
            .HasForeignKey(r => r.CourierId);

        builder.HasOne(r => r.Motorcycle)
            .WithMany(m => m.Rentals)
            .HasForeignKey(r => r.MotorcycleId);
    }
}
