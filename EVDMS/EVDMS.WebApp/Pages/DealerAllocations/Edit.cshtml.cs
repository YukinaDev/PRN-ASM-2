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
public class EditModel : PageModel
{
    private readonly DealerAllocationService _dealerAllocationService;
    private readonly IMapper _mapper;

    public EditModel(DealerAllocationService dealerAllocationService, IMapper mapper)
    {
        _dealerAllocationService = dealerAllocationService;
        _mapper = mapper;
    }

    [BindProperty]
    public DealerAllocationEditModel Allocation { get; set; } = new();

    public IReadOnlyList<SelectListItem> DealerOptions { get; private set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> VehicleModelOptions { get; private set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> StatusOptions { get; private set; } = Array.Empty<SelectListItem>();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var entity = await _dealerAllocationService.FindAsync(id);
        if (entity is null)
        {
            return NotFound();
        }

        Allocation = _mapper.Map<DealerAllocationEditModel>(entity);
        await PopulateSelectListsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (id != Allocation.Id)
        {
            return BadRequest();
        }

        await PopulateSelectListsAsync();

        var existing = await _dealerAllocationService.GetAllocationsAsync(Allocation.DealerId, Allocation.VehicleModelId, null);
        if (existing.Any(entry => entry.Id != Allocation.Id))
        {
            ModelState.AddModelError(string.Empty, "Đại lý này đã có phân bổ cho mẫu xe này. Hãy cập nhật phân bổ hiện có.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var entity = _mapper.Map<DealerAllocation>(Allocation);
        await _dealerAllocationService.UpdateAsync(entity);

        TempData["StatusMessage"] = "Đã cập nhật phân bổ đại lý.";
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
