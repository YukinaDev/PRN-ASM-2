using System.Collections.Generic;

namespace EVDMS.DataAccess.Entities;

public class VehicleModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ModelCode { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public VehicleStatus Status { get; set; } = VehicleStatus.Draft;
    public string? Notes { get; set; }
    public ICollection<DealerAllocation> Allocations { get; set; } = new List<DealerAllocation>();
}

public enum VehicleStatus
{
    Draft = 0,
    Active = 1,
    Discontinued = 2
}
