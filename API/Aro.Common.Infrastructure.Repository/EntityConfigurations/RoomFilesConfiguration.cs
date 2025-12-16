using Aro.Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Common.Infrastructure.Repository.EntityConfigurations;

public class RoomFilesConfiguration : IEntityTypeConfiguration<RoomFiles>
{
    public void Configure(EntityTypeBuilder<RoomFiles> builder)
    {
        builder.ToTable("RoomFiles");

        builder.HasKey(rf => rf.Id);

        builder.HasIndex(rf => new { rf.RoomId, rf.FileId })
               .IsUnique();

        builder.Property(rf => rf.RoomId)
            .IsRequired();

        builder.Property(rf => rf.FileId)
            .IsRequired();

        builder.Property(rf => rf.OrderIndex)
            .IsRequired();

        builder.Property(rf => rf.IsThumbnail)
            .IsRequired();

        builder.HasOne<FileResource>()
           .WithMany()
           .HasForeignKey(rf => rf.FileId)
           .OnDelete(DeleteBehavior.Restrict);
    }
}
