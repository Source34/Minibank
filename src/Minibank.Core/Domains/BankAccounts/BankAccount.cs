using Minibank.Core.Domains.Transactions;
using Minibank.Core.Domains.Enums;
using Minibank.Core.Domains.Users;

namespace Minibank.Core.Domains.BankAccounts
{
    public class BankAccount
    {
        public int BankAccountId { get; set; }
        public User Owner { get; set; }
        public decimal Balance { get; set; }
        public CurrencyCore CurrencyCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime OpeningTimestamp { get; set; }
        public DateTime ClosingTimestamp { get; set; }
        public List<Transaction> IncommingTransactions { get; set; }
        public List<Transaction> OutcommingTransactions { get; set; }
    }
}
