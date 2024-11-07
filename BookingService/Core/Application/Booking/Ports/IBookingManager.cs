using Application.Dtos;
using Application.Booking.Requests;
using Application.Responses;
using Application.Guest.Requests;
using Application.Room.Responses;

namespace Application.Ports
{
    public interface IBookingManager
    {
        Task<BookingResponse> CreateBooking(CreateBookingRequest request);
        Task<BookingResponse> GetBooking(int bookingId);
        Task<IEnumerable<BookingResponse>> GetAllBookings();
        Task<bool> DeleteBooking(int guestId);
    }
}
