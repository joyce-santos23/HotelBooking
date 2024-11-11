using Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace Data.Room
{
    public class RoomRepository : IRoomRepository
    {
        private readonly HotelDbContext _hotelDbContext;
        public RoomRepository(HotelDbContext hotelDbContext)
        {
            _hotelDbContext = hotelDbContext;

        }
        public async Task<int> Create(Domain.Entities.Room room)
        {
            _hotelDbContext.Rooms.Add(room);
            await _hotelDbContext.SaveChangesAsync();
            return room.Id;
        }

        public async Task<bool> Delete(int Id)
        {
            var room = await _hotelDbContext.Rooms.FindAsync(Id);
            if (room == null)
                return false;

            _hotelDbContext.Rooms.Remove(room);
            await _hotelDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Domain.Entities.Room> Get(int Id)
        {
            return await _hotelDbContext.Rooms
                .Include(r => r.Bookings) 
                .Where(r => r.Id == Id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Room>> GetAll()
        {
            return await _hotelDbContext.Rooms.ToListAsync(); 
        }

        public async Task Update(Domain.Entities.Room room)
        {
            _hotelDbContext.Rooms.Update(room);
            await _hotelDbContext.SaveChangesAsync();
        }


    }
}
