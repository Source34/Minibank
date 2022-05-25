using Minibank.Web.Models.Enums;

namespace Minibank.Web.Controllers.Transactions.Dto
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public CurrencyWeb CurrencyCode { get; set; }
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
    }
}
