using EVDMS.DataAccess.Entities;

namespace EVDMS.BusinessLogic.Application.Services;

public interface IVehicleCatalogService
{
    Task<List<VehicleModel>> GetAllAsync(VehicleStatus? statusFilter = null);
    Task<VehicleModel?> FindAsync(int id);
    Task<VehicleModel> CreateAsync(VehicleModel model);
    Task UpdateAsync(VehicleModel model);
    Task DeleteAsync(int id);
    Task<bool> ModelCodeExistsAsync(string modelCode, int? ignoreId = null);
}
