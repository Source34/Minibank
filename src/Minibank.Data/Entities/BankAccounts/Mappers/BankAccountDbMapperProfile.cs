using Minibank.Core.Domains.BankAccounts;
using AutoMapper;

namespace Minibank.Data.Entities.BankAccounts.Mappers
{
    public class BankAccountDbMapperProfile : Profile
    {
        public BankAccountDbMapperProfile()
        {
            CreateMap<BankAccount, BankAccountDbModel>()
                .ForMember(dest => dest.BankAccountId,
                    opt => opt.MapFrom(src => src.BankAccountId))
                .ForMember(dest => dest.Owner,
                    opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.CurrencyCode,
                    opt => opt.MapFrom(src => src.CurrencyCode))
                .ForMember(dest => dest.Balance,
                    opt => opt.MapFrom(src => src.Balance))
                .ForMember(dest => dest.OpeningTimestamp,
                    opt => opt.MapFrom(src => src.OpeningTimestamp))
                .ForMember(dest => dest.ClosingTimestamp,
                    opt => opt.MapFrom(src => src.ClosingTimestamp))
                .ForMember(dest => dest.IsActive,
                    opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.IncommingTransactions,
                    opt => opt.MapFrom(src => src.IncommingTransactions))
                .ForMember(dest => dest.OutcommingTransactions,
                    opt => opt.MapFrom(src => src.OutcommingTransactions))
                .ReverseMap();
        }
    }
}
