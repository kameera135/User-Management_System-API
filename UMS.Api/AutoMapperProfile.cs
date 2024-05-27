using AutoMapper;

namespace UMS.Api
{
    public class AutoMapperProfile : Profile
    {
        //Automappeer profile. This uses to map DTO s with the tables
        public AutoMapperProfile()
        {
            //*************Extension related mappers**************//

            //CreateMap<Extension, ExtensionCommonDTO>()
            //.ForMember(dest => dest.requestType, opt => opt.MapFrom(src => src.Id == 0 ? RequestTypeDeffinitions.New : RequestTypeDeffinitions.New))
            //.ForMember(dest => dest.DisplayDate, opt => opt.MapFrom(src => src.Date.ToString("yyyy-MM-dd")))
            //.ForMember(dest => dest.RequestBy, opt => opt.MapFrom(src => src.CreatedBy))
            //.ForMember(dest => dest.DisplayRequestBy, opt => opt.MapFrom(src => src.CreatedByNavigation == null ? "" : src.CreatedByNavigation.Fname))
            //.ForMember(dest => dest.DisplayUpdatedBy, opt => opt.MapFrom(src => src.UpdatedByNavigation == null ? "" : src.UpdatedByNavigation.Fname))
            //.ForMember(dest => dest.DisplayApprovedBy, opt => opt.MapFrom(src => src.ApprovedByNavigation == null ? "" : src.ApprovedByNavigation.Fname))
            //.ForMember(dest => dest.DisplayApprovedAt, opt => opt.MapFrom(src => src.ApprovedAt == null ? "" : src.ApprovedAt.Value.ToString("yyyy-MM-dd HH:mm:ss")))
            //.ForMember(dest => dest.DisplayCreatedAt, opt => opt.MapFrom(src => src.CreatedAt == null ? "" : src.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss")))
            //.ForMember(dest => dest.DisplayUpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt == null ? "" : src.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss")))
            //.ForMember(dest => dest.DisplayStartTime, opt => opt.MapFrom(src => src.StartTime == null ? "00:00" : string.Format("{0}:{1}", src.StartTime.Hours.ToString(), src.StartTime.Minutes.ToString())))
            //.ForMember(dest => dest.DisplayEndTime, opt => opt.MapFrom(src => src.EndTime == null ? "00:00" : string.Format("{0}:{1}", src.EndTime.Hours.ToString(), src.EndTime.Minutes.ToString())))
            //.ForMember(dest => dest.DisplayExtensionType, opt => opt.MapFrom(src => src.ExtensionType == null ? "" : (src.ExtensionType.Equals(ExtensionTypeDeffinitions.EXT) ? ExtensionTypeDeffinitions.extension : (src.ExtensionType.Equals(ExtensionTypeDeffinitions.SO) ? ExtensionTypeDeffinitions.standingOrder : ""))));

            //CreateMap<StandingOrderRecurrentDay, StandingOrderRecurrentDayDTO>();

            //CreateMap<StandingOrderExtension, StandingOrderExtensionDTO>();

            //CreateMap<StandingOrderUnit, StandingOrderUnitDTO>();

            //CreateMap<StandingOrderRecurrentDay, StandingOrderRecurrentDaysDTO>();

            //CreateMap<UserName, UserNameDTO>();

            //CreateMap<AssertUnitUserMapping, AssertUnitUserMappingDTO>();

            //CreateMap<AssertUnit, AssertUnitDTO>();
        }
    }
}