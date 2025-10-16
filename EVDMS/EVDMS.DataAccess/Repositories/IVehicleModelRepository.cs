using EVDMS.DataAccess.Entities;

namespace EVDMS.DataAccess.Repositories;

public interface IVehicleModelRepository : IRepository<VehicleModel>
{
    Task<List<VehicleModel>> GetAllByStatusAsync(VehicleStatus? statusFilter = null);
    Task<bool> ModelCodeExistsAsync(string modelCode, int? ignoreId = null);
    Task<List<VehicleModel>> GetActiveModelsAsync();
}
