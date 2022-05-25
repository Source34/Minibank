using Minibank.Core.Domains.Enums;

namespace Minibank.Core.Services
{
    public interface ICurrencyConverter
    {
        public Task<decimal> ConvertAsync(decimal amount, CurrencyCore fromCurrency, CurrencyCore toCurrency, CancellationToken cancellationToken);
    }
}