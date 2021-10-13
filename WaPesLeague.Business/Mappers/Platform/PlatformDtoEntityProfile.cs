using AutoMapper;
using System.Linq;
using WaPesLeague.Business.Dto.Platform;
using WaPesLeague.Business.Dto.User;

namespace WaPesLeague.Business.Mappers.Platform
{
    public class PlatformDtoEntityProfile : Profile
    {
        public PlatformDtoEntityProfile()
        {
            CreateMap<Data.Entities.Platform.Platform, SimplePlatformDto>();
            CreateMap<Data.Entities.Platform.Platform, PlatformWithUsersDto>()
                .ForMember(dest => dest.PlatformUserIds, opt => opt.MapFrom(src => src.PlatformUsers.Where(pu => !string.IsNullOrWhiteSpace(pu.UserName))));

            CreateMap<Data.Entities.User.UserPlatform, string>()
                .ConvertUsing(src => src.UserName);

            CreateMap<Data.Entities.User.UserPlatform, PlatformUserDto>()
                .ForMember(dest => dest.PlatformName, opt => opt.MapFrom(src => src.Platform.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName));
        }
    }
}
