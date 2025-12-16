using Aro.Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Booking.Infrastructure.Repository.EntityConfigurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.RoomName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.RoomCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Description)
            .HasMaxLength(800);

        builder.Property(r => r.MaxOccupancy).IsRequired();
        builder.Property(r => r.MaxAdults).IsRequired();
        builder.Property(r => r.MaxChildren).IsRequired();
        builder.Property(r => r.RoomSize).IsRequired();
        builder.Property(r => r.PropertyId).IsRequired();

        builder.Property(r => r.BedConfig)
            .IsRequired()
            .HasConversion<int>();

        builder
            .HasMany(r => r.RoomAmenities)
            .WithOne(ra => ra.Room)
            .HasForeignKey(ra => ra.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Property)
            .WithMany(p => p.Rooms)
            .HasForeignKey(r => r.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.RoomFiles)
            .WithOne()
            .HasForeignKey(rf => rf.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
