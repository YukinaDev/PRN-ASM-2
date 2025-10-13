using AutoMapper;
using EVDMS.BusinessLogic.Application.Models.Planning;
using EVDMS.BusinessLogic.Application.Services;
using EVDMS.DataAccess.Constants;
using EVDMS.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EVDMS.WebApp.Pages.DistributionPlans;

[Authorize(Roles = RoleNames.Admin + "," + RoleNames.EvmStaff)]
public class CreateModel : PageModel
{
    private readonly DistributionPlanService _distributionPlanService;
    private readonly DealerAllocationService _dealerAllocationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public CreateModel(
        DistributionPlanService distributionPlanService,
        DealerAllocationService dealerAllocationService,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _distributionPlanService = distributionPlanService;
        _dealerAllocationService = dealerAllocationService;
        _userManager = userManager;
        _mapper = mapper;
    }

    [BindProperty]
    public DistributionPlanCreateModel Plan { get; set; } = new();

    public IReadOnlyList<SelectListItem> DealerOptions { get; private set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> VehicleOptions { get; private set; } = Array.Empty<SelectListItem>();

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadOptionsAsync();
        EnsureLinePlaceholders();
        return Page();
    }

    public async Task<IActionResult> OnPostAddLineAsync()
    {
        Plan.Lines.Add(new DistributionPlanLineInput());
        await LoadOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveLineAsync(int index)
    {
        if (index >= 0 && index < Plan.Lines.Count)
        {
            Plan.Lines.RemoveAt(index);
        }

        await LoadOptionsAsync();
        EnsureLinePlaceholders();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadOptionsAsync();

        Plan.Lines = Plan.Lines
            .Where(line => line.DealerId > 0 && line.VehicleModelId > 0 && line.PlannedQuantity > 0)
            .ToList();

        if (Plan.Lines.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Vui lòng thêm ít nhất một dòng phân bổ.");
        }

        if (!ModelState.IsValid)
        {
            EnsureLinePlaceholders();
            return Page();
        }

        var userId = _userManager.GetUserId(User) ?? throw new InvalidOperationException("Không xác định được người dùng hiện tại.");
        var planLines = Plan.Lines.Select(line => _mapper.Map<DistributionPlanLine>(line)).ToList();
        var planId = await _distributionPlanService.CreateDraftAsync(userId, Plan.PlanName, Plan.Description, Plan.TargetMonth, planLines);

        TempData["StatusMessage"] = "Đã tạo kế hoạch nháp. Vui lòng xem chi tiết và gửi phê duyệt.";
        return RedirectToPage("Details", new { id = planId });
    }

    private async Task LoadOptionsAsync()
    {
        var dealers = await _dealerAllocationService.GetActiveDealersAsync();
        DealerOptions = dealers
            .Select(dealer => new SelectListItem(dealer.Name, dealer.Id.ToString()))
            .ToList();

        var vehicles = await _dealerAllocationService.GetActiveVehicleModelsAsync();
        VehicleOptions = vehicles
            .Select(vehicle => new SelectListItem(vehicle.Name, vehicle.Id.ToString()))
            .ToList();
    }

    private void EnsureLinePlaceholders()
    {
        if (Plan.Lines.Count == 0)
        {
            Plan.Lines.Add(new DistributionPlanLineInput { PlannedQuantity = 10, DiscountRate = 5 });
            Plan.Lines.Add(new DistributionPlanLineInput { PlannedQuantity = 5, DiscountRate = 3 });
        }
    }
}
