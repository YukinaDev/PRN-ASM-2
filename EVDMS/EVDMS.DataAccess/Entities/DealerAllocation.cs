using System;

namespace EVDMS.DataAccess.Entities;

public class DealerAllocation
{
    public int Id { get; set; }
    public int DealerId { get; set; }
    public Dealer? Dealer { get; set; }
    public int VehicleModelId { get; set; }
    public VehicleModel? VehicleModel { get; set; }
    public int QuantityInStock { get; set; }
    public int QuantityOnOrder { get; set; }
    public int ReorderPoint { get; set; }
    public AllocationStatus Status { get; set; } = AllocationStatus.Pending;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}

public enum AllocationStatus
{
    Pending = 0,
    Allocated = 1,
    Fulfilled = 2,
    OnHold = 3
}
