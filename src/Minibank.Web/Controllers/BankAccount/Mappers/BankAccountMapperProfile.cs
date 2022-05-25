using AutoMapper;
using Minibank.Web.Controllers.BankAccount.Dto;

namespace Minibank.Web.Controllers.BankAccount.Mappers
{
    public class BankAccountMapperProfile : Profile
    {
        public BankAccountMapperProfile()
        {
            CreateMap<Core.Domains.BankAccounts.BankAccount, BankAccountDto>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.BankAccountId))
                .ForMember(dest => dest.UserId,
                    opt => opt.MapFrom(src => src.Owner.UserId))
                .ForMember(dest => dest.CurrencyWebCode,
                    opt => opt.MapFrom(src => src.CurrencyCode))
                .ForMember(dest => dest.Balance,
                    opt => opt.MapFrom(src => src.Balance))
                .ForMember(dest => dest.OpeningTimestamp,
                    opt => opt.MapFrom(src => src.OpeningTimestamp))
                .ForMember(dest => dest.ClosingTimestamp,
                    opt => opt.MapFrom(src => src.ClosingTimestamp))
                .ForMember(dest => dest.IsActive,
                    opt => opt.MapFrom(src => src.IsActive))
                .ReverseMap();
        }
    }
}