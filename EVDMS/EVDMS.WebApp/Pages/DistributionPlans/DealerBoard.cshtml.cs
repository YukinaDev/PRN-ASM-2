using AutoMapper;
using EVDMS.BusinessLogic.Application.Models.Planning;
using EVDMS.BusinessLogic.Application.Services;
using EVDMS.DataAccess.Constants;
using EVDMS.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EVDMS.WebApp.Pages.DistributionPlans;

[Authorize(Roles = RoleNames.DealerManager + "," + RoleNames.DealerStaff)]
public class DealerBoardModel : PageModel
{
    private readonly IDistributionPlanService _distributionPlanService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public DealerBoardModel(
        IDistributionPlanService distributionPlanService,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _distributionPlanService = distributionPlanService;
        _userManager = userManager;
        _mapper = mapper;
    }

    public List<DealerPlanViewModel> Plans { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User)
            ?? throw new InvalidOperationException("Không tìm thấy thông tin người dùng.");

        if (user.DealerId is null)
        {
            Plans = new();
            return;
        }

        var entities = await _distributionPlanService.GetApprovedPlansForDealerAsync(user.DealerId.Value);
        Plans = entities
            .Select(plan => new DealerPlanViewModel
            {
                PlanId = plan.Id,
                PlanName = plan.PlanName,
                TargetMonth = plan.TargetMonth,
                ApprovedAt = plan.ApprovedAt,
                ApprovedBy = plan.ApprovedBy?.DisplayName,
                Lines = plan.Lines
                    .Where(line => line.DealerId == user.DealerId)
                    .Select(line => _mapper.Map<DistributionPlanLineModel>(line))
                    .ToList()
            })
            .OrderByDescending(plan => plan.TargetMonth)
            .ToList();
    }

    public class DealerPlanViewModel
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public DateTime TargetMonth { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }
        public List<DistributionPlanLineModel> Lines { get; set; } = new();
    }
}
