using EVDMS.DataAccess.Entities;
using EVDMS.DataAccess.Repositories;

namespace EVDMS.BusinessLogic.Application.Services;

public class DealerKpiService : IDealerKpiService
{
    private readonly IUnitOfWork _unitOfWork;

    public DealerKpiService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<DealerKpiPlan>> GetPlansForEvmStaffAsync()
    {
        return _unitOfWork.DealerKpiPlans.GetAllWithDetailsAsync();
    }

    public Task<List<DealerKpiPlan>> GetPlansWaitingApprovalAsync()
    {
        return _unitOfWork.DealerKpiPlans.GetSubmittedPlansAsync();
    }

    public Task<List<DealerKpiPlan>> GetPlansForDealerAsync(int dealerId)
    {
        return _unitOfWork.DealerKpiPlans.GetPlansForDealerAsync(dealerId);
    }

    public Task<DealerKpiPlan?> GetPlanDetailAsync(int id)
    {
        return _unitOfWork.DealerKpiPlans.GetByIdWithDetailsAsync(id);
    }

    public async Task<int> CreatePlanAsync(DealerKpiPlan plan)
    {
        plan.CreatedAt = DateTime.UtcNow;
        plan.TargetStartDate = plan.TargetStartDate.Date;
        plan.TargetEndDate = plan.TargetEndDate.Date;
        await _unitOfWork.DealerKpiPlans.AddAsync(plan);
        await _unitOfWork.SaveChangesAsync();
        return plan.Id;
    }

    public async Task SubmitAsync(int planId)
    {
        var plan = await _unitOfWork.DealerKpiPlans.GetByIdAsync(planId);
        if (plan == null)
        {
            throw new InvalidOperationException("Không tìm thấy kế hoạch KPI.");
        }

        plan.Status = PlanStatus.Submitted;
        plan.ApprovedById = null;
        plan.ApprovedAt = null;
        plan.RejectionReason = null;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ApproveAsync(int planId, string approverId, bool approve, string? reason)
    {
        var plan = await _unitOfWork.DealerKpiPlans.GetByIdAsync(planId);
        if (plan == null)
        {
            throw new InvalidOperationException("Không tìm thấy kế hoạch KPI.");
        }

        if (plan.Status != PlanStatus.Submitted)
        {
            throw new InvalidOperationException("Chỉ xử lý được kế hoạch đang chờ duyệt.");
        }

        plan.ApprovedById = approverId;
        plan.ApprovedAt = DateTime.UtcNow;
        plan.Status = approve ? PlanStatus.Approved : PlanStatus.Rejected;
        plan.RejectionReason = approve ? null : reason;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RecordPerformanceAsync(DealerPerformanceLog log)
    {
        log.ActivityDate = log.ActivityDate.Date;
        log.Revenue = Math.Round(log.Revenue, 2);
        log.InventoryTurnover = Math.Round(log.InventoryTurnover, 2);
        await _unitOfWork.PerformanceLogs.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();
    }
}
