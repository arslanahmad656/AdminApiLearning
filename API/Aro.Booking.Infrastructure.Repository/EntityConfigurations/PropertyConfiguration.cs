using Aro.Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Booking.Infrastructure.Repository.EntityConfigurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Properties");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.GroupId)
            .IsRequired();

        builder.Property(p => p.PropertyName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(p => p.PropertyTypes)
            .IsRequired();

        builder.Property(p => p.StarRating)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasComment("ISO 4217 currency code (e.g., USD, EUR, GBP)");

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired(false);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(p => p.GroupId);

        builder.HasIndex(p => new { p.PropertyName, p.GroupId })
            .IsUnique();

        builder.HasOne(p => p.Group)
            .WithMany()
            .HasForeignKey(p => p.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Contact)
            .WithMany()
            .HasForeignKey(p => p.ContactId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Address)
            .WithMany()
            .HasForeignKey(p => p.AddressId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
