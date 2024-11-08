using Domain.Enums;
using Domain.Ports;
using Domain.Room.Exceptions;
using Domain.Room.ValueObjects;

namespace Domain.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public bool InMaintenance { get; set; }
        public Price Price { get; set; }

        public List<Booking> Bookings { get; set; } = new List<Booking>();

        public bool IsAvailable => !InMaintenance;
        public bool IsAvailableForDates(DateTime start, DateTime end)
        {
            if (InMaintenance)
            {
                throw new RoomNotAvailableException();
            }

            bool isAvailable = !Bookings.Any(booking =>
                booking.Status == Status.Paid &&
                !(end <= booking.Start || start >= booking.End));

            if (!isAvailable)
            {
                throw new RoomNotAvailableException();
            }

            return isAvailable;
        }


        public void ValidateName()
        {
            if (string.IsNullOrEmpty(Name))
            {
                throw new InvalidRoomNameException();
            }
        }

        public void ValidateLevel()
        {
            if (Level <= 0)
            {
                throw new InvalidRoomLevelException();
            }
        }

        public void ValidatePrice()
        {
            if (Price == null || Price.Value <= 0)
            {
                throw new InvalidRoomPriceException();
            }
        }

        public void Validate()
        {
            ValidateName();
            ValidateLevel();
            ValidatePrice();
        }

        public async Task Save(IRoomRepository roomRepository)
        {
            this.Validate();

            if (this.Id == 0)
            {
                this.Id = await roomRepository.Create(this);
            }
            else
            {
                //await
            }
        }
    }
}

