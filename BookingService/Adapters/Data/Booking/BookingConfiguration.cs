using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Booking
{
    public class BookingConfiguration : IEntityTypeConfiguration<Domain.Entities.Booking>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Booking> builder)
        {
            builder.HasKey(b => b.Id);

            builder.HasOne(b => b.Room)
                    .WithMany(r => r.Bookings) 
                    .HasForeignKey(b => b.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Guest)
                   .WithMany()
                   .HasForeignKey(b => b.GuestId);
        }
    }
}
