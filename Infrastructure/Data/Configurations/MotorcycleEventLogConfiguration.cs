using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class MotorcycleEventLogConfiguration : IEntityTypeConfiguration<MotorcycleEventLog>
{
    public void Configure(EntityTypeBuilder<MotorcycleEventLog> builder)
    {
        builder.ToTable("MotorcycleEventLogs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.MotorcycleId)
            .IsRequired();

        builder.Property(e => e.Year)
            .IsRequired();

        builder.Property(e => e.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.LicensePlate)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.RegisteredAt)
            .IsRequired();
    }
}
