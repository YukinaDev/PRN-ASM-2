using AutoMapper;
using EVDMS.BusinessLogic.Application.Models.Planning;
using EVDMS.BusinessLogic.Application.Services;
using EVDMS.DataAccess.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EVDMS.WebApp.Pages.DealerKpi;

[Authorize(Roles = RoleNames.Admin + "," + RoleNames.EvmStaff)]
public class IndexModel : PageModel
{
    private readonly DealerKpiService _dealerKpiService;
    private readonly IMapper _mapper;

    public IndexModel(DealerKpiService dealerKpiService, IMapper mapper)
    {
        _dealerKpiService = dealerKpiService;
        _mapper = mapper;
    }

    public IReadOnlyList<DealerKpiPlanSummary> Plans { get; private set; } = Array.Empty<DealerKpiPlanSummary>();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        var entities = await _dealerKpiService.GetPlansForEvmStaffAsync();
        Plans = _mapper.Map<List<DealerKpiPlanSummary>>(entities);
    }
}
