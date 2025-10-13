using System;
using System.Collections.Generic;

namespace EVDMS.DataAccess.Entities;

public class DealerKpiPlan
{
    public int Id { get; set; }
    public int DealerId { get; set; }
    public Dealer? Dealer { get; set; }
    public DateTime TargetStartDate { get; set; }
    public DateTime TargetEndDate { get; set; }
    public decimal RevenueTarget { get; set; }
    public int UnitTarget { get; set; }
    public decimal InventoryTurnoverTarget { get; set; }
    public PlanStatus Status { get; set; } = PlanStatus.Draft;
    public string? Notes { get; set; }
    public string? RejectionReason { get; set; }
    public string? CreatedById { get; set; }
    public ApplicationUser? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ApprovedById { get; set; }
    public ApplicationUser? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public ICollection<DealerPerformanceLog> PerformanceLogs { get; set; } = new List<DealerPerformanceLog>();
}

public class DealerPerformanceLog
{
    public int Id { get; set; }
    public int DealerKpiPlanId { get; set; }
    public DealerKpiPlan? KpiPlan { get; set; }
    public DateTime ActivityDate { get; set; }
    public int UnitsSold { get; set; }
    public decimal Revenue { get; set; }
    public decimal InventoryTurnover { get; set; }
    public string? RecordedById { get; set; }
    public ApplicationUser? RecordedBy { get; set; }
    public string? Notes { get; set; }
}
