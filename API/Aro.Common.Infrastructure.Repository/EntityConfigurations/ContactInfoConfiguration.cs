using Aro.Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Common.Infrastructure.Repository.EntityConfigurations;

public class ContactInfoConfiguration : IEntityTypeConfiguration<ContactInfo>
{
    public void Configure(EntityTypeBuilder<ContactInfo> builder)
    {
        builder.ToTable("ContactInfos");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.CountryCode)
        .IsRequired()
        .HasMaxLength(5);

        builder.Property(u => u.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);
    }
}
