using AutoMapper;

namespace WaPesLeague.Data.Entities.User.Mappers
{
    public class UserEntityProfile : Profile
    {
        public UserEntityProfile()
        {
            //These ones are used to make a copy without related data
            CreateMap<User, User>()
                .ForMember(dest => dest.MixPositionReservations, opt => opt.Ignore())
                .ForMember(dest => dest.UserMetadatas, opt => opt.Ignore())
                .ForMember(dest => dest.PlatformUsers, opt => opt.Ignore())
                .ForMember(dest => dest.UserMembers, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerOfSessions, opt => opt.Ignore())
                .ForMember(dest => dest.AssociationTenantPlayers, opt => opt.Ignore())
                .ForMember(dest => dest.MatchTeamsConfirmedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.PositionSessionStats, opt => opt.Ignore());
        }
    }
}
