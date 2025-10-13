using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EVDMS.DataAccess.Entities;
using EVDMS.DataAccess.Database;
using Microsoft.EntityFrameworkCore;

namespace EVDMS.BusinessLogic.Application.Services;

public class DealerKpiService
{
    private readonly ApplicationDbContext _context;

    public DealerKpiService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<DealerKpiPlan>> GetPlansForEvmStaffAsync()
    {
        return _context.DealerKpiPlans
            .Include(plan => plan.Dealer)
            .Include(plan => plan.CreatedBy)
            .Include(plan => plan.ApprovedBy)
            .OrderByDescending(plan => plan.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<List<DealerKpiPlan>> GetPlansWaitingApprovalAsync()
    {
        return _context.DealerKpiPlans
            .Where(plan => plan.Status == PlanStatus.Submitted)
            .Include(plan => plan.Dealer)
            .Include(plan => plan.CreatedBy)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<List<DealerKpiPlan>> GetPlansForDealerAsync(int dealerId)
    {
        return _context.DealerKpiPlans
            .Where(plan => plan.DealerId == dealerId && plan.Status == PlanStatus.Approved)
            .Include(plan => plan.PerformanceLogs)
            .OrderByDescending(plan => plan.TargetStartDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<DealerKpiPlan?> GetPlanDetailAsync(int id)
    {
        return _context.DealerKpiPlans
            .Include(plan => plan.Dealer)
            .Include(plan => plan.CreatedBy)
            .Include(plan => plan.ApprovedBy)
            .Include(plan => plan.PerformanceLogs)
                .ThenInclude(log => log.RecordedBy)
            .FirstOrDefaultAsync(plan => plan.Id == id);
    }

    public async Task<int> CreatePlanAsync(DealerKpiPlan plan)
    {
        plan.CreatedAt = DateTime.UtcNow;
        plan.TargetStartDate = plan.TargetStartDate.Date;
        plan.TargetEndDate = plan.TargetEndDate.Date;
        _context.DealerKpiPlans.Add(plan);
        await _context.SaveChangesAsync();
        return plan.Id;
    }

    public async Task SubmitAsync(int planId)
    {
        var plan = await _context.DealerKpiPlans.FirstOrDefaultAsync(p => p.Id == planId);
        if (plan == null)
        {
            throw new InvalidOperationException("Không tìm thấy kế hoạch KPI.");
        }

        plan.Status = PlanStatus.Submitted;
        plan.ApprovedById = null;
        plan.ApprovedAt = null;
        plan.RejectionReason = null;
        await _context.SaveChangesAsync();
    }

    public async Task ApproveAsync(int planId, string approverId, bool approve, string? reason)
    {
        var plan = await _context.DealerKpiPlans.FirstOrDefaultAsync(p => p.Id == planId);
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
        await _context.SaveChangesAsync();
    }

    public async Task RecordPerformanceAsync(DealerPerformanceLog log)
    {
        log.ActivityDate = log.ActivityDate.Date;
        log.Revenue = Math.Round(log.Revenue, 2);
        log.InventoryTurnover = Math.Round(log.InventoryTurnover, 2);
        _context.DealerPerformanceLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
