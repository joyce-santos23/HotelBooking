using Domain.Enums;

namespace Application.Dtos
{
    public class BookingDto
    {
        public int Id { get; set; }
        public DateTime PlacedAt { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int RoomId { get; set; }
        public int GuestId { get; set; }
        public Status Status { get; set; }

        public static BookingDto MapToDto(Domain.Entities.Booking booking)
        {
            return new BookingDto
            {
                Id = booking.Id,
                PlacedAt = booking.PlacedAt,
                Start = booking.Start,
                End = booking.End,
                RoomId = booking.Room.Id,
                GuestId = booking.Guest.Id,
                Status = booking.Status
            };
        }

        public static Domain.Entities.Booking MapToEntity(BookingDto bookingDto)
        {
            return new Domain.Entities.Booking
            {
                Id = bookingDto.Id,
                PlacedAt = bookingDto.PlacedAt,
                Start = bookingDto.Start,
                End = bookingDto.End,
                RoomId = bookingDto.RoomId,
                GuestId = bookingDto.GuestId,
                Status = bookingDto.Status
            };
        }
    }
}
