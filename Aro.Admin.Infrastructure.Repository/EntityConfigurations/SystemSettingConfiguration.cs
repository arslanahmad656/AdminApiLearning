using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Admin.Infrastructure.Repository.EntityConfigurations;

public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.ToTable("SystemSettings");

        builder.HasKey(s => s.Key);

        builder.Property(s => s.Key)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(s => s.Value)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        var sharedKeys = new SharedKeys();
        builder.HasData(new SystemSetting
        {
            Key = sharedKeys.IS_SYSTEM_INITIALIZED,
            Value = false.ToString(),
        },
        new SystemSetting
        {
            Key = sharedKeys.IS_MIGRATIONS_COMPLETE,
            Value = false.ToString(),
        },
        new SystemSetting
        {
            Key = sharedKeys.IS_DATABASE_SEEDED_AT_STARTUP,
            Value = false.ToString()
        });
    }
}
