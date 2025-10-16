using AutoMapper;
using EVDMS.BusinessLogic.Application.Models.Planning;
using EVDMS.BusinessLogic.Application.Services;
using EVDMS.DataAccess.Constants;
using EVDMS.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EVDMS.WebApp.Pages.DealerKpi;

[Authorize(Roles = RoleNames.DealerManager + "," + RoleNames.DealerStaff)]
public class DealerProgressModel : PageModel
{
    private readonly IDealerKpiService _dealerKpiService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public DealerProgressModel(
        IDealerKpiService dealerKpiService,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _dealerKpiService = dealerKpiService;
        _userManager = userManager;
        _mapper = mapper;
    }

    public List<DealerKpiPlanDetail> Plans { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User)
            ?? throw new InvalidOperationException("Không tìm thấy người dùng.");

        if (user.DealerId is null)
        {
            Plans = new();
            return;
        }

        var entities = await _dealerKpiService.GetPlansForDealerAsync(user.DealerId.Value);
        Plans = entities
            .Select(entity =>
            {
                var detail = _mapper.Map<DealerKpiPlanDetail>(entity);
                detail.PerformanceLogs = entity.PerformanceLogs
                    .OrderByDescending(log => log.ActivityDate)
                    .Select(log => _mapper.Map<DealerPerformanceLogModel>(log))
                    .ToList();
                return detail;
            })
            .ToList();
    }
}
