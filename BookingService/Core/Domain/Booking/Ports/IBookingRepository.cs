namespace Domain.Ports
{
    public interface IBookingRepository
    {
        Task<Domain.Entities.Booking> Get(int Id);
        Task<int> Create(Domain.Entities.Booking booking);
        Task<IEnumerable<Domain.Entities.Booking>> GetAll();
        Task<bool> Delete(int Id);
    }
}
