using Aro.Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Booking.Infrastructure.Repository.EntityConfigurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("Groups");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.GroupName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.AddressLine1)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.AddressLine2)
            .HasMaxLength(256);

        builder.Property(u => u.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PostalCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.Logo)
            .HasColumnType("varbinary(max)");

        builder.HasOne(g => g.PrimaryContact)
            .WithMany()
            .HasForeignKey(g => g.PrimaryContactId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
