using EVDMS.DataAccess.Database;
using EVDMS.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace EVDMS.DataAccess.Repositories;

public class DistributionPlanRepository : Repository<DistributionPlan>, IDistributionPlanRepository
{
    public DistributionPlanRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<DistributionPlan>> GetAllWithDetailsAsync()
    {
        return await _context.DistributionPlans
            .Include(plan => plan.CreatedBy)
            .Include(plan => plan.ApprovedBy)
            .OrderByDescending(plan => plan.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<DistributionPlan>> GetSubmittedPlansAsync()
    {
        return await _context.DistributionPlans
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

    public async Task<List<DistributionPlan>> GetApprovedPlansForDealerAsync(int dealerId)
    {
        return await _context.DistributionPlans
            .Where(plan => plan.Status == PlanStatus.Approved && plan.Lines.Any(line => line.DealerId == dealerId))
            .Include(plan => plan.Lines)
                .ThenInclude(l => l.VehicleModel)
            .Include(plan => plan.ApprovedBy)
            .AsNoTracking()
            .OrderByDescending(plan => plan.TargetMonth)
            .ToListAsync();
    }

    public async Task<DistributionPlan?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.DistributionPlans
            .Include(plan => plan.CreatedBy)
            .Include(plan => plan.ApprovedBy)
            .Include(plan => plan.Lines)
                .ThenInclude(line => line.Dealer)
            .Include(plan => plan.Lines)
                .ThenInclude(line => line.VehicleModel)
            .FirstOrDefaultAsync(plan => plan.Id == id);
    }
}
