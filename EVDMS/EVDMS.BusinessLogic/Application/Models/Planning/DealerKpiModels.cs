using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EVDMS.DataAccess.Entities;

namespace EVDMS.BusinessLogic.Application.Models.Planning;

public class DealerKpiPlanSummary
{
    public int Id { get; set; }
    public string DealerName { get; set; } = string.Empty;
    public DateTime TargetStartDate { get; set; }
    public DateTime TargetEndDate { get; set; }
    public PlanStatus Status { get; set; }
    public decimal RevenueTarget { get; set; }
    public int UnitTarget { get; set; }
    public decimal InventoryTurnoverTarget { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class DealerKpiPlanDetail : DealerKpiPlanSummary
{
    public string? Notes { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
    public decimal RevenueAchieved { get; set; }
    public int UnitsAchieved { get; set; }
    public decimal InventoryTurnoverAchieved { get; set; }
    public List<DealerPerformanceLogModel> PerformanceLogs { get; set; } = new();
}

public class DealerPerformanceLogModel
{
    public int Id { get; set; }
    public DateTime ActivityDate { get; set; }
    public int UnitsSold { get; set; }
    public decimal Revenue { get; set; }
    public decimal InventoryTurnover { get; set; }
    public string RecordedBy { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class DealerKpiPlanCreateModel
{
    [Required]
    [Display(Name = "Đại lý")]
    public int DealerId { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày bắt đầu")]
    public DateTime TargetStartDate { get; set; } = DateTime.UtcNow.Date;

    [DataType(DataType.Date)]
    [Display(Name = "Ngày kết thúc")]
    public DateTime TargetEndDate { get; set; } = DateTime.UtcNow.Date.AddMonths(1).AddDays(-1);

    [Range(0, 1_000_000_000)]
    [Display(Name = "Doanh thu mục tiêu (VND)")]
    public decimal RevenueTarget { get; set; }

    [Range(0, 10000)]
    [Display(Name = "Số xe mục tiêu")]
    public int UnitTarget { get; set; }

    [Range(0, 100)]
    [Display(Name = "Vòng quay tồn kho mục tiêu")]
    public decimal InventoryTurnoverTarget { get; set; }

    [StringLength(2000)]
    [Display(Name = "Ghi chú")]
    public string? Notes { get; set; }
}

public class DealerPerformanceLogInput
{
    [Required]
    public int DealerKpiPlanId { get; set; }

    [DataType(DataType.Date)]
    public DateTime ActivityDate { get; set; } = DateTime.UtcNow.Date;

    [Range(0, 10000)]
    public int UnitsSold { get; set; }

    [Range(0, 1_000_000_000)]
    public decimal Revenue { get; set; }

    [Range(0, 100)]
    public decimal InventoryTurnover { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}
