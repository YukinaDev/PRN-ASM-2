using AutoMapper;
using EVDMS.BusinessLogic.Application.Models.Planning;
using EVDMS.BusinessLogic.Application.Services;
using EVDMS.DataAccess.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EVDMS.WebApp.Pages.Admin.Approvals;

[Authorize(Roles = RoleNames.Admin)]
public class IndexModel : PageModel
{
    private readonly DistributionPlanService _distributionPlanService;
    private readonly DealerKpiService _dealerKpiService;
    private readonly IMapper _mapper;

    public IndexModel(
        DistributionPlanService distributionPlanService,
        DealerKpiService dealerKpiService,
        IMapper mapper)
    {
        _distributionPlanService = distributionPlanService;
        _dealerKpiService = dealerKpiService;
        _mapper = mapper;
    }

    public IReadOnlyList<DistributionPlanSummary> PendingPlans { get; private set; } = Array.Empty<DistributionPlanSummary>();
    public IReadOnlyList<DealerKpiPlanSummary> PendingKpis { get; private set; } = Array.Empty<DealerKpiPlanSummary>();

    public async Task OnGetAsync()
    {
        var plans = await _distributionPlanService.GetSubmittedPlansAsync();
        PendingPlans = _mapper.Map<List<DistributionPlanSummary>>(plans);

        var kpis = await _dealerKpiService.GetPlansWaitingApprovalAsync();
        PendingKpis = _mapper.Map<List<DealerKpiPlanSummary>>(kpis);
    }
}
