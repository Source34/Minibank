using System.Net.Http.Json;
using Minibank.Core.Domains.Enums;
using Minibank.Core.Services;
using Minibank.Core.Exceptions;
using Minibank.Data.Entities.Enums;
using Minibank.Data.HttpClients.Models;

namespace Minibank.Data.HttpClients
{
    public class CurrencyExchangeProvider : ICurrencyExchangeProvider
    {
        private readonly HttpClient _httpClient;

        public CurrencyExchangeProvider(HttpClient httpClientFactory)
        {
            _httpClient = httpClientFactory;
        }

        public async Task<decimal> GetExchangeRateAsync(CurrencyCore currencyCode, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetFromJsonAsync<CurrenciesRateResponse>("daily_json.js", cancellationToken);

            if (response == null)
                throw new ValidationException("Ошибка источника данных!");

            return response.Valute[currencyCode.ToString()].Value;
        }

        public async Task<bool> IsExistingCurrencyCodeAsync(CurrencyCore currencyCode, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetFromJsonAsync<CurrenciesRateResponse>("daily_json.js", cancellationToken);

            if (response == null)
                throw new ValidationException("Ошибка источника данных!");
            return response.Valute.ContainsKey(currencyCode.ToString());
        }
    }
}