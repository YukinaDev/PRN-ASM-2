using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EVDMS.DataAccess.Entities;
using EVDMS.DataAccess.Database;
using Microsoft.EntityFrameworkCore;

namespace EVDMS.BusinessLogic.Application.Services;

public class DistributionPlanService
{
    private readonly ApplicationDbContext _context;

    public DistributionPlanService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<DistributionPlan>> GetPlansForEvmStaffAsync()
    {
        return _context.DistributionPlans
            .Include(plan => plan.CreatedBy)
            .Include(plan => plan.ApprovedBy)
            .OrderByDescending(plan => plan.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<List<DistributionPlan>> GetSubmittedPlansAsync()
    {
        return _context.DistributionPlans
            .Where(plan => plan.Status == PlanStatus.Submitted)
            .Include(plan => plan.CreatedBy)
            .Include(plan => plan.Lines)
                .ThenInclude(line => line.Dealer)
            .Include(plan => plan.Lines)
                .ThenInclude(line => line.VehicleModel)
            .OrderBy(plan => plan.TargetMonth)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<List<DistributionPlan>> GetApprovedPlansForDealerAsync(int dealerId)
    {
        return _context.DistributionPlanLines
            .Where(line => line.DealerId == dealerId && line.Plan!.Status == PlanStatus.Approved)
            .Select(line => line.Plan!)
            .Include(plan => plan.Lines)
                .ThenInclude(l => l.VehicleModel)
            .Include(plan => plan.ApprovedBy)
            .AsNoTracking()
            .Distinct()
            .OrderByDescending(plan => plan.TargetMonth)
            .ToListAsync();
    }

    public Task<DistributionPlan?> GetPlanDetailAsync(int id)
    {
        return _context.DistributionPlans
            .Include(plan => plan.CreatedBy)
            .Include(plan => plan.ApprovedBy)
            .Include(plan => plan.Lines)
                .ThenInclude(line => line.Dealer)
            .Include(plan => plan.Lines)
                .ThenInclude(line => line.VehicleModel)
            .FirstOrDefaultAsync(plan => plan.Id == id);
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

        _context.DistributionPlans.Add(plan);
        await _context.SaveChangesAsync();
        return plan.Id;
    }

    public async Task SubmitAsync(int planId)
    {
        var plan = await _context.DistributionPlans.FirstOrDefaultAsync(p => p.Id == planId);
        if (plan == null)
        {
            throw new InvalidOperationException("Không tìm thấy kế hoạch phân phối.");
        }

        if (plan.Status != PlanStatus.Draft && plan.Status != PlanStatus.Rejected)
        {
            throw new InvalidOperationException("Chỉ có thể gửi kế hoạch ở trạng thái nháp hoặc bị từ chối.");
        }

        plan.Status = PlanStatus.Submitted;
        plan.ApprovedById = null;
        plan.ApprovedAt = null;
        plan.RejectionReason = null;
        await _context.SaveChangesAsync();
    }

    public async Task ApproveAsync(int planId, string approverId, bool approve, string? reason)
    {
        var plan = await _context.DistributionPlans.FirstOrDefaultAsync(p => p.Id == planId);
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
        await _context.SaveChangesAsync();
    }
}
