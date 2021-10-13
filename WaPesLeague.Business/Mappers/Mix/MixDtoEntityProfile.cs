using AutoMapper;
using System;
using System.Linq;
using WaPesLeague.Business.Dto.Mix;
using WaPesLeague.Data.Entities.Mix;

namespace WaPesLeague.Business.Mappers.Mix
{
    public class MixDtoEntityProfile : Profile
    {
        public MixDtoEntityProfile()
        {
            CreateMap<MixPositionReservation, MixPositionReservationTimeCalcDto>()
                .ForMember(dest => dest.ReservationId, opt => opt.MapFrom(src => src.MixPositionReservationId))
                .ForMember(dest => dest.MixSessionId, opt => opt.MapFrom(src => src.MixPosition.MixTeam.MixSessionId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.PositionId, opt => opt.MapFrom(src => src.MixPosition.PositionId))
                .ForMember(dest => dest.PositionGroup, opt => opt.MapFrom(src => src.MixPosition.Position.PositionGroup.Name))
                .ForMember(dest => dest.PositionGroupOrder, opt => opt.MapFrom(src => src.MixPosition.Position.PositionGroup.Order))
                .ForMember(dest => dest.CalculatedStartTime, opt => opt.MapFrom(src => new[] { src.MixPosition.MixTeam.MixSession.DateStart, src.DateStart }.Max()))
                .ForMember(dest => dest.CaluclatedEndTime, opt => opt.MapFrom(src => new[] { src.DateEnd ?? DateTime.MaxValue, src.MixPosition.DateEnd ?? DateTime.MaxValue, src.MixPosition.MixTeam.MixSession.DateToClose, src.MixPosition.MixTeam.MixSession.DateClosed ?? DateTime.MaxValue }.Min()));
        }
    }
}
