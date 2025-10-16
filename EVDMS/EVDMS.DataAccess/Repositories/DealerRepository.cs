using EVDMS.DataAccess.Database;
using EVDMS.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace EVDMS.DataAccess.Repositories;

public class DealerRepository : Repository<Dealer>, IDealerRepository
{
    public DealerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Dealer>> GetActiveDealersAsync()
    {
        return await _context.Dealers
            .Where(dealer => dealer.IsActive)
            .OrderBy(dealer => dealer.Name)
            .AsNoTracking()
            .ToListAsync();
    }
}
