﻿using Aro.Admin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Admin.Infrastructure.Repository.Context.Configurations;

public class AuditTrailConfiguration : IEntityTypeConfiguration<AuditTrail>
{
    public void Configure(EntityTypeBuilder<AuditTrail> builder)
    {
        builder.ToTable("AuditTrails");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.ActorUserId).IsRequired(false);

        builder.Property(a => a.ActorName)
            .IsRequired(false)
            .HasMaxLength(256);

        builder.Property(a => a.Action)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(a => a.EntityType)
            .IsRequired(false)
            .HasMaxLength(128);

        builder.Property(a => a.EntityId)
            .IsRequired(false)
            .HasMaxLength(128);

        //builder.Property(a => a.Before)
        //    .HasColumnType("nvarchar(max)")
        //    .IsRequired(false);

        //builder.Property(a => a.After)
        //    .HasColumnType("nvarchar(max)")
        //    .IsRequired(false);

        builder.Property(a => a.Data)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        builder.Property(a => a.IpAddress)
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(a => a.Timestamp)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(a => a.ActorUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(a => new { a.EntityType, a.EntityId });
        
        builder.HasIndex(a => a.Timestamp);
    }
}
