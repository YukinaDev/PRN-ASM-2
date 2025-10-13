using System;
using System.Collections.Generic;

namespace EVDMS.DataAccess.Entities;

public class DistributionPlan
{
    public int Id { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime TargetMonth { get; set; }
    public PlanStatus Status { get; set; } = PlanStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedById { get; set; }
    public ApplicationUser? CreatedBy { get; set; }
    public string? ApprovedById { get; set; }
    public ApplicationUser? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
    public ICollection<DistributionPlanLine> Lines { get; set; } = new List<DistributionPlanLine>();
}

public class DistributionPlanLine
{
    public int Id { get; set; }
    public int DistributionPlanId { get; set; }
    public DistributionPlan? Plan { get; set; }
    public int DealerId { get; set; }
    public Dealer? Dealer { get; set; }
    public int VehicleModelId { get; set; }
    public VehicleModel? VehicleModel { get; set; }
    public int PlannedQuantity { get; set; }
    public decimal DiscountRate { get; set; }
    public string? Notes { get; set; }
}

public enum PlanStatus
{
    Draft = 0,
    Submitted = 1,
    Approved = 2,
    Rejected = 3
}
