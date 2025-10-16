using EVDMS.DataAccess.Entities;

namespace EVDMS.BusinessLogic.Application.Services;

public interface IDistributionPlanService
{
    Task<List<DistributionPlan>> GetPlansForEvmStaffAsync();
    Task<List<DistributionPlan>> GetSubmittedPlansAsync();
    Task<List<DistributionPlan>> GetApprovedPlansForDealerAsync(int dealerId);
    Task<DistributionPlan?> GetPlanDetailAsync(int id);
    Task<int> CreateDraftAsync(string userId, string planName, string? description, DateTime targetMonth, IEnumerable<DistributionPlanLine> lines);
    Task SubmitAsync(int planId);
    Task ApproveAsync(int planId, string approverId, bool approve, string? reason);
}
