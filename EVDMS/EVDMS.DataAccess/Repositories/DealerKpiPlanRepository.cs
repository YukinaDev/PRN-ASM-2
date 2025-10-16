using EVDMS.DataAccess.Database;
using EVDMS.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace EVDMS.DataAccess.Repositories;

public class DealerKpiPlanRepository : Repository<DealerKpiPlan>, IDealerKpiPlanRepository
{
    public DealerKpiPlanRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<DealerKpiPlan>> GetAllWithDetailsAsync()
    {
        return await _context.DealerKpiPlans
            .Include(plan => plan.Dealer)
            .Include(plan => plan.CreatedBy)
            .Include(plan => plan.ApprovedBy)
            .OrderByDescending(plan => plan.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<DealerKpiPlan>> GetSubmittedPlansAsync()
    {
        return await _context.DealerKpiPlans
            .Where(plan => plan.Status == PlanStatus.Submitted)
            .Include(plan => plan.Dealer)
            .Include(plan => plan.CreatedBy)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<DealerKpiPlan>> GetPlansForDealerAsync(int dealerId)
    {
        return await _context.DealerKpiPlans
            .Where(plan => plan.DealerId == dealerId && plan.Status == PlanStatus.Approved)
            .Include(plan => plan.PerformanceLogs)
            .OrderByDescending(plan => plan.TargetStartDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<DealerKpiPlan?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.DealerKpiPlans
            .Include(plan => plan.Dealer)
            .Include(plan => plan.CreatedBy)
            .Include(plan => plan.ApprovedBy)
            .Include(plan => plan.PerformanceLogs)
                .ThenInclude(log => log.RecordedBy)
            .FirstOrDefaultAsync(plan => plan.Id == id);
    }
}
