using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.Enums;

namespace Minibank.Core.Domains.Transactions
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public CurrencyCore CurrencyCode { get; set; }
        public BankAccount FromAccount { get; set; }
        public BankAccount ToAccount { get; set; }
    }
}
