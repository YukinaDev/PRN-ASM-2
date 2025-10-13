using EVDMS.DataAccess.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EVDMS.DataAccess.Database;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<VehicleModel> VehicleModels => Set<VehicleModel>();
    public DbSet<Dealer> Dealers => Set<Dealer>();
    public DbSet<DealerAllocation> DealerAllocations => Set<DealerAllocation>();
    public DbSet<DistributionPlan> DistributionPlans => Set<DistributionPlan>();
    public DbSet<DistributionPlanLine> DistributionPlanLines => Set<DistributionPlanLine>();
    public DbSet<DealerKpiPlan> DealerKpiPlans => Set<DealerKpiPlan>();
    public DbSet<DealerPerformanceLog> DealerPerformanceLogs => Set<DealerPerformanceLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<VehicleModel>(entity =>
        {
            entity.Property(m => m.Name).HasMaxLength(200).IsRequired();
            entity.Property(m => m.ModelCode).HasMaxLength(50).IsRequired();
            entity.Property(m => m.Version).HasMaxLength(120);
            entity.Property(m => m.Color).HasMaxLength(100);
            entity.Property(m => m.BasePrice).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Dealer>(entity =>
        {
            entity.Property(d => d.Name).HasMaxLength(200).IsRequired();
            entity.Property(d => d.Region).HasMaxLength(120);
            entity.Property(d => d.ContactEmail).HasMaxLength(256);
            entity.Property(d => d.ContactPhone).HasMaxLength(50);
        });

        modelBuilder.Entity<DealerAllocation>(entity =>
        {
            entity.HasOne(a => a.VehicleModel)
                .WithMany(m => m.Allocations)
                .HasForeignKey(a => a.VehicleModelId);

            entity.HasOne(a => a.Dealer)
                .WithMany(d => d.Allocations)
                .HasForeignKey(a => a.DealerId);

            entity.Property(a => a.Notes).HasMaxLength(1000);
        });

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.DisplayName).HasMaxLength(200);
            entity.HasOne(u => u.Dealer)
                .WithMany()
                .HasForeignKey(u => u.DealerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DistributionPlan>(entity =>
        {
            entity.Property(p => p.PlanName).HasMaxLength(200).IsRequired();
            entity.Property(p => p.Description).HasMaxLength(2000);
            entity.Property(p => p.TargetMonth).HasColumnType("date");
            entity.Property(p => p.RejectionReason).HasMaxLength(2000);
            entity.HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(p => p.ApprovedBy)
                .WithMany()
                .HasForeignKey(p => p.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DistributionPlanLine>(entity =>
        {
            entity.Property(l => l.DiscountRate).HasPrecision(5, 2);
            entity.HasOne(l => l.Plan)
                .WithMany(p => p.Lines)
                .HasForeignKey(l => l.DistributionPlanId);
            entity.HasOne(l => l.Dealer)
                .WithMany()
                .HasForeignKey(l => l.DealerId);
            entity.HasOne(l => l.VehicleModel)
                .WithMany()
                .HasForeignKey(l => l.VehicleModelId);
        });

        modelBuilder.Entity<DealerKpiPlan>(entity =>
        {
            entity.HasOne(p => p.Dealer)
                .WithMany()
                .HasForeignKey(p => p.DealerId);
            entity.HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(p => p.ApprovedBy)
                .WithMany()
                .HasForeignKey(p => p.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);
            entity.Property(p => p.RevenueTarget).HasPrecision(18, 2);
            entity.Property(p => p.InventoryTurnoverTarget).HasPrecision(9, 2);
            entity.Property(p => p.Notes).HasMaxLength(2000);
            entity.Property(p => p.RejectionReason).HasMaxLength(2000);
            entity.Property(p => p.TargetStartDate).HasColumnType("date");
            entity.Property(p => p.TargetEndDate).HasColumnType("date");
        });

        modelBuilder.Entity<DealerPerformanceLog>(entity =>
        {
            entity.Property(l => l.Revenue).HasPrecision(18, 2);
            entity.Property(l => l.InventoryTurnover).HasPrecision(9, 2);
            entity.Property(l => l.ActivityDate).HasColumnType("date");
            entity.HasOne(l => l.KpiPlan)
                .WithMany(p => p.PerformanceLogs)
                .HasForeignKey(l => l.DealerKpiPlanId);
            entity.HasOne(l => l.RecordedBy)
                .WithMany()
                .HasForeignKey(l => l.RecordedById)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
