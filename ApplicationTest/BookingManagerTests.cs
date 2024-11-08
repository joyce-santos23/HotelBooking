using Moq;
using Application.Booking;
using Application.Booking.Requests;
using Domain.Ports;
using Domain.Booking.Exceptions;
using Domain.Room.Exceptions;
using Application.Responses;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Application.Tests
{
    public class Tests
    {
        private Mock<IBookingRepository> _mockBookingRepository;
        private Mock<IRoomRepository> _mockRoomRepository;
        private BookingManager _bookingManager;

        [SetUp]
        public void SetUp()
        {
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockRoomRepository = new Mock<IRoomRepository>();
            _bookingManager = new BookingManager(_mockBookingRepository.Object, _mockRoomRepository.Object);
        }

        [Test]
        public async Task CreateBooking_RoomNotFound_ReturnsError()
        {
            var request = new CreateBookingRequest
            {
                BookingData = new Application.Dtos.BookingDto
                {
                    RoomId = 1,
                    GuestId = 1,
                    Start = DateTime.UtcNow.AddDays(1),
                    End = DateTime.UtcNow.AddDays(2)
                }
            };

            _mockBookingRepository.Setup(repo => repo.RoomExists(It.IsAny<int>())).ReturnsAsync(false);  // Simula que o RoomId não existe

            var response = await _bookingManager.CreateBooking(request);

            Assert.IsFalse(response.Success);
            Assert.AreEqual(ErrorCode.ROOM_NOT_FOUND, response.ErrorCode);
            Assert.AreEqual("The specified room does not exist.", response.Message);
        }

        [Test]
        public async Task CreateBooking_GuestNotFound_ReturnsError()
        {
            var request = new CreateBookingRequest
            {
                BookingData = new Application.Dtos.BookingDto
                {
                    RoomId = 1,
                    GuestId = 1,
                    Start = DateTime.UtcNow.AddDays(1),
                    End = DateTime.UtcNow.AddDays(2)
                }
            };

            _mockBookingRepository.Setup(repo => repo.RoomExists(It.IsAny<int>())).ReturnsAsync(true);  // RoomId existe
            _mockBookingRepository.Setup(repo => repo.GuestExists(It.IsAny<int>())).ReturnsAsync(false);  // Simula que o GuestId não existe

            var response = await _bookingManager.CreateBooking(request);

            Assert.IsFalse(response.Success);
            Assert.AreEqual(ErrorCode.GUEST_NOT_FOUND, response.ErrorCode);
            Assert.AreEqual("The specified guest does not exist.", response.Message);
        }

        [Test]
        public async Task CreateBooking_RoomInMaintenance_ReturnsError()
        {
            var request = new CreateBookingRequest
            {
                BookingData = new Application.Dtos.BookingDto
                {
                    RoomId = 1,
                    GuestId = 1,
                    Start = DateTime.UtcNow.AddDays(1),
                    End = DateTime.UtcNow.AddDays(2)
                }
            };

            _mockBookingRepository.Setup(repo => repo.RoomExists(It.IsAny<int>())).ReturnsAsync(true);  // RoomId existe
            _mockBookingRepository.Setup(repo => repo.GuestExists(It.IsAny<int>())).ReturnsAsync(true);  // GuestId existe
            _mockRoomRepository.Setup(repo => repo.Get(It.IsAny<int>())).ReturnsAsync(new Domain.Entities.Room { InMaintenance = true }); // Room em manutenção

            var response = await _bookingManager.CreateBooking(request);

            Assert.IsFalse(response.Success);
            Assert.AreEqual(ErrorCode.ROOM_IN_MAINTENANCE, response.ErrorCode);
            Assert.AreEqual("The selected room is under maintenance and cannot be booked.", response.Message);
        }

        [Test]
        public async Task CreateBooking_RoomNotAvailable_ReturnsError()
        {
            var request = new CreateBookingRequest
            {
                BookingData = new Application.Dtos.BookingDto
                {
                    RoomId = 1,
                    GuestId = 1,
                    Start = DateTime.UtcNow.AddDays(1),
                    End = DateTime.UtcNow.AddDays(2)
                }
            };

            _mockBookingRepository.Setup(repo => repo.RoomExists(It.IsAny<int>())).ReturnsAsync(true);  // RoomId existe
            _mockBookingRepository.Setup(repo => repo.GuestExists(It.IsAny<int>())).ReturnsAsync(true);  // GuestId existe
            _mockBookingRepository.Setup(repo => repo.GetBookingByRoomAndDateRange(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(new Domain.Entities.Booking()); // Room já reservado

            var response = await _bookingManager.CreateBooking(request);

            Assert.IsFalse(response.Success);
            Assert.AreEqual(ErrorCode.ROOM_NOT_AVAILABLE, response.ErrorCode);
            Assert.AreEqual("The selected room is already booked for the requested dates.", response.Message);
        }

        [Test]
        public async Task CreateBooking_InvalidBookingDatesException_ReturnsError_WhenEndDateIsBeforeStartDate()
        {
            var request = new CreateBookingRequest
            {
                BookingData = new Application.Dtos.BookingDto
                {
                    RoomId = 1,
                    GuestId = 1,
                    Start = DateTime.UtcNow.AddDays(2),
                    End = DateTime.UtcNow.AddDays(1)
                }
            };

            var response = await _bookingManager.CreateBooking(request);

            Assert.IsFalse(response.Success);
            Assert.AreEqual(ErrorCode.COULD_NOT_STORE_DATA, response.ErrorCode);
            Assert.AreEqual("Start date cannot be before the booking End date.", response.Message);
        }

        [Test]
        public async Task CreateBooking_Exception_ReturnsError()
        {
            var request = new CreateBookingRequest
            {
                BookingData = new Application.Dtos.BookingDto
                {
                    RoomId = 1,
                    GuestId = 1,
                    Start = DateTime.UtcNow.AddDays(1),
                    End = DateTime.UtcNow.AddDays(2)
                }
            };

            _mockBookingRepository.Setup(repo => repo.RoomExists(It.IsAny<int>())).ReturnsAsync(true);  // RoomId existe
            _mockBookingRepository.Setup(repo => repo.GuestExists(It.IsAny<int>())).ReturnsAsync(true);  // GuestId existe
            _mockBookingRepository.Setup(repo => repo.Create(It.IsAny<Domain.Entities.Booking>())).ThrowsAsync(new Exception("Database error"));  // Simula uma exceção no banco de dados

            var response = await _bookingManager.CreateBooking(request);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Error creating a booking: Database error", response.Message);
        }
    }
}
