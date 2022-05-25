using AutoMapper;
using Minibank.Core.Domains.Users;
using Minibank.Web.Controllers.Users.Dto;

namespace Minibank.Web.Controllers.Users.Mappers
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Login,
                    opt => opt.MapFrom(src => src.Login))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Email))
                .ReverseMap();
        }
    }
}
