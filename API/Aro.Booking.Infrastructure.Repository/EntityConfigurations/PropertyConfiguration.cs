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

        #region Step 1: Property Information
        builder.Property(p => p.GroupId)
            .IsRequired(false); // Nullable - can be set later

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
            .HasComment("ISO 4217 currency code (e.g., USD, EUR, DKK)");

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(2000);
        #endregion

        #region Step 2: Address & Primary Contact
        builder.Property(p => p.AddressLine1)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(p => p.AddressLine2)
            .IsRequired(false)
            .HasMaxLength(256);

        builder.Property(p => p.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.PostalCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.PhoneNumber)
            .IsRequired()
            .HasMaxLength(30)
            .HasComment("Phone number with country code");

        builder.Property(p => p.WebsiteUrl)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(p => p.PrimaryContactName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(p => p.PrimaryContactEmail)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(p => p.PrimaryContactRole)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("Property Manager");
        #endregion

        #region Step 3: Key Selling Points (Owned Entities)
        builder.HasMany(p => p.SellingPoints)
            .WithOne(sp => sp.Property)
            .HasForeignKey(sp => sp.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion

        #region Step 4: Media Files (Owned Entities)
        builder.HasMany(p => p.MediaFiles)
            .WithOne(m => m.Property)
            .HasForeignKey(m => m.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.LogoUrl)
            .IsRequired(false)
            .HasMaxLength(512);

        builder.Property(p => p.FaviconUrl)
            .IsRequired(false)
            .HasMaxLength(512);
        #endregion

        #region Step 5: Marketing & SEO
        builder.Property(p => p.MetaTitle)
            .IsRequired(false)
            .HasMaxLength(60)
            .HasComment("H1 heading / Meta title for SEO");

        builder.Property(p => p.MetaDescription)
            .IsRequired(false)
            .HasMaxLength(320)
            .HasComment("Meta description for SEO");
        #endregion

        #region Workflow Management
        builder.Property(p => p.IsDraft)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Indicates if this is a draft or published property");

        builder.Property(p => p.CurrentStep)
            .IsRequired()
            .HasDefaultValue(0)
            .HasComment("Tracks which wizard step was last completed (0-6)");
        #endregion

        #region Audit & Status
        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired(false);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
        #endregion

        #region Indexes
        builder.HasIndex(p => p.GroupId);

        builder.HasIndex(p => p.PropertyName);

        builder.HasIndex(p => p.IsDraft);

        builder.HasIndex(p => p.IsActive);

        builder.HasIndex(p => new { p.PropertyName, p.GroupId })
            .HasFilter("[GroupId] IS NOT NULL")
            .IsUnique(false); // Multiple properties can have same name in different groups
        #endregion

        #region Relationships
        builder.HasOne(p => p.Group)
            .WithMany()
            .HasForeignKey(p => p.GroupId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        #endregion
    }
}
