using AutoMapper;
using System.Linq;
using WaPesLeague.Business.Dto.Formation;
using WaPesLeague.Data.Entities.Formation;

namespace WaPesLeague.Business.Mappers.Formation
{
    public class FormationDtoEntityProfile : Profile
    {
        public FormationDtoEntityProfile()
        {
            CreateMap<ServerFormation, FormationDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(src => src.IsDefault))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Tag).ToList()))
                .ForMember(dest => dest.Positions, opt => opt.MapFrom(src => src.Positions.OrderBy(fp => fp.Position.PositionGroup.Order).ThenBy(fp => fp.Position.Order).Select(fp => fp.Position.Code).ToList()));
        }
    }
}
