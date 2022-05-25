using Minibank.Core.Domains.Enums;

namespace Minibank.Core.Services
{
    public interface ICurrencyExchangeProvider
    {
        public Task<decimal> GetExchangeRateAsync(CurrencyCore сurrencyCode, CancellationToken cancellationToken);

        public Task<bool> IsExistingCurrencyCodeAsync(CurrencyCore currencyCode, CancellationToken cancellationToken);
    }
}