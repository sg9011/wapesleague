using AutoMapper;

namespace WaPesLeague.Data.Entities.Discord.Mappers
{
    public class ServerEntityProfile : Profile
    {
        public ServerEntityProfile()
        {
            CreateMap<Server, Server>()
                .ForMember(dest => dest.Members, opt => opt.Ignore())
                .ForMember(dest => dest.MixGroups, opt => opt.Ignore())
                .ForMember(dest => dest.ServerFormations, opt => opt.Ignore())
                .ForMember(dest => dest.DefaultTeams, opt => opt.Ignore())
                .ForMember(dest => dest.ServerRoles, opt => opt.Ignore())
                .ForMember(dest => dest.ServerEvents, opt => opt.Ignore());

            CreateMap<ServerTeam, ServerTeam>()
                .ForMember(dest => dest.Server, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.Ignore());


            CreateMap<ServerSniping, ServerSniping>()
                .ForMember(dest => dest.Server, opt => opt.Ignore());
        }
    }
}
