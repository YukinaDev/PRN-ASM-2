using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EVDMS.DataAccess.Entities;

namespace EVDMS.BusinessLogic.Application.Models.Planning;

public class DistributionPlanSummary
{
    public int Id { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public DateTime TargetMonth { get; set; }
    public PlanStatus Status { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

public class DistributionPlanDetail : DistributionPlanSummary
{
    public string? Description { get; set; }
    public string? RejectionReason { get; set; }
    public List<DistributionPlanLineModel> Lines { get; set; } = new();
}

public class DistributionPlanLineModel
{
    public int Id { get; set; }
    public int DealerId { get; set; }
    public string DealerName { get; set; } = string.Empty;
    public int VehicleModelId { get; set; }
    public string VehicleName { get; set; } = string.Empty;
    public int PlannedQuantity { get; set; }
    public decimal DiscountRate { get; set; }
    public string? Notes { get; set; }
}

public class DistributionPlanCreateModel
{
    [Required]
    [Display(Name = "Tên kế hoạch")]
    [StringLength(200)]
    public string PlanName { get; set; } = string.Empty;

    [Display(Name = "Mô tả")]
    [StringLength(2000)]
    public string? Description { get; set; }

    [Display(Name = "Tháng triển khai")]
    [DataType(DataType.Date)]
    public DateTime TargetMonth { get; set; } = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

    public List<DistributionPlanLineInput> Lines { get; set; } = new();
}

public class DistributionPlanLineInput
{
    [Required]
    [Display(Name = "Đại lý")]
    public int DealerId { get; set; }

    [Required]
    [Display(Name = "Mẫu xe")]
    public int VehicleModelId { get; set; }

    [Range(1, 1000)]
    [Display(Name = "Số lượng phân bổ")]
    public int PlannedQuantity { get; set; }

    [Range(0, 100)]
    [Display(Name = "Chiết khấu (%)")]
    public decimal DiscountRate { get; set; }

    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? Notes { get; set; }
}
