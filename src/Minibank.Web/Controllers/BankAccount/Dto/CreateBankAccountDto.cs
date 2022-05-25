using Minibank.Web.Models.Enums;

namespace Minibank.Web.Controllers.BankAccount.Dto
{
    public class CreateBankAccountDto
    {
        public int UserId { get; set; }
        public CurrencyWeb Currency {get; set; }
    }
}
