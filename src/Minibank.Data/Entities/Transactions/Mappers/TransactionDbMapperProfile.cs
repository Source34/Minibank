using Minibank.Core.Domains.Transactions;
using AutoMapper;

namespace Minibank.Data.Entities.Transactions.Mappers
{
    public class TransactionDbMapperProfile : Profile
    {
        public TransactionDbMapperProfile()
        {
            CreateMap<Transaction, TransactionDbModel>()
                .ForMember(dest => dest.TransactionId,                    
                    opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.Amount,
                    opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.CurrencyCode,
                    opt => opt.MapFrom(src => src.CurrencyCode))
                .ForMember(dest => dest.FromAccount,
                    opt => opt.MapFrom(src => src.FromAccount))
                .ForMember(dest => dest.ToAccount,
                    opt => opt.MapFrom(src => src.ToAccount))
                .ReverseMap();
        }
    }
}
