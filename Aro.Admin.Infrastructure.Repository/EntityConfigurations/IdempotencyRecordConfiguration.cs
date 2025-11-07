using Aro.Admin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Admin.Infrastructure.Repository.EntityConfigurations;

public class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.ToTable("IdempotencyRecords");

        builder.HasKey(i => i.Key);

        builder.Property(i => i.Key)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(i => i.ResponseData)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.ExpiresAt)
            .IsRequired();

        builder.Property(i => i.UserId)
            .IsRequired(false);

        builder.HasIndex(i => i.ExpiresAt);
        builder.HasIndex(i => i.UserId);
    }
}
