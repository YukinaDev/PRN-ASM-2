using EVDMS.DataAccess.Entities;

namespace EVDMS.DataAccess.Repositories;

public interface IDealerRepository : IRepository<Dealer>
{
    Task<List<Dealer>> GetActiveDealersAsync();
}
