using AutoMapper;
using EVDMS.BusinessLogic.Models;
using EVDMS.BusinessLogic.Application.Services;
using EVDMS.DataAccess.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EVDMS.WebApp.Pages.VehicleModels;

[Authorize(Roles = RoleNames.Admin + "," + RoleNames.EvmStaff)]
public class DeleteModel : PageModel
{
    private readonly IVehicleCatalogService _vehicleCatalogService;
    private readonly IMapper _mapper;

    public DeleteModel(IVehicleCatalogService vehicleCatalogService, IMapper mapper)
    {
        _vehicleCatalogService = vehicleCatalogService;
        _mapper = mapper;
    }

    public VehicleModelListItem? Vehicle { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var entity = await _vehicleCatalogService.FindAsync(id);
        if (entity is null)
        {
            return NotFound();
        }

        Vehicle = _mapper.Map<VehicleModelListItem>(entity);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        await _vehicleCatalogService.DeleteAsync(id);
        TempData["StatusMessage"] = "Đã xoá mẫu xe khỏi danh mục.";
        return RedirectToPage("Index");
    }
}
