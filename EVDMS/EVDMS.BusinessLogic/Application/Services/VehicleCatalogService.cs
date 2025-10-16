using EVDMS.DataAccess.Entities;
using EVDMS.DataAccess.Repositories;

namespace EVDMS.BusinessLogic.Application.Services;

public class VehicleCatalogService : IVehicleCatalogService
{
    private readonly IUnitOfWork _unitOfWork;

    public VehicleCatalogService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<VehicleModel>> GetAllAsync(VehicleStatus? statusFilter = null)
    {
        return _unitOfWork.VehicleModels.GetAllByStatusAsync(statusFilter);
    }

    public Task<VehicleModel?> FindAsync(int id)
    {
        return _unitOfWork.VehicleModels.GetByIdAsync(id);
    }

    public async Task<VehicleModel> CreateAsync(VehicleModel model)
    {
        await _unitOfWork.VehicleModels.AddAsync(model);
        await _unitOfWork.SaveChangesAsync();
        return model;
    }

    public async Task UpdateAsync(VehicleModel model)
    {
        _unitOfWork.VehicleModels.Update(model);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _unitOfWork.VehicleModels.GetByIdAsync(id);
        if (entity is null)
        {
            return;
        }

        _unitOfWork.VehicleModels.Remove(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public Task<bool> ModelCodeExistsAsync(string modelCode, int? ignoreId = null)
    {
        return _unitOfWork.VehicleModels.ModelCodeExistsAsync(modelCode, ignoreId);
    }
}
