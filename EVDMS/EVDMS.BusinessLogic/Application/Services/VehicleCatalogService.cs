using System.Collections.Generic;
using System.Linq;
using EVDMS.DataAccess.Entities;
using EVDMS.DataAccess.Database;
using Microsoft.EntityFrameworkCore;

namespace EVDMS.BusinessLogic.Application.Services;

public class VehicleCatalogService
{
    private readonly ApplicationDbContext _context;

    public VehicleCatalogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<VehicleModel>> GetAllAsync(VehicleStatus? statusFilter = null)
    {
        var query = _context.VehicleModels.AsQueryable();

        if (statusFilter.HasValue)
        {
            query = query.Where(model => model.Status == statusFilter);
        }

        return query
            .OrderBy(model => model.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<VehicleModel?> FindAsync(int id)
    {
        return _context.VehicleModels.FirstOrDefaultAsync(model => model.Id == id);
    }

    public async Task<VehicleModel> CreateAsync(VehicleModel model)
    {
        _context.VehicleModels.Add(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public async Task UpdateAsync(VehicleModel model)
    {
        _context.VehicleModels.Update(model);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.VehicleModels.FindAsync(id);
        if (entity is null)
        {
            return;
        }

        _context.VehicleModels.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public Task<bool> ModelCodeExistsAsync(string modelCode, int? ignoreId = null)
    {
        var query = _context.VehicleModels.AsQueryable();

        if (ignoreId.HasValue)
        {
            query = query.Where(m => m.Id != ignoreId.Value);
        }

        return query.AnyAsync(m => m.ModelCode == modelCode);
    }
}
