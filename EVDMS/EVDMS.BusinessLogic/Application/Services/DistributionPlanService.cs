using EVDMS.DataAccess.Entities;
using EVDMS.DataAccess.Repositories;

namespace EVDMS.BusinessLogic.Application.Services;

public class DistributionPlanService : IDistributionPlanService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public DistributionPlanService(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public Task<List<DistributionPlan>> GetPlansForEvmStaffAsync()
    {
        return _unitOfWork.DistributionPlans.GetAllWithDetailsAsync();
    }

    public Task<List<DistributionPlan>> GetSubmittedPlansAsync()
    {
        return _unitOfWork.DistributionPlans.GetSubmittedPlansAsync();
    }

    public Task<List<DistributionPlan>> GetApprovedPlansForDealerAsync(int dealerId)
    {
        return _unitOfWork.DistributionPlans.GetApprovedPlansForDealerAsync(dealerId);
    }

    public Task<DistributionPlan?> GetPlanDetailAsync(int id)
    {
        return _unitOfWork.DistributionPlans.GetByIdWithDetailsAsync(id);
    }

    public async Task<int> CreateDraftAsync(string userId, string planName, string? description, DateTime targetMonth, IEnumerable<DistributionPlanLine> lines)
    {
        var plan = new DistributionPlan
        {
            PlanName = planName,
            Description = description,
            TargetMonth = new DateTime(targetMonth.Year, targetMonth.Month, 1),
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow,
            Status = PlanStatus.Draft,
            Lines = lines.ToList()
        };

        await _unitOfWork.DistributionPlans.AddAsync(plan);
        await _unitOfWork.SaveChangesAsync();
        return plan.Id;
    }

    public async Task SubmitAsync(int planId)
    {
        var plan = await _unitOfWork.DistributionPlans.GetByIdAsync(planId);
        if (plan == null)
        {
            throw new InvalidOperationException("Không tìm thấy kế hoạch phân phối.");
        }

        if (plan.Status != PlanStatus.Draft)
        {
            throw new InvalidOperationException("Chỉ có thể gửi kế hoạch ở trạng thái nháp. Kế hoạch đã được phê duyệt hoặc từ chối không thể gửi lại.");
        }

        plan.Status = PlanStatus.Submitted;
        plan.ApprovedById = null;
        plan.ApprovedAt = null;
        plan.RejectionReason = null;
        await _unitOfWork.SaveChangesAsync();

        if (!string.IsNullOrEmpty(plan.CreatedById))
        {
            await _notificationService.NotifyPlanSubmittedAsync(
                "DistributionPlan",
                plan.Id,
                plan.PlanName,
                plan.CreatedById);
        }
    }

    public async Task ApproveAsync(int planId, string approverId, bool approve, string? reason)
    {
        var plan = await _unitOfWork.DistributionPlans.GetByIdAsync(planId);
        if (plan == null)
        {
            throw new InvalidOperationException("Không tìm thấy kế hoạch phân phối.");
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

        if (!string.IsNullOrEmpty(plan.CreatedById))
        {
            await _notificationService.NotifyPlanApprovedAsync(
                "DistributionPlan",
                plan.Id,
                plan.PlanName,
                plan.CreatedById,
                approve,
                reason);
        }
    }
}
