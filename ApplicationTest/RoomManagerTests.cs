using Application.Room.Requests;
using Domain.Entities;
using Domain.Ports;
using Moq;
using Application.Dtos;
using Application.Room;
using Domain.Room.Exceptions;

namespace ApplicationTest
{
    [TestFixture]
    public class RoomManagerTests
    {
        private Mock<IRoomRepository> _mockRoomRepository;
        private RoomManager _roomManager;

        [SetUp]
        public void SetUp()
        {
            _mockRoomRepository = new Mock<IRoomRepository>();
            _roomManager = new RoomManager(_mockRoomRepository.Object);
        }

        [Test]
        public async Task CreateRoom_ShouldReturnSuccess_WhenRoomIsCreatedSuccessfully()
        {
            var request = new CreateRoomRequest
            {
                RoomData = new RoomDto
                {
                    Name = "Test Room",
                    Level = 1,
                    PriceValue = 100.0m 
                }
            };

            _mockRoomRepository.Setup(repo => repo.Create(It.IsAny<Room>())).ReturnsAsync(1); 

            var response = await _roomManager.CreateRoom(request);

            Assert.IsTrue(response.Success);
            Assert.AreEqual("Sala criada com sucesso!", response.Message);
            Assert.AreEqual(1, response.RoomData.Id);
        }

        [Test]
        public async Task CreateRoom_InvalidRoomNameException_WhenRoomNameIsMissing()
        {
            var request = new CreateRoomRequest
            {
                RoomData = new RoomDto
                {
                    Name = string.Empty, 
                    Level = 1,
                    PriceValue = 100.0m
                }
            };

            var response = await _roomManager.CreateRoom(request);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Missing passed name information", response.Message);
        }

        [Test]
        public async Task CreateRoom_InvalidRoomPriceException_WhenPriceIsZeroOrNegative()
        {
            var request = new CreateRoomRequest
            {
                RoomData = new RoomDto
                {
                    Name = "Test Room",
                    Level = 1,
                    PriceValue = -10.0m 
                }
            };

            var response = await _roomManager.CreateRoom(request);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("The passed price cannot be <= 0", response.Message);
        }

        [Test]
        public async Task CreateRoom_InvalidRoomLevelException_WhenRoomLevelIsInvalid()
        {
            var request = new CreateRoomRequest
            {
                RoomData = new RoomDto
                {
                    Name = "Test Room",
                    Level = 0, 
                    PriceValue = 100.0m
                }
            };

            var response = await _roomManager.CreateRoom(request);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Missing passed level information", response.Message);
        }

        [Test]
        public async Task CreateRoom_RoomNotAvailableException_WhenRoomIsNotAvailable()
        {
            var request = new CreateRoomRequest
            {
                RoomData = new RoomDto
                {
                    Name = "Test Room",
                    Level = 1,
                    PriceValue = 100.0m
                }
            };

            _mockRoomRepository.Setup(repo => repo.Create(It.IsAny<Room>())).ThrowsAsync(new RoomNotAvailableException());

            var response = await _roomManager.CreateRoom(request);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("The passed Room is not available", response.Message);
        }

    }
}
