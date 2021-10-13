using AutoMapper;

namespace WaPesLeague.Data.Entities.Mix.Mappers
{
    public class MixEntityProfile : Profile
    {
        public MixEntityProfile()
        {
            //Those mappings are used to make a new one base on the old 1
            CreateMap<MixChannel, MixChannel>()
                .ForMember(dest => dest.MixChannelId, opt => opt.Ignore())
                //.ForMember(dest => dest.MixChannelTeams, opt => opt.Ignore())
                .ForMember(dest => dest.MixSessions, opt => opt.Ignore())
                .ForMember(dest => dest.MixGroup, opt => opt.Ignore());

            CreateMap<MixChannelTeam, MixChannelTeam>()
                .ForMember(dest => dest.MixChannelTeamId, opt => opt.Ignore())
                .ForMember(dest => dest.MixChannelId, opt => opt.Ignore())
                .ForMember(dest => dest.MixChannel, opt => opt.Ignore());

            CreateMap<MixChannelTeamTag, MixChannelTeamTag>()
                .ForMember(dest => dest.MixChannelTeamTagId, opt => opt.Ignore())
                .ForMember(dest => dest.MixChannelTeamId, opt => opt.Ignore())
                .ForMember(dest => dest.MixChannelTeam, opt => opt.Ignore());

            CreateMap<MixChannelTeamPosition, MixChannelTeamPosition>()
                .ForMember(dest => dest.MixChannelTeamPositionId, opt => opt.Ignore())
                .ForMember(dest => dest.MixChannelTeamId, opt => opt.Ignore())
                .ForMember(dest => dest.MixChannelTeam, opt => opt.Ignore())
                .ForMember(dest => dest.Position, opt => opt.Ignore());



            //These ones are used to make a copy without related data
            CreateMap<MixSession, MixSession>()
                .ForMember(dest => dest.MixTeams, opt => opt.Ignore())
                .ForMember(dest => dest.MixChannel, opt => opt.Ignore())
                .ForMember(dest => dest.RoomOwner, opt => opt.Ignore());

            CreateMap<MixPositionReservation, MixPositionReservation>()
                .ForMember(dest => dest.MixPosition, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<MixPosition, MixPosition>()
                .ForMember(dest => dest.MixTeam, opt => opt.Ignore())
                .ForMember(dest => dest.Position, opt => opt.Ignore())
                .ForMember(dest => dest.Reservations, opt => opt.Ignore());

            CreateMap<MixTeam, MixTeam>()
                .ForMember(dest => dest.MixSession, opt => opt.Ignore())
                .ForMember(dest => dest.Formation, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.Ignore());
        }
    }
}
