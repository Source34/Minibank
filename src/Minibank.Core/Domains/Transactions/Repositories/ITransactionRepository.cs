namespace Minibank.Core.Domains.Transactions.Repositories
{
    public interface ITransactionRepository
    {
        public Task<Transaction> GetByIdAsync(int id, CancellationToken cancellationToken);
        public Task<List<Transaction>> GetAllAsync(CancellationToken cancellationToken);
        public Task CreateAsync(Transaction transaction, CancellationToken cancellationToken);
    }
}
