using Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace Data.Booking
{
    public class BookingRepository : IBookingRepository
    {
        private readonly HotelDbContext _hotelDbContext;

        public BookingRepository(HotelDbContext hotelDbContext)
        {
            _hotelDbContext = hotelDbContext;

        }
        public async Task<int> Create(Domain.Entities.Booking booking)
        {
            _hotelDbContext.Bokings.Add(booking);
            await _hotelDbContext.SaveChangesAsync();
            return booking.Id;
        }

        public async Task<Domain.Entities.Booking> Get(int Id)
        {
            return await _hotelDbContext.Bokings
                            .Include(b => b.Room) 
                            .Include(b => b.Guest)
                            .FirstOrDefaultAsync(b => b.Id == Id);
        }

        public async Task<IEnumerable<Domain.Entities.Booking>> GetAll()
        {
            return await _hotelDbContext.Bokings
                            .Include(b => b.Room) 
                            .Include(b => b.Guest)
                            .ToListAsync();
        }


        public async Task<bool> Delete(int Id)
        {
            var booking = await _hotelDbContext.Bokings.FindAsync(Id);
            if (booking == null)
                return false;

            _hotelDbContext.Bokings.Remove(booking);
            await _hotelDbContext.SaveChangesAsync();
            return true;
        }


    }
}
