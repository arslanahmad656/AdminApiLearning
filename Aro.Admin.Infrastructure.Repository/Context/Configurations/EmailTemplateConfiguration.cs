using Aro.Admin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Admin.Infrastructure.Repository.Context.Configurations;

public class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplate>
{
    public void Configure(EntityTypeBuilder<EmailTemplate> builder)
    {
        builder.ToTable("EmailTemplates");

        builder.HasKey(et => et.Id);

        builder.Property(et => et.Identifier)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(et => et.Identifier)
            .IsUnique();

        builder.Property(et => et.Subject)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(et => et.Body)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(et => et.IsHTML)
            .IsRequired()
            .HasMaxLength(10);
    }
}
