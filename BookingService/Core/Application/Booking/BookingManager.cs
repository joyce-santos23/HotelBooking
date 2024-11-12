using Application.Booking.Requests;
using Application.Dtos;
using Application.Ports;
using Application.Responses;
using Application.Room.Responses;
using Domain.Booking.Exceptions;
using Domain.Ports;
using Domain.Room.Exceptions;

namespace Application.Booking
{
    public class BookingManager : IBookingManager
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;

        public BookingManager(IBookingRepository bookingRepository, IRoomRepository roomRepository)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
        }

        public async Task<BookingResponse> CreateBooking(CreateBookingRequest request)
        {
            try
            {
                if (request.BookingData.End <= request.BookingData.Start)
                {
                    return new BookingResponse
                    {
                        Success = false,
                        ErrorCode = ErrorCode.COULD_NOT_STORE_DATA,
                        Message = "Start date cannot be before the booking End date."
                    };
                }

                if (!await _bookingRepository.RoomExists(request.BookingData.RoomId))
                {
                    return new BookingResponse
                    {
                        Success = false,
                        ErrorCode = ErrorCode.ROOM_NOT_FOUND,
                        Message = "The specified room does not exist."
                    };
                }

                var room = await _roomRepository.Get(request.BookingData.RoomId);
                if (room != null && room.InMaintenance)
                {
                    return new BookingResponse
                    {
                        Success = false,
                        ErrorCode = ErrorCode.ROOM_IN_MAINTENANCE,
                        Message = "The selected room is under maintenance and cannot be booked."
                    };
                }

                var existingBooking = await _bookingRepository.GetBookingByRoomAndDateRange(
                request.BookingData.RoomId, 
                request.BookingData.Start, 
                request.BookingData.End);

                if (existingBooking != null)
                {
                    return new BookingResponse
                    {
                        Success = false,
                        ErrorCode = ErrorCode.ROOM_NOT_AVAILABLE,
                        Message = "The selected room is already booked for the requested dates."
                    };
                }

                if (!await _bookingRepository.GuestExists(request.BookingData.GuestId))
                {
                    return new BookingResponse
                    {
                        Success = false,
                        ErrorCode = ErrorCode.GUEST_NOT_FOUND,
                        Message = "The specified guest does not exist."
                    };
                }

                var booking = BookingDto.MapToEntity(request.BookingData);
                await booking.Save(_bookingRepository);

                request.BookingData.Id = booking.Id;

                return new BookingResponse
                {
                    BookingData = request.BookingData,
                    Success = true,
                    Message = "Reserva criada com sucesso!"
                };
            }
            catch (InvalidBookingDatesException)
            {
                return new BookingResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.COULD_NOT_STORE_DATA,
                    Message = "Start date cannot be before the booking End date."
                };
            }
            catch (RoomNotFoundException)
            {
                return new BookingResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.ROOM_NOT_FOUND,
                    Message = "Room id must be defined."
                };
            }
            catch (GuestNotFoundException)
            {
                return new BookingResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.GUEST_NOT_FOUND,
                    Message = "The specified guest does not exist."
                };
            }
            catch (RoomInMaintenanceException)
            {
                return new BookingResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.ROOM_IN_MAINTENANCE,
                    Message = "The selected room is under maintenance and cannot be booked."
                };
            }
            catch (RoomNotAvailableException)
            {
                return new BookingResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.ROOM_NOT_AVAILABLE,
                    Message = "The selected room is already booked for the requested dates."
                };
            }
            catch (Exception ex)
            {
                return new BookingResponse
                {
                    Success = false,
                    Message = "Error creating a booking: " + ex.Message
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

        public async Task<BookingResponse> UpdateBooking(int bookingId, UpdateBookingRequest request)
        {
            var existingBooking = await _bookingRepository.Get(bookingId);
            if (existingBooking == null)
            {
                return new BookingResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.BOOKING_NOT_FOUND,
                    Message = "Booking not found."
                };
            }

            if (request.BookingData.End <= request.BookingData.Start)
            {
                return new BookingResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.COULD_NOT_STORE_DATA,
                    Message = "End date must be after the start date."
                };
            }

            var conflictingBooking = await _bookingRepository.GetBookingByRoomAndDateRange(
                existingBooking.RoomId,
                request.BookingData.Start,
                request.BookingData.End);

            if (conflictingBooking != null && conflictingBooking.Id != bookingId)
            {
                return new BookingResponse
                {
                    Success = false,
                    ErrorCode = ErrorCode.ROOM_NOT_AVAILABLE,
                    Message = "The room is already booked for the requested dates."
                };
            }

            existingBooking.Start = request.BookingData.Start;
            existingBooking.End = request.BookingData.End;

            try
            {
                await _bookingRepository.Update(existingBooking);
                return new BookingResponse
                {
                    BookingData = BookingDto.MapToDto(existingBooking),
                    Success = true,
                    Message = "Booking dates updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new BookingResponse
                {
                    Success = false,
                    Message = "Error updating booking dates: " + ex.Message
                };
            }
        }


    }
}
