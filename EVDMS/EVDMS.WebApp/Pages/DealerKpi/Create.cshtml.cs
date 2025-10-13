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

namespace EVDMS.WebApp.Pages.DealerKpi;

[Authorize(Roles = RoleNames.Admin + "," + RoleNames.EvmStaff)]
public class CreateModel : PageModel
{
    private readonly DealerKpiService _dealerKpiService;
    private readonly DealerAllocationService _dealerAllocationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public CreateModel(
        DealerKpiService dealerKpiService,
        DealerAllocationService dealerAllocationService,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _dealerKpiService = dealerKpiService;
        _dealerAllocationService = dealerAllocationService;
        _userManager = userManager;
        _mapper = mapper;
    }

    [BindProperty]
    public DealerKpiPlanCreateModel Plan { get; set; } = new();

    public IReadOnlyList<SelectListItem> DealerOptions { get; private set; } = Array.Empty<SelectListItem>();

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadDealersAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadDealersAsync();
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Plan.TargetEndDate < Plan.TargetStartDate)
        {
            ModelState.AddModelError(string.Empty, "Ngày kết thúc phải lớn hơn ngày bắt đầu.");
            return Page();
        }

        var entity = _mapper.Map<DealerKpiPlan>(Plan);
        entity.CreatedById = _userManager.GetUserId(User)
            ?? throw new InvalidOperationException("Không xác định được người dùng hiện tại.");

        await _dealerKpiService.CreatePlanAsync(entity);
        TempData["StatusMessage"] = "Đã tạo kế hoạch KPI nháp. Gửi phê duyệt sau khi hoàn tất.";
        return RedirectToPage("Details", new { id = entity.Id });
    }

    private async Task LoadDealersAsync()
    {
        var dealers = await _dealerAllocationService.GetActiveDealersAsync();
        DealerOptions = dealers
            .Select(dealer => new SelectListItem(dealer.Name, dealer.Id.ToString()))
            .ToList();
    }
}
