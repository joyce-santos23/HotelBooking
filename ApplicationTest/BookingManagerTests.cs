using Moq;
using Application.Booking;
using Application.Booking.Requests;
using Domain.Ports;
using Application.Dtos;

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
                BookingData = new BookingDto
                {
                    RoomId = 1,
                    GuestId = 1,
                    Start = DateTime.UtcNow.AddDays(1),
                    End = DateTime.UtcNow.AddDays(2)
                }
            };

            _mockBookingRepository.Setup(repo => repo.RoomExists(It.IsAny<int>())).ReturnsAsync(false);

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
                BookingData = new BookingDto
                {
                    RoomId = 1,
                    GuestId = 1,
                    Start = DateTime.UtcNow.AddDays(1),
                    End = DateTime.UtcNow.AddDays(2)
                }
            };

            _mockBookingRepository.Setup(repo => repo.RoomExists(It.IsAny<int>())).ReturnsAsync(true);
            _mockBookingRepository.Setup(repo => repo.GuestExists(It.IsAny<int>())).ReturnsAsync(false);

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
                BookingData = new BookingDto
                {
                    RoomId = 1,
                    GuestId = 1,
                    Start = DateTime.UtcNow.AddDays(1),
                    End = DateTime.UtcNow.AddDays(2)
                }
            };

            _mockBookingRepository.Setup(repo => repo.RoomExists(It.IsAny<int>())).ReturnsAsync(true);
            _mockBookingRepository.Setup(repo => repo.GuestExists(It.IsAny<int>())).ReturnsAsync(true);
            _mockRoomRepository.Setup(repo => repo.Get(It.IsAny<int>())).ReturnsAsync(new Domain.Entities.Room { InMaintenance = true });

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
                BookingData = new BookingDto
                {
                    RoomId = 1,
                    GuestId = 1,
                    Start = DateTime.UtcNow.AddDays(1),
                    End = DateTime.UtcNow.AddDays(2)
                }
            };

            _mockBookingRepository.Setup(repo => repo.RoomExists(It.IsAny<int>())).ReturnsAsync(true);
            _mockBookingRepository.Setup(repo => repo.GuestExists(It.IsAny<int>())).ReturnsAsync(true);
            _mockBookingRepository.Setup(repo => repo.GetBookingByRoomAndDateRange(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(new Domain.Entities.Booking());

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
                BookingData = new BookingDto
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
                BookingData = new BookingDto
                {
                    RoomId = 1,
                    GuestId = 1,
                    Start = DateTime.UtcNow.AddDays(1),
                    End = DateTime.UtcNow.AddDays(2)
                }
            };

            _mockBookingRepository.Setup(repo => repo.RoomExists(It.IsAny<int>())).ReturnsAsync(true);
            _mockBookingRepository.Setup(repo => repo.GuestExists(It.IsAny<int>())).ReturnsAsync(true);
            _mockBookingRepository.Setup(repo => repo.Create(It.IsAny<Domain.Entities.Booking>())).ThrowsAsync(new Exception("Database error"));

            var response = await _bookingManager.CreateBooking(request);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Error creating a booking: Database error", response.Message);
        }

        [Test]
        public async Task UpdateBooking_BookingNotFound_ReturnsError()
        {
            var request = new UpdateBookingRequest
            {
                BookingData = new BookingDto
                {
                    RoomId = 1,
                    Start = DateTime.UtcNow.AddDays(3),
                    End = DateTime.UtcNow.AddDays(5)
                }
            };

            _mockBookingRepository.Setup(repo => repo.Get(It.IsAny<int>())).ReturnsAsync((Domain.Entities.Booking)null);

            var response = await _bookingManager.UpdateBooking(1, request);

            Assert.IsFalse(response.Success);
            Assert.AreEqual(ErrorCode.BOOKING_NOT_FOUND, response.ErrorCode);
            Assert.AreEqual("Booking not found.", response.Message);
        }

        [Test]
        public async Task UpdateBooking_InvalidDates_ReturnsError()
        {
            var bookingId = 123;
            var invalidDate = DateTime.Now.AddDays(1);

            var fakeBookingRepo = new Mock<IBookingRepository>();
            var fakeRoomRepo = new Mock<IRoomRepository>();
            fakeBookingRepo.Setup(x => x.Get(bookingId))
                           .ReturnsAsync(new Domain.Entities.Booking { Id = bookingId, Start = DateTime.Now });

            var bookingManager = new BookingManager(fakeBookingRepo.Object, fakeRoomRepo.Object);

            var invalidBookingRequest = new UpdateBookingRequest
            {
                BookingData = new BookingDto
                {
                    Start = DateTime.UtcNow.AddDays(2),
                    End = DateTime.UtcNow.AddDays(1)
                }
            };

            var result = await bookingManager.UpdateBooking(bookingId, invalidBookingRequest);

            Assert.AreEqual(ErrorCode.COULD_NOT_STORE_DATA, result.ErrorCode);
            Assert.AreEqual("End date must be after the start date.", result.Message);
        }

        [Test]
        public async Task UpdateBooking_RoomNotAvailable_ReturnsError()
        {
            var existingBooking = new Domain.Entities.Booking { Id = 1, RoomId = 1, Start = DateTime.UtcNow, End = DateTime.UtcNow.AddDays(2) };
            var request = new UpdateBookingRequest
            {
                BookingData = new BookingDto
                {
                    RoomId = 1,
                    Start = DateTime.UtcNow.AddDays(3),
                    End = DateTime.UtcNow.AddDays(5)
                }
            };

            _mockBookingRepository.Setup(repo => repo.Get(It.IsAny<int>())).ReturnsAsync(existingBooking);
            _mockBookingRepository.Setup(repo => repo.GetBookingByRoomAndDateRange(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(new Domain.Entities.Booking());

            var response = await _bookingManager.UpdateBooking(1, request);

            Assert.IsFalse(response.Success);
            Assert.AreEqual(ErrorCode.ROOM_NOT_AVAILABLE, response.ErrorCode);
            Assert.AreEqual("The room is already booked for the requested dates.", response.Message);
        }

        [Test]
        public async Task UpdateBooking_Success_ReturnsSuccess()
        {
            var existingBooking = new Domain.Entities.Booking
            {
                Id = 1,
                RoomId = 1,
                Start = DateTime.UtcNow.Date,
                End = DateTime.UtcNow.AddDays(2).Date,
                Room = new Domain.Entities.Room
                {
                    Id = 1,
                    Name = "Room 1"
                },
                Guest = new Domain.Entities.Guest
                {
                    Id = 1,
                    Name = "John Doe"
                }
            };

            var request = new UpdateBookingRequest
            {
                BookingData = new BookingDto
                {
                    RoomId = 1,
                    Start = DateTime.UtcNow.AddDays(3).Date,
                    End = DateTime.UtcNow.AddDays(4).Date
                }
            };

            _mockBookingRepository.Setup(repo => repo.Get(It.IsAny<int>())).ReturnsAsync(existingBooking);
            _mockBookingRepository.Setup(repo => repo.GetBookingByRoomAndDateRange(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync((Domain.Entities.Booking)null);
            _mockBookingRepository.Setup(repo => repo.Update(It.IsAny<Domain.Entities.Booking>())).Returns(Task.CompletedTask);

            var response = await _bookingManager.UpdateBooking(1, request);

            Assert.IsTrue(response.Success);
            Assert.AreEqual("Booking dates updated successfully.", response.Message);

            _mockBookingRepository.Verify(repo => repo.Update(It.Is<Domain.Entities.Booking>(b => b.Id == 1 && b.Start.Date == request.BookingData.Start && b.End.Date == request.BookingData.End)), Times.Once);
        }

        [Test]
        public async Task UpdateBooking_Exception_ReturnsError()
        {
            var existingBooking = new Domain.Entities.Booking { Id = 1, RoomId = 1, Start = DateTime.UtcNow, End = DateTime.UtcNow.AddDays(2) };
            var request = new UpdateBookingRequest
            {
                BookingData = new BookingDto
                {
                    RoomId = 1,
                    Start = DateTime.UtcNow.AddDays(3),
                    End = DateTime.UtcNow.AddDays(5)
                }
            };

            _mockBookingRepository.Setup(repo => repo.Get(It.IsAny<int>())).ReturnsAsync(existingBooking);
            _mockBookingRepository.Setup(repo => repo.Update(It.IsAny<Domain.Entities.Booking>())).ThrowsAsync(new Exception("Database error"));

            var response = await _bookingManager.UpdateBooking(1, request);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Error updating booking dates: Database error", response.Message);
        }
    }
}
