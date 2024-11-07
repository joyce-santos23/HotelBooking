using Application.Booking.Requests;
using Application.Dtos;
using Application.Ports;
using Application.Responses;
using Domain.Ports;

namespace Application.Booking
{
    public class BookingManager : IBookingManager
    {
        private IBookingRepository _bookingRepository;

        public BookingManager(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }
        public async Task<BookingResponse> CreateBooking(CreateBookingRequest request)
        {
            try
            {
                var booking = BookingDto.MapToEntity(request.BookingData);
                request.BookingData.Id = await _bookingRepository.Create(booking);
                //await booking.Save(_bookingRepository);
                return new BookingResponse
                {
                    BookingData = request.BookingData,
                    Success = true,
                    Message = "Reserva criada com sucesso!"
                };
            }
            catch (Exception ex)
            {
                return new BookingResponse
                {
                    Success = false,
                    Message = "Erro ao criar sala: " + ex.Message
                };
            }
        }

        public async Task<BookingResponse> GetBooking(int bookingId)
        {
            var booking = await _bookingRepository.Get(bookingId);

            if (booking == null)
            {
                return new BookingResponse
                {
                    Success = false
                };
            }

            return new BookingResponse
            {
                BookingData = BookingDto.MapToDto(booking),
                Success = true
            };
        }

        public async Task<IEnumerable<BookingResponse>> GetAllBookings()
        {
            var bookings = await _bookingRepository.GetAll();
            var responseList = new List<BookingResponse>();

            foreach (var booking in bookings)
            {
                responseList.Add(new BookingResponse
                {
                    BookingData = BookingDto.MapToDto(booking),
                    Success = true
                });
            }

            return responseList;
        }

        public async Task<bool> DeleteBooking(int bookingId)
        {
            try
            {
                var booking = await _bookingRepository.Get(bookingId);
                if (booking == null)
                {
                    return false;
                }

                await _bookingRepository.Delete(bookingId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
