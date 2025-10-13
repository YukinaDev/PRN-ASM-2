using System;
using System.Collections.Generic;
using System.Linq;
using EVDMS.DataAccess.Entities;

namespace EVDMS.DataAccess.Database;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        if (!context.VehicleModels.Any())
        {
            var models = new List<VehicleModel>
            {
                new()
                {
                    Name = "Eclipse LX",
                    ModelCode = "ECL-LX",
                    Version = "2025 Launch Edition",
                    Color = "Arctic White",
                    BasePrice = 38500m,
                    Status = VehicleStatus.Active,
                    Notes = "Phiên bản chiến lược dành cho đại lý hạng Platinum."
                },
                new()
                {
                    Name = "Volt Runner",
                    ModelCode = "VR-SPORT",
                    Version = "Sport AWD",
                    Color = "Graphite Black",
                    BasePrice = 42900m,
                    Status = VehicleStatus.Active,
                    Notes = "Tăng tốc 0-100km trong 4.8s, phù hợp phân khúc cao cấp."
                },
                new()
                {
                    Name = "CityGlide",
                    ModelCode = "CG-URBAN",
                    Version = "Urban Flex",
                    Color = "Electric Blue",
                    BasePrice = 29900m,
                    Status = VehicleStatus.Draft,
                    Notes = "Đang chuẩn bị ra mắt thị trường miền Bắc."
                }
            };

            context.VehicleModels.AddRange(models);
            context.SaveChanges();
        }

        if (!context.Dealers.Any())
        {
            var dealers = new List<Dealer>
            {
                new()
                {
                    Name = "Metro EV Hub",
                    Region = "Hồ Chí Minh",
                    ContactEmail = "sales@metroev.vn",
                    ContactPhone = "+84-28-5555-2222",
                    IsActive = true
                },
                new()
                {
                    Name = "Northern Volt Dealers",
                    Region = "Hà Nội",
                    ContactEmail = "contact@northernvolt.vn",
                    ContactPhone = "+84-24-7777-9999",
                    IsActive = true
                },
                new()
                {
                    Name = "Coastal Green Mobility",
                    Region = "Đà Nẵng",
                    ContactEmail = "hello@coastalgreen.vn",
                    ContactPhone = "+84-236-888-4444",
                    IsActive = true
                }
            };

            context.Dealers.AddRange(dealers);
            context.SaveChanges();
        }

        if (!context.DealerAllocations.Any())
        {
            var dealers = context.Dealers.ToList();
            var models = context.VehicleModels.ToList();

            var allocations = new List<DealerAllocation>
            {
                new()
                {
                    DealerId = dealers[0].Id,
                    VehicleModelId = models[0].Id,
                    QuantityInStock = 12,
                    QuantityOnOrder = 8,
                    ReorderPoint = 10,
                    Status = AllocationStatus.Allocated,
                    LastUpdated = DateTime.UtcNow.AddDays(-3),
                    Notes = "Chuẩn bị nhận lô hàng mới trong tuần này."
                },
                new()
                {
                    DealerId = dealers[0].Id,
                    VehicleModelId = models[1].Id,
                    QuantityInStock = 5,
                    QuantityOnOrder = 12,
                    ReorderPoint = 8,
                    Status = AllocationStatus.Pending,
                    LastUpdated = DateTime.UtcNow.AddDays(-1),
                    Notes = "Yêu cầu bổ sung khẩn do nhu cầu cao."
                },
                new()
                {
                    DealerId = dealers[1].Id,
                    VehicleModelId = models[1].Id,
                    QuantityInStock = 18,
                    QuantityOnOrder = 0,
                    ReorderPoint = 12,
                    Status = AllocationStatus.Fulfilled,
                    LastUpdated = DateTime.UtcNow.AddDays(-6),
                    Notes = "Đáp ứng đoàn xe demo của chính phủ."
                }
            };

            context.DealerAllocations.AddRange(allocations);
            context.SaveChanges();
        }

        if (!context.DistributionPlans.Any())
        {
            var dealers = context.Dealers.Take(2).ToList();
            var models = context.VehicleModels.Where(m => m.Status == VehicleStatus.Active).ToList();

            var plan = new DistributionPlan
            {
                PlanName = "Kế hoạch phân phối Q1",
                Description = "Phân bổ sản phẩm chiến lược cho đại lý trọng điểm khu vực miền Nam và miền Bắc.",
                TargetMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
                Status = PlanStatus.Approved,
                ApprovedAt = DateTime.UtcNow.AddDays(-2),
                Lines = new List<DistributionPlanLine>
                {
                    new()
                    {
                        DealerId = dealers[0].Id,
                        VehicleModelId = models[0].Id,
                        PlannedQuantity = 15,
                        DiscountRate = 7m,
                        Notes = "Áp dụng combo ưu đãi lái thử & marketing địa phương."
                    },
                    new()
                    {
                        DealerId = dealers[1].Id,
                        VehicleModelId = models[1].Id,
                        PlannedQuantity = 20,
                        DiscountRate = 5m,
                        Notes = "Triển khai chiến dịch fleet cho doanh nghiệp."
                    }
                }
            };

            context.DistributionPlans.Add(plan);
            context.SaveChanges();
        }

        if (!context.DealerKpiPlans.Any())
        {
            var dealer = context.Dealers.First();

            var kpiPlan = new DealerKpiPlan
            {
                DealerId = dealer.Id,
                TargetStartDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
                TargetEndDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1).AddDays(-1),
                RevenueTarget = 950_000_000m,
                UnitTarget = 35,
                InventoryTurnoverTarget = 4.5m,
                Status = PlanStatus.Approved,
                ApprovedAt = DateTime.UtcNow.AddDays(-5),
                Notes = "Tập trung bán mẫu Eclipse LX cho phân khúc doanh nghiệp.",
                PerformanceLogs = new List<DealerPerformanceLog>
                {
                    new()
                    {
                        ActivityDate = DateTime.UtcNow.Date.AddDays(-7),
                        UnitsSold = 6,
                        Revenue = 155_000_000m,
                        InventoryTurnover = 4.1m,
                        Notes = "Chốt hợp đồng thuê xe với startup công nghệ."
                    },
                    new()
                    {
                        ActivityDate = DateTime.UtcNow.Date.AddDays(-2),
                        UnitsSold = 4,
                        Revenue = 110_000_000m,
                        InventoryTurnover = 4.6m,
                        Notes = "Doanh số từ sự kiện trải nghiệm cuối tuần."
                    }
                }
            };

            context.DealerKpiPlans.Add(kpiPlan);
            context.SaveChanges();
        }
    }
}
