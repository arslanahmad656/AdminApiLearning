using Aro.Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Common.Infrastructure.Repository.EntityConfigurations;

public class PropertyFilesConfiguration : IEntityTypeConfiguration<PropertyFiles>
{
    public void Configure(EntityTypeBuilder<PropertyFiles> builder)
    {
        builder.ToTable("PropertyFiles");

        builder.HasKey(pf => pf.Id);

        builder.HasIndex(pf => new { pf.PropertyId, pf.FileId })
               .IsUnique();

        builder.Property(pf => pf.PropertyId)
            .IsRequired();

        builder.Property(pf => pf.FileId)
            .IsRequired();

        builder.HasOne<FileResource>()
           .WithMany()
           .HasForeignKey(pf => pf.FileId)
           .OnDelete(DeleteBehavior.Restrict);
    }
}
