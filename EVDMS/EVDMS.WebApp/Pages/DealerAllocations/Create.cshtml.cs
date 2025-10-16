using AutoMapper;
using EVDMS.BusinessLogic.Models;
using EVDMS.BusinessLogic.Application.Services;
using EVDMS.DataAccess.Constants;
using EVDMS.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EVDMS.WebApp.Pages.DealerAllocations;

[Authorize(Roles = RoleNames.Admin + "," + RoleNames.EvmStaff)]
public class CreateModel : PageModel
{
    private readonly IDealerAllocationService _dealerAllocationService;
    private readonly IMapper _mapper;

    public CreateModel(IDealerAllocationService dealerAllocationService, IMapper mapper)
    {
        _dealerAllocationService = dealerAllocationService;
        _mapper = mapper;
    }

    [BindProperty]
    public DealerAllocationEditModel Allocation { get; set; } = new();

    public IReadOnlyList<SelectListItem> DealerOptions { get; private set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> VehicleModelOptions { get; private set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> StatusOptions { get; private set; } = Array.Empty<SelectListItem>();

    public async Task OnGetAsync()
    {
        await PopulateSelectListsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await PopulateSelectListsAsync();

        var existing = await _dealerAllocationService.GetAllocationsAsync(Allocation.DealerId, Allocation.VehicleModelId, null);
        if (existing.Any())
        {
            ModelState.AddModelError(string.Empty, "Đại lý này đã được phân bổ mẫu xe tương tự. Vui lòng cập nhật phân bổ hiện tại.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var entity = _mapper.Map<DealerAllocation>(Allocation);
        await _dealerAllocationService.CreateAsync(entity);

        TempData["StatusMessage"] = "Đã tạo phân bổ cho đại lý.";
        return RedirectToPage("Index");
    }

    private async Task PopulateSelectListsAsync()
    {
        DealerOptions = (await _dealerAllocationService.GetActiveDealersAsync())
            .Select(dealer => new SelectListItem(dealer.Name, dealer.Id.ToString(), dealer.Id == Allocation.DealerId))
            .ToList();

        VehicleModelOptions = (await _dealerAllocationService.GetActiveVehicleModelsAsync())
            .Select(vehicle => new SelectListItem(vehicle.Name, vehicle.Id.ToString(), vehicle.Id == Allocation.VehicleModelId))
            .ToList();

        StatusOptions = Enum.GetValues<AllocationStatus>()
            .Select(status => new SelectListItem(status.ToString(), status.ToString(), status == Allocation.Status))
            .ToList();
    }
}
