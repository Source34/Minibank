using AutoMapper;
using Minibank.Core.Domains.Transactions;
using Minibank.Web.Controllers.Transactions.Dto;

namespace Minibank.Web.Controllers.Transactions.Mappers
{
    public class TransactionMapperProfile : Profile
    {
        public TransactionMapperProfile()
        {
            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.Amount,
                    opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.CurrencyCode,
                    opt => opt.MapFrom(src => src.CurrencyCode))
                .ForMember(dest => dest.FromAccountId,
                    opt => opt.MapFrom(src => src.FromAccount.BankAccountId))
                .ForMember(dest => dest.ToAccountId,
                    opt => opt.MapFrom(src => src.ToAccount.BankAccountId))
                .ReverseMap();
        }
    }
}
