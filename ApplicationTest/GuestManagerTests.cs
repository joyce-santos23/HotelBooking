using Application;
using Application.Dtos;
using Application.Guest;
using Application.Guest.Requests;
using Domain.Entities;
using Domain.Ports;
using Moq;

namespace ApplicationTest
{
    public class Tests
    {
        GuestManager guestManager;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public async Task HappyPath()
        {
            var guestDto = new GuestDto
            {
                Name = "Fulano",
                Surname = "De tal",
                Email = "fulano@email.com",
                IdNumber = "abcd",
                IdTypeCode = 1,
            };

            int expectedId = 222;

            var request = new CreateGuestRequest()
            {
                Data = guestDto,
            };

            var fakeRepo = new Mock<IGuestRepository>();

            fakeRepo.Setup(x => x.Create(It.IsAny<Guest>())).Returns(Task.FromResult(expectedId));

            guestManager = new GuestManager(fakeRepo.Object, Mock.Of<IBookingRepository>());  

            var res = await guestManager.CreateGuest(request);

            Assert.IsNotNull(res);
            Assert.True(res.Success);
            Assert.AreEqual(res.Data.Id, expectedId);
            Assert.AreEqual(res.Data.Name, guestDto.Name);
            Assert.AreEqual(res.Data.Surname, guestDto.Surname);
            Assert.AreEqual(res.Data.Email, guestDto.Email);
            Assert.AreEqual(res.Data.IdNumber, guestDto.IdNumber);
            Assert.AreEqual(res.Data.IdTypeCode, guestDto.IdTypeCode);
        }

        [TestCase("")]
        [TestCase("a")]
        [TestCase("ab")]
        [TestCase("abc")]
        public async Task Should_Return_InvalidPersonDocumentIdException_WhenDocsAreInvalid(string docNumber)
        {
            var guestDto = new GuestDto
            {
                Name = "Fulano",
                Surname = "De tal",
                Email = "fulano@email.com",
                IdNumber = docNumber,
                IdTypeCode = 1,
            };

            var request = new CreateGuestRequest()
            {
                Data = guestDto,
            };

            var fakeRepo = new Mock<IGuestRepository>();

            fakeRepo.Setup(x => x.Create(It.IsAny<Guest>())).Returns(Task.FromResult(222));

            guestManager = new GuestManager(fakeRepo.Object, Mock.Of<IBookingRepository>());  

            var res = await guestManager.CreateGuest(request);

            Assert.IsNotNull(res);
            Assert.False(res.Success);
            Assert.AreEqual(res.ErrorCode, ErrorCode.INVALID_PERSON_ID);
            Assert.AreEqual(res.Message, "The passed ID is not valid");
        }

        [TestCase("", "Surname teste", "email@email.com")]
        [TestCase("Name", "", "email@email.com")]
        [TestCase("Name", "Surname teste", "")]
        [TestCase("", "", "")]
        public async Task Should_Return_MissingRequiredInformation_WhenDocsAreInvalid(
            string name,
            string surname,
            string email)
        {
            var guestDto = new GuestDto
            {
                Name = name,
                Surname = surname,
                Email = email,
                IdNumber = "abcd",
                IdTypeCode = 1,
            };

            var request = new CreateGuestRequest()
            {
                Data = guestDto,
            };

            var fakeRepo = new Mock<IGuestRepository>();

            fakeRepo.Setup(x => x.Create(It.IsAny<Guest>())).Returns(Task.FromResult(222));

            guestManager = new GuestManager(fakeRepo.Object, Mock.Of<IBookingRepository>());  

            var res = await guestManager.CreateGuest(request);

            Assert.IsNotNull(res);
            Assert.False(res.Success);
            Assert.AreEqual(res.ErrorCode, ErrorCode.MISSING_REQUIRED_INFORMATION);
            Assert.AreEqual(res.Message, "Missing passed required information");
        }

        [TestCase("emailsemarrobasemponto")]
        [TestCase("b@b.com")]
        public async Task Should_Return_InvalidEmailException_WhenDocsAreInvalid(string email)
        {
            var guestDto = new GuestDto
            {
                Name = "Fulano",
                Surname = "De tal",
                Email = email,
                IdNumber = "abcd",
                IdTypeCode = 1,
            };

            var request = new CreateGuestRequest()
            {
                Data = guestDto,
            };

            var fakeRepo = new Mock<IGuestRepository>();

            fakeRepo.Setup(x => x.Create(It.IsAny<Guest>())).Returns(Task.FromResult(222));

            guestManager = new GuestManager(fakeRepo.Object, Mock.Of<IBookingRepository>());  

            var res = await guestManager.CreateGuest(request);

            Assert.IsNotNull(res);
            Assert.False(res.Success);
            Assert.AreEqual(res.ErrorCode, ErrorCode.INVALID_EMAIL);
            Assert.AreEqual(res.Message, "The given email is not valid");
        }

        [Test]
        public async Task Should_Return_GuestNotFound_WhenDocsAreInvalid()
        {
            var fakeRepo = new Mock<IGuestRepository>();

            fakeRepo.Setup(x => x.Get(333)).Returns(Task.FromResult<Guest?>(null));

            guestManager = new GuestManager(fakeRepo.Object, Mock.Of<IBookingRepository>());  

            var res = await guestManager.GetGuest(333);

            Assert.IsNotNull(res);
            Assert.False(res.Success);
            Assert.AreEqual(res.ErrorCode, ErrorCode.GUEST_NOT_FOUND);
            Assert.AreEqual(res.Message, "No guest record was found with the given id");
        }

        [Test]
        public async Task Should_Return_Guest_Success()
        {
            var fakeRepo = new Mock<IGuestRepository>();

            var fakeGuest = new Guest
            {
                Id = 333,
                Name = "Test",
                DocumentId = new Domain.ValueObjects.PersonId
                {
                    DocumentType = Domain.Enums.DocumentType.DriveLicence,
                    IdNumber = "123"
                }
            };

            fakeRepo.Setup(x => x.Get(333)).Returns(Task.FromResult<Guest?>(fakeGuest));

            guestManager = new GuestManager(fakeRepo.Object, Mock.Of<IBookingRepository>());  

            var res = await guestManager.GetGuest(333);

            Assert.IsNotNull(res);
            Assert.True(res.Success);
            Assert.AreEqual(res.Data.Id, fakeGuest.Id);
            Assert.AreEqual(res.Data.Name, fakeGuest.Name);
        }

        [Test]
        public async Task Should_DeleteGuest_Successfully()
        {
            int guestId = 1;
            var fakeRepo = new Mock<IGuestRepository>();

            fakeRepo.Setup(x => x.Get(guestId)).ReturnsAsync(new Guest { Id = guestId, Name = "John Doe" });

            fakeRepo.Setup(x => x.Delete(guestId)).ReturnsAsync(true);

            guestManager = new GuestManager(fakeRepo.Object, Mock.Of<IBookingRepository>());  

            var res = await guestManager.DeleteGuest(guestId);

            Assert.IsNotNull(res);
            Assert.True(res.Success);
            Assert.AreEqual("Guest deleted successfully", res.Message);
        }

        [Test]
        public async Task Should_Return_Failure_When_Guest_Not_Found_For_Delete()
        {
            int guestId = 999;
            var fakeRepo = new Mock<IGuestRepository>();

            fakeRepo.Setup(x => x.Delete(guestId)).ReturnsAsync(false);

            guestManager = new GuestManager(fakeRepo.Object, Mock.Of<IBookingRepository>());  

            var res = await guestManager.DeleteGuest(guestId);

            Assert.IsNotNull(res);
            Assert.False(res.Success);
            Assert.AreEqual("No guest record was found with the given id", res.Message);
        }

        [Test]
        public async Task Should_Return_CannotDeleteGuestWithBookings_When_Guest_Has_Bookings()
        {
            int guestId = 1;
            var fakeGuestRepo = new Mock<IGuestRepository>();
            var fakeBookingRepo = new Mock<IBookingRepository>();

            fakeGuestRepo.Setup(x => x.Get(guestId)).ReturnsAsync(new Guest
            {
                Id = guestId,
                Name = "John Doe"
            });

            fakeBookingRepo.Setup(x => x.HasBookingsForGuest(guestId)).ReturnsAsync(true);

            guestManager = new GuestManager(fakeGuestRepo.Object, fakeBookingRepo.Object);

            var res = await guestManager.DeleteGuest(guestId);

            Assert.IsNotNull(res);
            Assert.False(res.Success);
            Assert.AreEqual(ErrorCode.CANNOT_DELETE_GUEST_WITH_BOOKINGS, res.ErrorCode);
            Assert.AreEqual("Cannot delete guest because they have existing bookings.", res.Message);
        }
    }
}
