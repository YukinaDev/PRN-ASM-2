using EVDMS.DataAccess.Entities;

namespace EVDMS.DataAccess.Repositories;

public interface IDistributionPlanRepository : IRepository<DistributionPlan>
{
    Task<List<DistributionPlan>> GetAllWithDetailsAsync();
    Task<List<DistributionPlan>> GetSubmittedPlansAsync();
    Task<List<DistributionPlan>> GetApprovedPlansForDealerAsync(int dealerId);
    Task<DistributionPlan?> GetByIdWithDetailsAsync(int id);
}
