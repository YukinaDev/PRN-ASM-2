using AutoMapper;
using EVDMS.BusinessLogic.Application.Models.Planning;
using EVDMS.BusinessLogic.Application.Services;
using EVDMS.DataAccess.Constants;
using EVDMS.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EVDMS.WebApp.Pages.DistributionPlans;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly DistributionPlanService _distributionPlanService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public DetailsModel(
        DistributionPlanService distributionPlanService,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _distributionPlanService = distributionPlanService;
        _userManager = userManager;
        _mapper = mapper;
    }

    public DistributionPlanDetail? Plan { get; private set; }
    public bool CanSubmit { get; private set; }
    public bool CanApprove { get; private set; }
    public bool CanViewDealerDetail { get; private set; }

    [BindProperty]
    public string? ApprovalComment { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var entity = await _distributionPlanService.GetPlanDetailAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        if (!await IsAccessibleAsync(entity))
        {
            return Forbid();
        }

        Plan = _mapper.Map<DistributionPlanDetail>(entity);
        SetPermissions(entity);
        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync(int id)
    {
        if (!User.IsInRole(RoleNames.Admin) && !User.IsInRole(RoleNames.EvmStaff))
        {
            return Forbid();
        }

        await _distributionPlanService.SubmitAsync(id);
        StatusMessage = "Đã gửi kế hoạch tới Admin để phê duyệt.";
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
            ModelState.AddModelError(string.Empty, "Vui lòng ghi rõ lý do khi từ chối kế hoạch.");
            await LoadPlanAsync(id);
            return Page();
        }

        var userId = _userManager.GetUserId(User)
            ?? throw new InvalidOperationException("Không xác định được người dùng hiện tại.");

        await _distributionPlanService.ApproveAsync(id, userId, approve, ApprovalComment);
        StatusMessage = approve
            ? "Đã phê duyệt kế hoạch và kích hoạt phân bổ cho đại lý."
            : "Đã từ chối kế hoạch. EVM Staff sẽ nhận thông báo để cập nhật.";

        return RedirectToPage(new { id });
    }

    private async Task LoadPlanAsync(int id)
    {
        var entity = await _distributionPlanService.GetPlanDetailAsync(id)
            ?? throw new InvalidOperationException("Kế hoạch không tồn tại.");
        Plan = _mapper.Map<DistributionPlanDetail>(entity);
        SetPermissions(entity);
    }

    private async Task<bool> IsAccessibleAsync(DistributionPlan entity)
    {
        if (User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.EvmStaff))
        {
            return true;
        }

        if ((User.IsInRole(RoleNames.DealerManager) || User.IsInRole(RoleNames.DealerStaff))
            && entity.Status == PlanStatus.Approved)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return currentUser?.DealerId != null && entity.Lines.Any(line => line.DealerId == currentUser.DealerId);
        }

        return false;
    }

    private void SetPermissions(DistributionPlan entity)
    {
        CanSubmit = (User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.EvmStaff))
            && (entity.Status == PlanStatus.Draft || entity.Status == PlanStatus.Rejected);

        CanApprove = User.IsInRole(RoleNames.Admin) && entity.Status == PlanStatus.Submitted;
        CanViewDealerDetail = User.IsInRole(RoleNames.DealerManager) || User.IsInRole(RoleNames.DealerStaff);
    }
}
