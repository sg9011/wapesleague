using AutoMapper;

namespace WaPesLeague.Data.Entities.Formation.Mappers
{
    public class FormationEntityConfiguration : Profile
    {
        public FormationEntityConfiguration()
        {
            CreateMap<ServerFormation, ServerFormation>()
                .ForMember(dest => dest.ServerFormationId, opt => opt.MapFrom(src => src.ServerFormationId))
                .ForMember(dest => dest.ServerId, opt => opt.MapFrom(src => src.ServerId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(src => src.IsDefault))
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .ForMember(dest => dest.Positions, opt => opt.Ignore())
                .ForMember(dest => dest.Server, opt => opt.Ignore());

        }
    }
}
