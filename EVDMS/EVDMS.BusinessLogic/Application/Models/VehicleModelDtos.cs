using System.ComponentModel.DataAnnotations;
using EVDMS.DataAccess.Entities;

namespace EVDMS.BusinessLogic.Models;

public class VehicleModelListItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ModelCode { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public VehicleStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class VehicleModelEditModel
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(50)]
    [Display(Name = "Model Code")]
    public string ModelCode { get; set; } = string.Empty;

    [StringLength(120)]
    public string Version { get; set; } = string.Empty;

    [StringLength(100)]
    public string Color { get; set; } = string.Empty;

    [Range(0, 2000000)]
    [Display(Name = "Base Price (USD)")]
    [DataType(DataType.Currency)]
    public decimal BasePrice { get; set; }

    [Display(Name = "Status")]
    public VehicleStatus Status { get; set; } = VehicleStatus.Draft;

    [StringLength(1000)]
    public string? Notes { get; set; }
}
