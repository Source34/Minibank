namespace Minibank.Web.Controllers.Transactions.Dto
{
    public class CreateTransactionDto
    {
        public decimal Amount { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
    }
}
