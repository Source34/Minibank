namespace Minibank.Data.HttpClients.Models
{
    public class CurrenciesRateResponse
    {
        public DateTime Date { get; set; }
        public Dictionary<string, CurrencyValue> Valute { get; set; }
    }
}
