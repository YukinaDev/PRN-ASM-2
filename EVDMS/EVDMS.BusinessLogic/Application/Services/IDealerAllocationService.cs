using EVDMS.DataAccess.Entities;

namespace EVDMS.BusinessLogic.Application.Services;

public interface IDealerAllocationService
{
    Task<List<DealerAllocation>> GetAllocationsAsync(
        int? dealerId = null,
        int? vehicleModelId = null,
        AllocationStatus? status = null);
    
    Task<DealerAllocation?> FindAsync(int id);
    Task<DealerAllocation> CreateAsync(DealerAllocation allocation);
    Task UpdateAsync(DealerAllocation allocation);
    Task DeleteAsync(int id);
    Task<List<Dealer>> GetActiveDealersAsync();
    Task<List<VehicleModel>> GetActiveVehicleModelsAsync();
    Task<List<DealerInventoryAlert>> GetReorderAlertsAsync();
}
