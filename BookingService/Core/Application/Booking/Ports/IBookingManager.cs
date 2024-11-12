using Application.Booking.Requests;
using Application.Responses;

namespace Application.Ports
{
    public interface IBookingManager
    {
        Task<BookingResponse> CreateBooking(CreateBookingRequest request);
        Task<BookingResponse> GetBooking(int bookingId);
        Task<IEnumerable<BookingResponse>> GetAllBookings();
        Task<bool> DeleteBooking(int guestId);
        Task<BookingResponse> UpdateBooking(int bookingId, UpdateBookingRequest request);
    }
}
