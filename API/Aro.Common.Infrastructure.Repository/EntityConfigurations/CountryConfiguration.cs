using Aro.Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Common.Infrastructure.Repository.EntityConfigurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Countries");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.OfficialName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.ISO2)
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(b => b.PostalCodeRegex)
            .IsRequired()
            .HasMaxLength(-1);

        builder.Property(b => b.PhoneCountryCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(b => b.PhoneNumberRegex)
            .IsRequired()
            .HasMaxLength(-1);
    }
}
