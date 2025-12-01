using Aro.Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Booking.Infrastructure.Repository.EntityConfigurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses");

        builder.HasKey(u => u.Id);

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
    }
}
