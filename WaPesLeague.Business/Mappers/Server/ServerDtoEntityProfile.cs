using AutoMapper;
using System.Linq;
using WaPesLeague.Business.Dto.Server;
using WaPesLeague.Data.Entities.Discord;
using WaPesLeague.Data.Helpers;

namespace WaPesLeague.Business.Mappers.Server
{
    public class ServerDtoEntityProfile : Profile
    {
        public ServerDtoEntityProfile()
        {
            CreateMap<Data.Entities.Discord.Server, ServerDto>()
                .ForMember(dest => dest.ServerName, opt => opt.MapFrom(src => src.ServerName))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.DefaultStartTime, opt => opt.MapFrom(src => new Time(src.DefaultStartTime)))
                .ForMember(dest => dest.DefaultHoursToOpenRegistrationBeforeStart, opt => opt.MapFrom(src => new Time(src.DefaultHoursToOpenRegistrationBeforeStart)))
                .ForMember(dest => dest.DefaultSessionDuration, opt => opt.MapFrom(src => new Time(src.DefaultSessionDuration)))
                .ForMember(dest => dest.DefaultSessionExtraInfo, opt => opt.MapFrom(src => src.DefaultSessionExtraInfo))
                .ForMember(dest => dest.DefaultFormation, opt => opt.MapFrom(src => src.ServerFormations.Single(sf => sf.IsDefault == true).Name))
                .ForMember(dest => dest.DefaultTeams, opt => opt.MapFrom(src => src.DefaultTeams))
                .ForMember(dest => dest.TimeZoneName, opt => opt.MapFrom(src => src.TimeZoneName))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language))
                .ForMember(dest => dest.ServerSniping, opt => opt.MapFrom(src => src.ServerSnipings != null ? src.ServerSnipings.FirstOrDefault() : null));

            CreateMap<ServerTeam, ServerTeamDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.IsOpen, opt => opt.MapFrom(src => src.IsOpen))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));

            CreateMap<ServerTeamTag, string>()
                .ConvertUsing(src => src.Tag);

        }
    }
}
