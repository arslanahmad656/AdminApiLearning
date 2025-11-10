using Aro.Admin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Admin.Infrastructure.Repository.EntityConfigurations;

internal class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("PasswordResetTokens");

        builder.HasKey(prt => prt.Id);

        builder.Property(prt => prt.UserId)
            .IsRequired();

        builder.Property(prt => prt.TokenHash)
            .IsRequired()
            .HasMaxLength(1024);

        builder.Property(prt => prt.CreatedAt)
            .IsRequired();

        builder.Property(prt => prt.Expiry)
            .IsRequired();

        builder.Property(prt => prt.IsUsed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(prt => prt.RequestIP)
            .IsRequired()
            .HasMaxLength(45); // IPv6 max length

        builder.Property(prt => prt.UserAgent)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.HasOne(prt => prt.User)
            .WithMany()
            .HasForeignKey(prt => prt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
