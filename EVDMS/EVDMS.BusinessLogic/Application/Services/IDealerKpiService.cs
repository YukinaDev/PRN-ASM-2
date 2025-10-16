using EVDMS.DataAccess.Entities;

namespace EVDMS.BusinessLogic.Application.Services;

public interface IDealerKpiService
{
    Task<List<DealerKpiPlan>> GetPlansForEvmStaffAsync();
    Task<List<DealerKpiPlan>> GetPlansWaitingApprovalAsync();
    Task<List<DealerKpiPlan>> GetPlansForDealerAsync(int dealerId);
    Task<DealerKpiPlan?> GetPlanDetailAsync(int id);
    Task<int> CreatePlanAsync(DealerKpiPlan plan);
    Task SubmitAsync(int planId);
    Task ApproveAsync(int planId, string approverId, bool approve, string? reason);
    Task RecordPerformanceAsync(DealerPerformanceLog log);
}
