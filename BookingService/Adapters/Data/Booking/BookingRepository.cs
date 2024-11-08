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

        public async Task<bool> RoomExists(int roomId)
        {
            return await _hotelDbContext.Rooms.AnyAsync(r => r.Id == roomId);
        }

        public async Task<bool> GuestExists(int guestId)
        {
            return await _hotelDbContext.Guests.AnyAsync(g => g.Id == guestId);
        }

        public async Task<Domain.Entities.Booking> GetBookingByRoomAndDateRange(int roomId, DateTime startDate, DateTime endDate)
        {
            // Ajusta as datas para garantir que estamos trabalhando apenas com a parte da data (sem hora)
            var startDateOnly = startDate.Date;
            var endDateOnly = endDate.Date;

            return await _hotelDbContext.Bokings
                .FirstOrDefaultAsync(b => b.RoomId == roomId &&
                                          b.Start.Date < endDateOnly &&  // Compara apenas as datas, sem hora
                                          b.End.Date > startDateOnly);   // Compara apenas as datas, sem hora
        }

    }
}
