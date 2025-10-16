using EVDMS.DataAccess.Database;
using EVDMS.DataAccess.Entities;

namespace EVDMS.DataAccess.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    
    public IVehicleModelRepository VehicleModels { get; }
    public IDealerRepository Dealers { get; }
    public IDealerAllocationRepository DealerAllocations { get; }
    public IDistributionPlanRepository DistributionPlans { get; }
    public IDealerKpiPlanRepository DealerKpiPlans { get; }
    public IRepository<DealerPerformanceLog> PerformanceLogs { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        VehicleModels = new VehicleModelRepository(_context);
        Dealers = new DealerRepository(_context);
        DealerAllocations = new DealerAllocationRepository(_context);
        DistributionPlans = new DistributionPlanRepository(_context);
        DealerKpiPlans = new DealerKpiPlanRepository(_context);
        PerformanceLogs = new Repository<DealerPerformanceLog>(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
