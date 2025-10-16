using EVDMS.DataAccess.Database;
using EVDMS.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace EVDMS.DataAccess.Repositories;

public class VehicleModelRepository : Repository<VehicleModel>, IVehicleModelRepository
{
    public VehicleModelRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<VehicleModel>> GetAllByStatusAsync(VehicleStatus? statusFilter = null)
    {
        var query = _context.VehicleModels.AsQueryable();

        if (statusFilter.HasValue)
        {
            query = query.Where(model => model.Status == statusFilter);
        }

        return await query
            .OrderBy(model => model.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> ModelCodeExistsAsync(string modelCode, int? ignoreId = null)
    {
        var query = _context.VehicleModels.AsQueryable();

        if (ignoreId.HasValue)
        {
            query = query.Where(m => m.Id != ignoreId.Value);
        }

        return await query.AnyAsync(m => m.ModelCode == modelCode);
    }

    public async Task<List<VehicleModel>> GetActiveModelsAsync()
    {
        return await _context.VehicleModels
            .Where(model => model.Status == VehicleStatus.Active)
            .OrderBy(model => model.Name)
            .AsNoTracking()
            .ToListAsync();
    }
}
