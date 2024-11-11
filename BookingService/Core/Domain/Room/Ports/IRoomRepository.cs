namespace Domain.Ports
{
    public interface IRoomRepository
    {
        Task<Domain.Entities.Room> Get(int Id);
        Task<int> Create(Domain.Entities.Room room);
        Task<IEnumerable<Domain.Entities.Room>> GetAll();
        Task<bool> Delete(int Id);
        Task Update(Domain.Entities.Room room);
    }
}
