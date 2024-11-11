using Application.Room.Requests;
using Domain.Entities;
using Domain.Ports;
using Moq;
using Application.Dtos;
using Application.Room;
using Domain.Room.Exceptions;
using Domain.Room.Enums;
using Domain.Room.ValueObjects;

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
            Assert.AreEqual("The room has been created successfully!", response.Message);
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
            Assert.AreEqual("Missing name information", response.Message);
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
            Assert.AreEqual("Missing level information", response.Message);
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

        [Test]
        public async Task UpdateRoom_ShouldReturnSuccess_WhenRoomIsUpdatedSuccessfully()
        {
            var roomId = 1;
            var existingRoom = new Room
            {
                Id = roomId,
                Name = "Original Room",
                Level = 1,
                Price = new Price { Value = 100.0m, Currency = AcceptedCurrencies.Dolar },
                InMaintenance = false
            };

            var updateRequest = new UpdateRoomRequest
            {
                RoomData = new RoomDto
                {
                    Name = "Updated Room",
                    PriceValue = 150.0m,
                    CurrencyCode = (int)AcceptedCurrencies.Dolar,
                    InMaintenance = true
                }
            };

            _mockRoomRepository.Setup(repo => repo.Get(roomId)).ReturnsAsync(existingRoom);
            _mockRoomRepository.Setup(repo => repo.Update(It.IsAny<Room>())).Returns(Task.CompletedTask);

            var response = await _roomManager.UpdateRoom(roomId, updateRequest);

            Assert.IsTrue(response.Success);
            Assert.AreEqual("Room updated successfully!", response.Message);
            Assert.AreEqual(updateRequest.RoomData.Name, response.RoomData.Name);
            Assert.AreEqual(updateRequest.RoomData.PriceValue, response.RoomData.PriceValue);
            Assert.AreEqual(updateRequest.RoomData.InMaintenance, response.RoomData.InMaintenance);
        }

        [Test]
        public async Task UpdateRoom_ShouldReturnNotFound_WhenRoomDoesNotExist()
        {
            var roomId = 1;
            var updateRequest = new UpdateRoomRequest
            {
                RoomData = new RoomDto
                {
                    Name = "Updated Room",
                    PriceValue = 150.0m,
                    CurrencyCode = (int)AcceptedCurrencies.Dolar,
                    InMaintenance = true
                }
            };

            _mockRoomRepository.Setup(repo => repo.Get(roomId)).ReturnsAsync((Room)null);

            var response = await _roomManager.UpdateRoom(roomId, updateRequest);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Room not found.", response.Message);
        }

        [Test]
        public async Task UpdateRoom_ShouldReturnError_WhenPriceIsInvalid()
        {
            var roomId = 1;
            var existingRoom = new Room
            {
                Id = roomId,
                Name = "Original Room",
                Level = 1,
                Price = new Price { Value = 100.0m, Currency = AcceptedCurrencies.Dolar },
                InMaintenance = false
            };

            var updateRequest = new UpdateRoomRequest
            {
                RoomData = new RoomDto
                {
                    Name = "Updated Room",
                    PriceValue = -50.0m,
                    CurrencyCode = (int)AcceptedCurrencies.Dolar,
                    InMaintenance = true
                }
            };

            _mockRoomRepository.Setup(repo => repo.Get(roomId)).ReturnsAsync(existingRoom);

            var response = await _roomManager.UpdateRoom(roomId, updateRequest);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("The passed price cannot be <= 0", response.Message);
        }

        [Test]
        public async Task DeleteRoom_ShouldReturnSuccess_WhenRoomIsDeletedSuccessfully()
        {

            var roomId = 1;
            _mockRoomRepository.Setup(repo => repo.Get(roomId)).ReturnsAsync(new Room { Id = roomId });
            _mockRoomRepository.Setup(repo => repo.Delete(roomId)).ReturnsAsync(true);

            var response = await _roomManager.DeleteRoom(roomId);

            Assert.IsTrue(response.Success);
            Assert.AreEqual("The room was successfully deleted.", response.Message);
        }

        [Test]
        public async Task DeleteRoom_ShouldReturnNotFound_WhenRoomDoesNotExist()
        {

            var roomId = 1;
            _mockRoomRepository.Setup(repo => repo.Get(roomId)).ReturnsAsync((Room)null);


            var response = await _roomManager.DeleteRoom(roomId);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Room not found", response.Message);
        }



    }
}
