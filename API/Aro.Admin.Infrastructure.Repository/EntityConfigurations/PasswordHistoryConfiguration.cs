using Aro.Admin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Admin.Infrastructure.Repository.EntityConfigurations;

internal class PasswordHistoryConfiguration : IEntityTypeConfiguration<PasswordHistory>
{
	public void Configure(EntityTypeBuilder<PasswordHistory> builder)
	{
		builder.ToTable("PasswordHistories");

		builder.HasKey(ph => ph.Id);

		builder.Property(ph => ph.UserId)
			.IsRequired();

		builder.Property(ph => ph.PasswordHash)
			.IsRequired()
			.HasMaxLength(1024);

		builder.Property(ph => ph.PasswordSetDate)
			.IsRequired();

		builder.HasOne(ph => ph.User)
			.WithMany()
			.HasForeignKey(ph => ph.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}


