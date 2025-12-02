using Aro.Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Booking.Infrastructure.Repository.EntityConfigurations;

public class RoomAmenityConfiguration : IEntityTypeConfiguration<RoomAmenity>
{
    public void Configure(EntityTypeBuilder<RoomAmenity> builder)
    {
        builder.ToTable("RoomAmenities");

        builder.HasKey(ra => new { ra.RoomId, ra.AmenityId });

        builder.Property(ra => ra.RoomId).IsRequired();
        builder.Property(ra => ra.AmenityId).IsRequired();

        builder.HasIndex(ra => ra.AmenityId);
    }
}
