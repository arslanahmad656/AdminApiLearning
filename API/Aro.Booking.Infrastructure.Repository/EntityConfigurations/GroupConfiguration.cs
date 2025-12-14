using Aro.Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Booking.Infrastructure.Repository.EntityConfigurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("Groups");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.GroupName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(g => g.PrimaryContact)
            .WithMany()
            .HasForeignKey(g => g.PrimaryContactId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.Address)
            .WithMany()
            .HasForeignKey(g => g.AddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.Icon)
            .WithMany()
            .HasForeignKey(l => l.IconId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
