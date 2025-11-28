using Aro.Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Common.Infrastructure.Repository.EntityConfigurations;

public class FileResourceConfiguration : IEntityTypeConfiguration<FileResource>
{
    public void Configure(EntityTypeBuilder<FileResource> builder)
    {
        builder.ToTable("FileResources");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.Uri)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.Metadata)
            .HasColumnType("nvarchar(max)");
    }
}