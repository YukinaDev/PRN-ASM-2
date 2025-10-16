using AutoMapper;
using EVDMS.BusinessLogic.Models;
using EVDMS.BusinessLogic.Application.Services;
using EVDMS.DataAccess.Constants;
using EVDMS.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EVDMS.WebApp.Pages.VehicleModels;

[Authorize(Roles = RoleNames.Admin + "," + RoleNames.EvmStaff)]
public class EditModel : PageModel
{
    private readonly IVehicleCatalogService _vehicleCatalogService;
    private readonly IMapper _mapper;

    public EditModel(IVehicleCatalogService vehicleCatalogService, IMapper mapper)
    {
        _vehicleCatalogService = vehicleCatalogService;
        _mapper = mapper;
    }

    [BindProperty]
    public VehicleModelEditModel Vehicle { get; set; } = new();

    public IReadOnlyList<SelectListItem> StatusOptions { get; private set; } = Array.Empty<SelectListItem>();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var entity = await _vehicleCatalogService.FindAsync(id);
        if (entity is null)
        {
            return NotFound();
        }

        Vehicle = _mapper.Map<VehicleModelEditModel>(entity);
        StatusOptions = BuildStatusOptions();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        StatusOptions = BuildStatusOptions();

        if (id != Vehicle.Id)
        {
            return BadRequest();
        }

        if (await _vehicleCatalogService.ModelCodeExistsAsync(Vehicle.ModelCode, id))
        {
            ModelState.AddModelError($"{nameof(Vehicle)}.{nameof(Vehicle.ModelCode)}", "Mã mẫu xe đã tồn tại.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var entity = _mapper.Map<VehicleModel>(Vehicle);
        await _vehicleCatalogService.UpdateAsync(entity);

        TempData["StatusMessage"] = $"Đã cập nhật mẫu xe {Vehicle.Name}.";
        return RedirectToPage("Index");
    }

    private static IReadOnlyList<SelectListItem> BuildStatusOptions()
    {
        return Enum.GetValues<VehicleStatus>()
            .Select(status => new SelectListItem(status.ToString(), status.ToString()))
            .ToList();
    }
}
