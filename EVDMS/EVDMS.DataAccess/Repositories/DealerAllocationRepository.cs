using EVDMS.DataAccess.Database;
using EVDMS.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace EVDMS.DataAccess.Repositories;

public class DealerAllocationRepository : Repository<DealerAllocation>, IDealerAllocationRepository
{
    public DealerAllocationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<DealerAllocation>> GetAllocationsWithDetailsAsync(
        int? dealerId = null,
        int? vehicleModelId = null,
        AllocationStatus? status = null)
    {
        var query = _context.DealerAllocations
            .Include(allocation => allocation.Dealer)
            .Include(allocation => allocation.VehicleModel)
            .AsQueryable();

        if (dealerId.HasValue)
        {
            query = query.Where(allocation => allocation.DealerId == dealerId);
        }

        if (vehicleModelId.HasValue)
        {
            query = query.Where(allocation => allocation.VehicleModelId == vehicleModelId);
        }

        if (status.HasValue)
        {
            query = query.Where(allocation => allocation.Status == status);
        }

        return await query
            .OrderBy(allocation => allocation.Dealer!.Name)
            .ThenBy(allocation => allocation.VehicleModel!.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<DealerAllocation?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.DealerAllocations
            .Include(allocation => allocation.Dealer)
            .Include(allocation => allocation.VehicleModel)
            .FirstOrDefaultAsync(allocation => allocation.Id == id);
    }

    public async Task<List<DealerAllocation>> GetReorderAlertsAsync()
    {
        return await _context.DealerAllocations
            .Include(allocation => allocation.Dealer)
            .Include(allocation => allocation.VehicleModel)
            .Where(allocation => allocation.QuantityInStock <= allocation.ReorderPoint)
            .OrderBy(allocation => allocation.Dealer != null ? allocation.Dealer.Name : string.Empty)
            .ThenBy(allocation => allocation.VehicleModel != null ? allocation.VehicleModel.Name : string.Empty)
            .AsNoTracking()
            .ToListAsync();
    }
}
