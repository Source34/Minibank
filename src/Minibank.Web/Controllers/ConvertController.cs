using System.Threading;
using System.Threading.Tasks;
using Minibank.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Minibank.Web.Models.Enums;
using Minibank.Core.Domains.Enums;

namespace Minibank.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConvertController : ControllerBase
    {   
        private readonly ICurrencyConverter _currencyConverter;

        public ConvertController(ICurrencyConverter currencyConverter)
        {
            _currencyConverter = currencyConverter;
        }

        [HttpGet]
        public async Task<decimal> Get(int amount, CurrencyWeb fromCurrencyWeb, CurrencyWeb toCurrencyWeb, CancellationToken cancellationToken = default)
        {
           return await _currencyConverter.ConvertAsync(amount, (CurrencyCore)fromCurrencyWeb, (CurrencyCore)toCurrencyWeb, cancellationToken);
        }
    }
}
