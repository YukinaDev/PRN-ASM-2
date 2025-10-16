using EVDMS.DataAccess.Entities;

namespace EVDMS.DataAccess.Repositories;

public interface IUnitOfWork : IDisposable
{
    IVehicleModelRepository VehicleModels { get; }
    IDealerRepository Dealers { get; }
    IDealerAllocationRepository DealerAllocations { get; }
    IDistributionPlanRepository DistributionPlans { get; }
    IDealerKpiPlanRepository DealerKpiPlans { get; }
    IRepository<DealerPerformanceLog> PerformanceLogs { get; }
    
    Task<int> SaveChangesAsync();
}
