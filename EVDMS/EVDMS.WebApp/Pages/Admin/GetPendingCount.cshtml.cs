using EVDMS.BusinessLogic.Application.Services;
using EVDMS.DataAccess.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EVDMS.WebApp.Pages.Admin;

[Authorize(Roles = RoleNames.Admin)]
public class GetPendingCountModel : PageModel
{
    private readonly IDistributionPlanService _distributionPlanService;
    private readonly IDealerKpiService _dealerKpiService;

    public GetPendingCountModel(
        IDistributionPlanService distributionPlanService,
        IDealerKpiService dealerKpiService)
    {
        _distributionPlanService = distributionPlanService;
        _dealerKpiService = dealerKpiService;
    }

    public int PendingCount { get; set; }

    public async Task OnGetAsync()
    {
        var distributionPlans = await _distributionPlanService.GetSubmittedPlansAsync();
        var kpiPlans = await _dealerKpiService.GetPlansWaitingApprovalAsync();
        
        PendingCount = distributionPlans.Count + kpiPlans.Count;
    }
}
