using AutoMapper;
using EVDMS.BusinessLogic.Application.Models.Planning;
using EVDMS.BusinessLogic.Application.Services;
using EVDMS.DataAccess.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EVDMS.WebApp.Pages.DistributionPlans;

[Authorize(Roles = RoleNames.Admin + "," + RoleNames.EvmStaff)]
public class IndexModel : PageModel
{
    private readonly DistributionPlanService _distributionPlanService;
    private readonly IMapper _mapper;

    public IndexModel(DistributionPlanService distributionPlanService, IMapper mapper)
    {
        _distributionPlanService = distributionPlanService;
        _mapper = mapper;
    }

    public IReadOnlyList<DistributionPlanSummary> Plans { get; private set; } = Array.Empty<DistributionPlanSummary>();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        var entities = await _distributionPlanService.GetPlansForEvmStaffAsync();
        Plans = _mapper.Map<List<DistributionPlanSummary>>(entities);
    }
}
