using AutoMapper;
using Base.Bot.Commands;
using WaPesLeague.Business.Dto;

namespace WaPesLeague.Business.Mappers
{
    public class DiscordCommandPropertiesProfile : Profile
    {
        public DiscordCommandPropertiesProfile()
        {
            CreateMap<DiscordCommandProperties, DiscordCommandPropsDto>();
        }
    }
}
