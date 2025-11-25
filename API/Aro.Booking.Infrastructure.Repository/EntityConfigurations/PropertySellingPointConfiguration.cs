using Aro.Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Booking.Infrastructure.Repository.EntityConfigurations;

public class PropertySellingPointConfiguration : IEntityTypeConfiguration<PropertySellingPoint>
{
    public void Configure(EntityTypeBuilder<PropertySellingPoint> builder)
    {
        builder.ToTable("PropertySellingPoints");

        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.PropertyId)
            .IsRequired();

        builder.Property(sp => sp.Text)
            .IsRequired()
            .HasMaxLength(30)
            .HasComment("Selling point text (max 30 characters)");

        builder.Property(sp => sp.DisplayOrder)
            .IsRequired()
            .HasComment("Display order (0-3 for up to 4 points)");

        // Index for efficient querying by property and ordering
        builder.HasIndex(sp => new { sp.PropertyId, sp.DisplayOrder });

        // Relationship configured in PropertyConfiguration
        builder.HasOne(sp => sp.Property)
            .WithMany(p => p.SellingPoints)
            .HasForeignKey(sp => sp.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
