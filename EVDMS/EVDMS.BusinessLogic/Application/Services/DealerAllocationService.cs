using EVDMS.DataAccess.Entities;
using EVDMS.DataAccess.Repositories;

namespace EVDMS.BusinessLogic.Application.Services;

public class DealerAllocationService : IDealerAllocationService
{
    private readonly IUnitOfWork _unitOfWork;

    public DealerAllocationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<DealerAllocation>> GetAllocationsAsync(
        int? dealerId = null,
        int? vehicleModelId = null,
        AllocationStatus? status = null)
    {
        return _unitOfWork.DealerAllocations.GetAllocationsWithDetailsAsync(dealerId, vehicleModelId, status);
    }

    public Task<DealerAllocation?> FindAsync(int id)
    {
        return _unitOfWork.DealerAllocations.GetByIdWithDetailsAsync(id);
    }

    public async Task<DealerAllocation> CreateAsync(DealerAllocation allocation)
    {
        allocation.LastUpdated = DateTime.UtcNow;
        await _unitOfWork.DealerAllocations.AddAsync(allocation);
        await _unitOfWork.SaveChangesAsync();
        return allocation;
    }

    public async Task UpdateAsync(DealerAllocation allocation)
    {
        allocation.LastUpdated = DateTime.UtcNow;
        _unitOfWork.DealerAllocations.Update(allocation);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _unitOfWork.DealerAllocations.GetByIdAsync(id);
        if (entity is null)
        {
            return;
        }

        _unitOfWork.DealerAllocations.Remove(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public Task<List<Dealer>> GetActiveDealersAsync()
    {
        return _unitOfWork.Dealers.GetActiveDealersAsync();
    }

    public Task<List<VehicleModel>> GetActiveVehicleModelsAsync()
    {
        return _unitOfWork.VehicleModels.GetActiveModelsAsync();
    }

    public async Task<List<DealerInventoryAlert>> GetReorderAlertsAsync()
    {
        var allocations = await _unitOfWork.DealerAllocations.GetReorderAlertsAsync();
        
        return allocations.Select(allocation => new DealerInventoryAlert(
            allocation.DealerId,
            allocation.VehicleModelId,
            allocation.Dealer != null ? allocation.Dealer.Name : string.Empty,
            allocation.VehicleModel != null ? allocation.VehicleModel.Name : string.Empty,
            allocation.QuantityInStock,
            allocation.ReorderPoint))
        .ToList();
    }
}

public record DealerInventoryAlert(
    int DealerId,
    int VehicleModelId,
    string DealerName,
    string VehicleName,
    int QuantityInStock,
    int ReorderPoint);
