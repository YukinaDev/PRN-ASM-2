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

[Authorize]
public class IndexModel : PageModel
{
    private readonly IVehicleCatalogService _vehicleCatalogService;
    private readonly IMapper _mapper;

    public IndexModel(IVehicleCatalogService vehicleCatalogService, IMapper mapper)
    {
        _vehicleCatalogService = vehicleCatalogService;
        _mapper = mapper;
    }

    public IReadOnlyList<VehicleModelListItem> Vehicles { get; private set; } = Array.Empty<VehicleModelListItem>();

    [BindProperty(SupportsGet = true)]
    public VehicleStatus? Status { get; set; }

    public IReadOnlyList<SelectListItem> StatusOptions { get; private set; } = Array.Empty<SelectListItem>();

    public CatalogSummary Summary { get; private set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        var allModels = await _vehicleCatalogService.GetAllAsync();

        Summary = new CatalogSummary
        {
            Total = allModels.Count,
            Active = allModels.Count(model => model.Status == VehicleStatus.Active),
            Draft = allModels.Count(model => model.Status == VehicleStatus.Draft),
            Discontinued = allModels.Count(model => model.Status == VehicleStatus.Discontinued),
            AverageBasePrice = allModels.Count == 0 ? 0 : Math.Round(allModels.Average(model => model.BasePrice), 2)
        };

        var filtered = Status.HasValue
            ? allModels.Where(model => model.Status == Status.Value).ToList()
            : allModels;

        Vehicles = _mapper.Map<List<VehicleModelListItem>>(filtered);

        StatusOptions = BuildStatusOptions();
    }

    private IReadOnlyList<SelectListItem> BuildStatusOptions()
    {
        var options = new List<SelectListItem>
        {
            new("All statuses", string.Empty, !Status.HasValue)
        };

        foreach (var status in Enum.GetValues<VehicleStatus>())
        {
            options.Add(new SelectListItem(status.ToString(), status.ToString(), Status == status));
        }

        return options;
    }

    public sealed class CatalogSummary
    {
        public int Total { get; set; }
        public int Active { get; set; }
        public int Draft { get; set; }
        public int Discontinued { get; set; }
        public decimal AverageBasePrice { get; set; }
    }
}
