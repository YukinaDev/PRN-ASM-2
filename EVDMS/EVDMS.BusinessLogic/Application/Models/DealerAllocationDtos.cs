using System;
using System.ComponentModel.DataAnnotations;
using EVDMS.DataAccess.Entities;

namespace EVDMS.BusinessLogic.Models;

public class DealerAllocationListItem
{
    public int Id { get; set; }
    public string DealerName { get; set; } = string.Empty;
    public string VehicleName { get; set; } = string.Empty;
    public AllocationStatus Status { get; set; }
    public int QuantityInStock { get; set; }
    public int QuantityOnOrder { get; set; }
    public int ReorderPoint { get; set; }
    public DateTime LastUpdated { get; set; }
    public string? Notes { get; set; }
}

public class DealerAllocationEditModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Dealer")]
    public int DealerId { get; set; }

    [Required]
    [Display(Name = "Vehicle Model")]
    public int VehicleModelId { get; set; }

    [Range(0, 10000)]
    [Display(Name = "In Stock")]
    public int QuantityInStock { get; set; }

    [Range(0, 10000)]
    [Display(Name = "On Order")]
    public int QuantityOnOrder { get; set; }

    [Range(0, 10000)]
    [Display(Name = "Reorder Point")]
    public int ReorderPoint { get; set; }

    [Display(Name = "Status")]
    public AllocationStatus Status { get; set; } = AllocationStatus.Pending;

    [StringLength(1000)]
    [DataType(DataType.MultilineText)]
    public string? Notes { get; set; }
}
