using AutoMapper;
using EVDMS.BusinessLogic.Application.Models.Planning;
using EVDMS.BusinessLogic.Application.Services;
using EVDMS.DataAccess.Constants;
using EVDMS.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EVDMS.WebApp.Pages.DealerKpi;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly DealerKpiService _dealerKpiService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public DetailsModel(
        DealerKpiService dealerKpiService,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _dealerKpiService = dealerKpiService;
        _userManager = userManager;
        _mapper = mapper;
    }

    public DealerKpiPlanDetail? Plan { get; private set; }
    public bool CanSubmit { get; private set; }
    public bool CanApprove { get; private set; }
    public bool CanRecordActivity { get; private set; }

    [BindProperty]
    public DealerPerformanceLogInput ActivityInput { get; set; } = new();

    [BindProperty]
    public string? ApprovalComment { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var entity = await _dealerKpiService.GetPlanDetailAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        if (!await IsAccessibleAsync(entity))
        {
            return Forbid();
        }

        Plan = _mapper.Map<DealerKpiPlanDetail>(entity);
        SetPermissions(entity);
        ActivityInput.DealerKpiPlanId = entity.Id;
        ActivityInput.ActivityDate = DateTime.UtcNow.Date;
        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync(int id)
    {
        if (!User.IsInRole(RoleNames.Admin) && !User.IsInRole(RoleNames.EvmStaff))
        {
            return Forbid();
        }

        await _dealerKpiService.SubmitAsync(id);
        StatusMessage = "Đã gửi kế hoạch KPI tới Admin để phê duyệt.";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostApproveAsync(int id, bool approve)
    {
        if (!User.IsInRole(RoleNames.Admin))
        {
            return Forbid();
        }

        if (!approve && string.IsNullOrWhiteSpace(ApprovalComment))
        {
            ModelState.AddModelError(string.Empty, "Vui lòng cung cấp lý do khi từ chối kế hoạch.");
            await LoadPlanAsync(id);
            return Page();
        }

        var userId = _userManager.GetUserId(User)
            ?? throw new InvalidOperationException("Không xác định được người dùng hiện tại.");

        await _dealerKpiService.ApproveAsync(id, userId, approve, ApprovalComment);
        StatusMessage = approve
            ? "Đã phê duyệt kế hoạch KPI."
            : "Đã từ chối kế hoạch KPI. EVM Staff sẽ nhận thông báo.";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRecordActivityAsync(int id)
    {
        var entity = await _dealerKpiService.GetPlanDetailAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        if (!await IsAccessibleAsync(entity, requireDealerOwnership: true))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            Plan = _mapper.Map<DealerKpiPlanDetail>(entity);
            SetPermissions(entity);
            return Page();
        }

        if (ActivityInput.ActivityDate < entity.TargetStartDate || ActivityInput.ActivityDate > entity.TargetEndDate)
        {
            ModelState.AddModelError(string.Empty, "Ngày ghi nhận phải nằm trong khoảng thời gian của kế hoạch.");
            Plan = _mapper.Map<DealerKpiPlanDetail>(entity);
            SetPermissions(entity);
            return Page();
        }

        ActivityInput.DealerKpiPlanId = id;
        var record = _mapper.Map<DealerPerformanceLog>(ActivityInput);
        record.RecordedById = _userManager.GetUserId(User)
            ?? throw new InvalidOperationException("Không xác định được người dùng hiện tại.");
        await _dealerKpiService.RecordPerformanceAsync(record);

        StatusMessage = "Đã ghi nhận hoạt động bán hàng của đại lý.";
        return RedirectToPage(new { id });
    }

    private async Task LoadPlanAsync(int id)
    {
        var entity = await _dealerKpiService.GetPlanDetailAsync(id)
            ?? throw new InvalidOperationException("Kế hoạch không tồn tại.");
        Plan = _mapper.Map<DealerKpiPlanDetail>(entity);
        SetPermissions(entity);
    }

    private async Task<bool> IsAccessibleAsync(DealerKpiPlan entity, bool requireDealerOwnership = false)
    {
        if (User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.EvmStaff))
        {
            return true;
        }

        if (User.IsInRole(RoleNames.DealerManager) || User.IsInRole(RoleNames.DealerStaff))
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.DealerId == null)
            {
                return false;
            }

            var matchDealer = user.DealerId == entity.DealerId;
            return requireDealerOwnership ? matchDealer : matchDealer && entity.Status == PlanStatus.Approved;
        }

        return false;
    }

    private void SetPermissions(DealerKpiPlan entity)
    {
        CanSubmit = (User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.EvmStaff))
            && (entity.Status == PlanStatus.Draft || entity.Status == PlanStatus.Rejected);

        CanApprove = User.IsInRole(RoleNames.Admin) && entity.Status == PlanStatus.Submitted;

        CanRecordActivity = (User.IsInRole(RoleNames.DealerManager) || User.IsInRole(RoleNames.DealerStaff))
            && entity.Status == PlanStatus.Approved;
    }
}
