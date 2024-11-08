using Domain.Booking.Exceptions;
using Domain.Enums;
using Domain.Ports;
using System.Text.Json.Serialization;
using Action = Domain.Enums.Action;

namespace Domain.Entities
{
    public class Booking
    {
        private DateTime _start;
        private DateTime _end;

        public Booking()
        {
            Status = Status.Created;
            PlacedAt = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public DateTime PlacedAt { get; set; }
        public DateTime Start
        {
            get => _start;
            set => _start = value.Date;
        }

        public DateTime End
        {
            get => _end;
            set => _end = value.Date; 
        }
        [JsonIgnore] 
        public DateTime StartDateOnly => Start.Date;

        [JsonIgnore] 
        public DateTime EndDateOnly => End.Date;
        public int RoomId { get; set; }  
        public int GuestId { get; set; }
        public Room Room { get; set; }

        public Guest Guest { get; set; }

        public Status Status { get; set; }
        public Status CurrentStatus { 
            get{ return this.Status;} 
        }

        //Máquina de estado - POO
        public void ChangeState(Action action){
            
            Status = (Status, action) switch
            {
                (Status.Created, Action.Pay) => Status.Paid,
                (Status.Created, Action.Cancel) => Status.Canceled,
                (Status.Paid, Action.Finish) => Status.Finished,
                (Status.Paid, Action.Refound) => Status.Refounded,
                (Status.Canceled, Action.Reopen) => Status.Created,

                _=> Status
                
            };
        }

        public void Validate()
        {

            if (Start >= End)
                throw new InvalidBookingDatesException();

            if (Start < PlacedAt)
                throw new InvalidBookingDatesException();

            if (RoomId <= 0)
                throw new RoomNotFoundException();

            if (GuestId <= 0)
                throw new GuestNotFoundException();
        }

        public async Task Save(IBookingRepository bookingRepository)
        {
            this.Validate();

            if (this.Id == 0)
            {
                this.Id = await bookingRepository.Create(this);
            }
            else
            {
                //await
            }
        }




    }
}
