using Aro.Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aro.Booking.Infrastructure.Repository.EntityConfigurations;

//public class ContactConfiguration : IEntityTypeConfiguration<Contact>
//{
//    public void Configure(EntityTypeBuilder<Contact> builder)
//    {
//        builder.ToTable("Contacts");

//        builder.HasKey(c => c.Id);

//        builder.Property(c => c.UserId)
//            .IsRequired();

//        builder.HasOne(c => c.User)
//            .WithOne()
//            .HasForeignKey<Contact>(c => c.UserId)
//            .OnDelete(DeleteBehavior.Restrict);
//    }
//}
