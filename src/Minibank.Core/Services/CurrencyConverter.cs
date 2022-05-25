using Minibank.Core.Domains.Enums;
using Minibank.Core.Exceptions;

namespace Minibank.Core.Services
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private readonly ICurrencyExchangeProvider _currencyExchangeProvider;

        public CurrencyConverter(ICurrencyExchangeProvider currencyExchangeProvider)
        {
            _currencyExchangeProvider = currencyExchangeProvider;
        }

        public async Task<decimal> ConvertAsync(
            decimal amount,
            CurrencyCore fromCurrency,
            CurrencyCore toCurrency,
            CancellationToken cancellationToken)
        {
            await ValidateAndThrow(amount, fromCurrency, toCurrency, cancellationToken);

            //RUB to RUB. <= For example
            if (fromCurrency == toCurrency)
                return amount;

            //Not RUB to not RUB
            if (fromCurrency != CurrencyCore.RUB && toCurrency != CurrencyCore.RUB)
            {
                 var fromCurrInRub = await _currencyExchangeProvider.GetExchangeRateAsync(fromCurrency, cancellationToken);
                 var toCurrInRub = await _currencyExchangeProvider.GetExchangeRateAsync(toCurrency, cancellationToken);
                 return (fromCurrInRub * amount) / toCurrInRub;
            }

            //RUB to not RUB
            if(fromCurrency == CurrencyCore.RUB && toCurrency == CurrencyCore.RUB)
                return amount / await _currencyExchangeProvider.GetExchangeRateAsync(toCurrency, cancellationToken);

            //Not RUB to RUB
            if (fromCurrency != CurrencyCore.RUB && toCurrency == CurrencyCore.RUB)
                return amount * await _currencyExchangeProvider.GetExchangeRateAsync(fromCurrency, cancellationToken);

            throw new Exception("Непредвиденная внутренняя ошибка сервера!");
        }

        private async Task ValidateAndThrow(decimal amount, CurrencyCore fromCurrency, CurrencyCore toCurrency, CancellationToken cancellationToken)
        {
            if (amount < 0)
            {
                throw new ValidationException("Некорректная сумма денег!");
            }
   
            if (fromCurrency != CurrencyCore.RUB
                && !await _currencyExchangeProvider.IsExistingCurrencyCodeAsync(fromCurrency, cancellationToken))
            {
                throw new ValidationException("Неизвестный код исходной валюты!");
            }

            if (toCurrency != CurrencyCore.RUB
                && !await _currencyExchangeProvider.IsExistingCurrencyCodeAsync(toCurrency, cancellationToken))
            {
                throw new ValidationException("Неизвестный код целевой валюты!");
            }
        }
    }
}