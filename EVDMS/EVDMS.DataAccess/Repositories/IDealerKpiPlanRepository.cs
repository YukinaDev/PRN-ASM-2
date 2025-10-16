using EVDMS.DataAccess.Entities;

namespace EVDMS.DataAccess.Repositories;

public interface IDealerKpiPlanRepository : IRepository<DealerKpiPlan>
{
    Task<List<DealerKpiPlan>> GetAllWithDetailsAsync();
    Task<List<DealerKpiPlan>> GetSubmittedPlansAsync();
    Task<List<DealerKpiPlan>> GetPlansForDealerAsync(int dealerId);
    Task<DealerKpiPlan?> GetByIdWithDetailsAsync(int id);
}
