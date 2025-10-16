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
public class CreateModel : PageModel
{
    private readonly IVehicleCatalogService _vehicleCatalogService;
    private readonly IMapper _mapper;

    public CreateModel(IVehicleCatalogService vehicleCatalogService, IMapper mapper)
    {
        _vehicleCatalogService = vehicleCatalogService;
        _mapper = mapper;
    }

    [BindProperty]
    public VehicleModelEditModel Vehicle { get; set; } = new();

    public IReadOnlyList<SelectListItem> StatusOptions { get; private set; } = Array.Empty<SelectListItem>();

    public void OnGet()
    {
        StatusOptions = BuildStatusOptions();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        StatusOptions = BuildStatusOptions();

        if (await _vehicleCatalogService.ModelCodeExistsAsync(Vehicle.ModelCode))
        {
            ModelState.AddModelError($"{nameof(Vehicle)}.{nameof(Vehicle.ModelCode)}", "Mã mẫu xe đã tồn tại.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var entity = _mapper.Map<VehicleModel>(Vehicle);
        await _vehicleCatalogService.CreateAsync(entity);

        TempData["StatusMessage"] = $"Đã tạo mẫu xe {Vehicle.Name}.";
        return RedirectToPage("Index");
    }

    private static IReadOnlyList<SelectListItem> BuildStatusOptions()
    {
        return Enum.GetValues<VehicleStatus>()
            .Select(status => new SelectListItem(status.ToString(), status.ToString()))
            .ToList();
    }
}
