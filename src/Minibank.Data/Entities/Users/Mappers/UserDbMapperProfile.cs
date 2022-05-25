using AutoMapper;
using Minibank.Core.Domains.Users;

namespace Minibank.Data.Entities.Users.Mappers
{
    public class UserDbMapperProfile
    {
        public class UserMapperProfile : Profile
        {
            public UserMapperProfile()
            {
                CreateMap<User, UserDbModel>()
                    .ForMember(dest => dest.UserId,
                        opt => opt.MapFrom(src => src.UserId))
                    .ForMember(dest => dest.Login,
                        opt => opt.MapFrom(src => src.Login))
                    .ForMember(dest => dest.Email,
                        opt => opt.MapFrom(src => src.Email))
                    .ReverseMap();
            }
        }
    }
}
