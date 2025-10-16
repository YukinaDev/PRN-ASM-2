using EVDMS.DataAccess.Entities;

namespace EVDMS.DataAccess.Repositories;

public interface IDealerAllocationRepository : IRepository<DealerAllocation>
{
    Task<List<DealerAllocation>> GetAllocationsWithDetailsAsync(
        int? dealerId = null,
        int? vehicleModelId = null,
        AllocationStatus? status = null);
    
    Task<DealerAllocation?> GetByIdWithDetailsAsync(int id);
    Task<List<DealerAllocation>> GetReorderAlertsAsync();
}
