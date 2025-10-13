using System.Linq;
using AutoMapper;
using EVDMS.BusinessLogic.Application.Models.Planning;
using EVDMS.BusinessLogic.Models;
using EVDMS.DataAccess.Entities;

namespace EVDMS.BusinessLogic.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<VehicleModel, VehicleModelListItem>();
        CreateMap<VehicleModel, VehicleModelEditModel>().ReverseMap();

        CreateMap<DealerAllocation, DealerAllocationListItem>()
            .ForMember(dest => dest.DealerName, opt => opt.MapFrom(src => src.Dealer!.Name))
            .ForMember(dest => dest.VehicleName, opt => opt.MapFrom(src => src.VehicleModel!.Name));

        CreateMap<DealerAllocation, DealerAllocationEditModel>().ReverseMap();

        CreateMap<DistributionPlan, DistributionPlanSummary>()
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy != null ? src.CreatedBy.DisplayName : string.Empty))
            .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.ApprovedBy != null ? src.ApprovedBy.DisplayName : null));

        CreateMap<DistributionPlan, DistributionPlanDetail>()
            .IncludeBase<DistributionPlan, DistributionPlanSummary>();

        CreateMap<DistributionPlanLine, DistributionPlanLineModel>()
            .ForMember(dest => dest.DealerName, opt => opt.MapFrom(src => src.Dealer != null ? src.Dealer.Name : string.Empty))
            .ForMember(dest => dest.VehicleName, opt => opt.MapFrom(src => src.VehicleModel != null ? src.VehicleModel.Name : string.Empty));

        CreateMap<DistributionPlanLineInput, DistributionPlanLine>();

        CreateMap<DealerKpiPlan, DealerKpiPlanSummary>()
            .ForMember(dest => dest.DealerName, opt => opt.MapFrom(src => src.Dealer != null ? src.Dealer.Name : string.Empty))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy != null ? src.CreatedBy.DisplayName : string.Empty));

        CreateMap<DealerKpiPlan, DealerKpiPlanDetail>()
            .IncludeBase<DealerKpiPlan, DealerKpiPlanSummary>()
            .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.ApprovedBy != null ? src.ApprovedBy.DisplayName : null))
            .ForMember(dest => dest.RevenueAchieved, opt => opt.MapFrom(src => src.PerformanceLogs.Sum(log => log.Revenue)))
            .ForMember(dest => dest.UnitsAchieved, opt => opt.MapFrom(src => src.PerformanceLogs.Sum(log => log.UnitsSold)))
            .ForMember(dest => dest.InventoryTurnoverAchieved, opt => opt.MapFrom(src => src.PerformanceLogs.Any() ? src.PerformanceLogs.Average(log => log.InventoryTurnover) : 0m));

        CreateMap<DealerKpiPlanCreateModel, DealerKpiPlan>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => PlanStatus.Draft))
            .ForMember(dest => dest.PerformanceLogs, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ApprovedBy, opt => opt.Ignore());

        CreateMap<DealerPerformanceLog, DealerPerformanceLogModel>()
            .ForMember(dest => dest.RecordedBy, opt => opt.MapFrom(src => src.RecordedBy != null ? src.RecordedBy.DisplayName : string.Empty));

        CreateMap<DealerPerformanceLogInput, DealerPerformanceLog>()
            .ForMember(dest => dest.RecordedBy, opt => opt.Ignore())
            .ForMember(dest => dest.KpiPlan, opt => opt.Ignore());
    }
}
