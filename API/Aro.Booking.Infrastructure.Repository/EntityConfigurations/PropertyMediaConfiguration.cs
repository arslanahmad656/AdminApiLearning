using Aro.Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Booking.Infrastructure.Repository.EntityConfigurations;

public class PropertyMediaConfiguration : IEntityTypeConfiguration<PropertyMedia>
{
    public void Configure(EntityTypeBuilder<PropertyMedia> builder)
    {
        builder.ToTable("PropertyMediaFiles");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.PropertyId)
            .IsRequired();

        builder.Property(m => m.Url)
            .IsRequired()
            .HasMaxLength(1024)
            .HasComment("URL or path to the media file");

        builder.Property(m => m.MediaType)
            .IsRequired()
            .HasConversion<int>()
            .HasComment("Type of media: Banner, Thumbnail, Gallery, etc.");

        builder.Property(m => m.DisplayOrder)
            .IsRequired()
            .HasComment("Display order for sorting images");

        builder.Property(m => m.OriginalFileName)
            .IsRequired(false)
            .HasMaxLength(256);

        builder.Property(m => m.FileSizeBytes)
            .IsRequired(false)
            .HasComment("File size in bytes (max 5MB = 5,242,880 bytes)");

        builder.Property(m => m.ContentType)
            .IsRequired(false)
            .HasMaxLength(100)
            .HasComment("MIME type (e.g., image/png, image/jpeg, image/svg+xml)");

        builder.Property(m => m.UploadedAt)
            .IsRequired();

        // Indexes for efficient querying
        builder.HasIndex(m => m.PropertyId);

        builder.HasIndex(m => new { m.PropertyId, m.MediaType, m.DisplayOrder });

        // Relationship configured in PropertyConfiguration
        builder.HasOne(m => m.Property)
            .WithMany(p => p.MediaFiles)
            .HasForeignKey(m => m.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
