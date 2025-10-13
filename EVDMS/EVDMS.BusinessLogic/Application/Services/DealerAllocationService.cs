using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EVDMS.DataAccess.Entities;
using EVDMS.DataAccess.Database;
using Microsoft.EntityFrameworkCore;

namespace EVDMS.BusinessLogic.Application.Services;

public class DealerAllocationService
{
    private readonly ApplicationDbContext _context;

    public DealerAllocationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DealerAllocation>> GetAllocationsAsync(
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

    public Task<DealerAllocation?> FindAsync(int id)
    {
        return _context.DealerAllocations
            .Include(allocation => allocation.Dealer)
            .Include(allocation => allocation.VehicleModel)
            .FirstOrDefaultAsync(allocation => allocation.Id == id);
    }

    public async Task<DealerAllocation> CreateAsync(DealerAllocation allocation)
    {
        allocation.LastUpdated = DateTime.UtcNow;
        _context.DealerAllocations.Add(allocation);
        await _context.SaveChangesAsync();
        return allocation;
    }

    public async Task UpdateAsync(DealerAllocation allocation)
    {
        allocation.LastUpdated = DateTime.UtcNow;
        _context.DealerAllocations.Update(allocation);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.DealerAllocations.FindAsync(id);
        if (entity is null)
        {
            return;
        }

        _context.DealerAllocations.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public Task<List<Dealer>> GetActiveDealersAsync()
    {
        return _context.Dealers
            .Where(dealer => dealer.IsActive)
            .OrderBy(dealer => dealer.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<List<VehicleModel>> GetActiveVehicleModelsAsync()
    {
        return _context.VehicleModels
            .Where(model => model.Status == VehicleStatus.Active)
            .OrderBy(model => model.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<List<DealerInventoryAlert>> GetReorderAlertsAsync()
    {
        return _context.DealerAllocations
            .Where(allocation => allocation.QuantityInStock <= allocation.ReorderPoint)
            .OrderBy(allocation => allocation.Dealer != null ? allocation.Dealer.Name : string.Empty)
            .ThenBy(allocation => allocation.VehicleModel != null ? allocation.VehicleModel.Name : string.Empty)
            .Select(allocation => new DealerInventoryAlert(
                allocation.DealerId,
                allocation.VehicleModelId,
                allocation.Dealer != null ? allocation.Dealer.Name : string.Empty,
                allocation.VehicleModel != null ? allocation.VehicleModel.Name : string.Empty,
                allocation.QuantityInStock,
                allocation.ReorderPoint))
            .ToListAsync();
    }
}

public record DealerInventoryAlert(
    int DealerId,
    int VehicleModelId,
    string DealerName,
    string VehicleName,
    int QuantityInStock,
    int ReorderPoint);
