using Domain.Entities;

namespace Domain.Ports
{
    public interface IGuestRepository
    {
        Task<Domain.Entities.Guest> Get(int Id);
        Task<IEnumerable<Domain.Entities.Guest>> GetAll();
        Task<int> Create(Domain.Entities.Guest guest);
        Task<bool> Delete(int id);

    }
}
