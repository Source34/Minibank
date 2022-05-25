using System;
using Minibank.Web.Models.Enums;

namespace Minibank.Web.Controllers.BankAccount.Dto
{
    public class BankAccountDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public decimal Balance { get; set; }
        public CurrencyWeb CurrencyWebCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime OpeningTimestamp { get; set; }
        public DateTime ClosingTimestamp { get; set; }
    }
}
