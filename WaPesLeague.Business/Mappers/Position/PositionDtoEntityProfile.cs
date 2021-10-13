using AutoMapper;
using System.Linq;
using WaPesLeague.Business.Dto.Position;

namespace WaPesLeague.Business.Mappers.Position
{
    public class PositionDtoEntityProfile : Profile
    {
        public PositionDtoEntityProfile()
        {
            CreateMap<Data.Entities.Position.Position, PositionDto>()
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ParentPositionCode, opt => opt.MapFrom(src => src.ParentPosition.Code))
                .ForMember(dest => dest.PositionGroup, opt => opt.MapFrom(src => src.PositionGroup.Name))
                .ForMember(dest => dest.PositionGroupOrder, opt => opt.MapFrom(src => src.PositionGroup.Order))
                .ForMember(dest => dest.PositionOrder, opt => opt.MapFrom(src => src.Order))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Tag).ToList()))
                .ForMember(dest => dest.IsRequiredForMix, opt => opt.MapFrom(src => src.IsRequiredForMix));

        }
    }
}
