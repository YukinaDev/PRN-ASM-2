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

[Authorize]
public class IndexModel : PageModel
{
    private readonly DealerAllocationService _dealerAllocationService;
    private readonly IMapper _mapper;

    public IndexModel(DealerAllocationService dealerAllocationService, IMapper mapper)
    {
        _dealerAllocationService = dealerAllocationService;
        _mapper = mapper;
    }

    [BindProperty(SupportsGet = true)]
    public int? DealerId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? VehicleModelId { get; set; }

    [BindProperty(SupportsGet = true)]
    public AllocationStatus? Status { get; set; }

    public IReadOnlyList<DealerAllocationListItem> Allocations { get; private set; } = Array.Empty<DealerAllocationListItem>();

    public IReadOnlyList<SelectListItem> DealerOptions { get; private set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> VehicleModelOptions { get; private set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> StatusOptions { get; private set; } = Array.Empty<SelectListItem>();

    public IReadOnlyList<DealerInventoryAlert> Alerts { get; private set; } = Array.Empty<DealerInventoryAlert>();

    public AllocationSummary Summary { get; private set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        var allocations = await _dealerAllocationService.GetAllocationsAsync(DealerId, VehicleModelId, Status);
        Allocations = _mapper.Map<List<DealerAllocationListItem>>(allocations);

        var alerts = await _dealerAllocationService.GetReorderAlertsAsync();
        Alerts = alerts
            .Where(alert => (!DealerId.HasValue || alert.DealerId == DealerId)
                && (!VehicleModelId.HasValue || alert.VehicleModelId == VehicleModelId))
            .ToList();

        DealerOptions = await BuildDealerOptionsAsync();
        VehicleModelOptions = await BuildVehicleOptionsAsync();
        StatusOptions = BuildStatusOptions();

        Summary = new AllocationSummary
        {
            TotalAllocations = Allocations.Count,
            TotalInStock = Allocations.Sum(item => item.QuantityInStock),
            TotalOnOrder = Allocations.Sum(item => item.QuantityOnOrder),
            DistinctDealers = Allocations.Select(item => item.DealerName).Distinct().Count(),
            PendingAlerts = Alerts.Count
        };
    }

    private async Task<IReadOnlyList<SelectListItem>> BuildDealerOptionsAsync()
    {
        var dealers = await _dealerAllocationService.GetActiveDealersAsync();
        var items = new List<SelectListItem>
        {
            new("Tất cả đại lý", string.Empty, !DealerId.HasValue)
        };
        items.AddRange(
            dealers.Select(dealer => new SelectListItem(dealer.Name, dealer.Id.ToString(), DealerId == dealer.Id)));
        return items;
    }

    private async Task<IReadOnlyList<SelectListItem>> BuildVehicleOptionsAsync()
    {
        var vehicles = await _dealerAllocationService.GetActiveVehicleModelsAsync();
        var items = new List<SelectListItem>
        {
            new("Tất cả mẫu xe", string.Empty, !VehicleModelId.HasValue)
        };
        items.AddRange(
            vehicles.Select(vehicle => new SelectListItem(vehicle.Name, vehicle.Id.ToString(), VehicleModelId == vehicle.Id)));
        return items;
    }

    private IReadOnlyList<SelectListItem> BuildStatusOptions()
    {
        var items = new List<SelectListItem>
        {
            new("Tất cả trạng thái", string.Empty, !Status.HasValue)
        };

        foreach (var status in Enum.GetValues<AllocationStatus>())
        {
            items.Add(new SelectListItem(status.ToString(), status.ToString(), Status == status));
        }

        return items;
    }

    public sealed class AllocationSummary
    {
        public int TotalAllocations { get; set; }
        public int TotalInStock { get; set; }
        public int TotalOnOrder { get; set; }
        public int DistinctDealers { get; set; }
        public int PendingAlerts { get; set; }
    }
}
