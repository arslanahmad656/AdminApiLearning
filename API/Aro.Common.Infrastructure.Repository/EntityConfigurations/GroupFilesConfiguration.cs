using Aro.Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aro.Common.Infrastructure.Repository.EntityConfigurations;

public class GroupFilesConfiguration : IEntityTypeConfiguration<GroupFiles>
{
    public void Configure(EntityTypeBuilder<GroupFiles> builder)
    {
        builder.ToTable("GroupFiles");

        builder.HasKey(pf => pf.Id);

        builder.HasIndex(pf => new { pf.GroupId, pf.FileId })
               .IsUnique();

        builder.Property(pf => pf.GroupId)
            .IsRequired();

        builder.Property(pf => pf.FileId)
            .IsRequired();

        builder.HasOne<FileResource>()
           .WithMany()
           .HasForeignKey(pf => pf.FileId)
           .OnDelete(DeleteBehavior.Restrict);
    }
}
