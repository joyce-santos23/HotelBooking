using Domain.Entities;

namespace Domain.Ports
{
    public interface IGuestRepository
    {
        Task<Domain.Entities.Guest> Get(int Id);
        Task<int> Create(Domain.Entities.Guest guest);

    }
}
