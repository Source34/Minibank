using Minibank.Core.Domains.Enums;

namespace Minibank.Core.Domains.BankAccounts.Repositories
{
    public interface IBankAccountRepository
    {     
        public Task CreateAsync(int userId, CurrencyCore currencyCode, CancellationToken cancellationToken);
        public Task UpdateAsync(BankAccount bankAccount, CancellationToken cancellationToken);
        public Task DeleteAsync(int bankAccountId, CancellationToken cancellationToken);
        public Task<BankAccount> GetByIdAsync(int bankAccountId, CancellationToken cancellationToken);
        public Task<List<BankAccount>> GetAllAsync(CancellationToken cancellationToken);
        public Task CloseAsync(int bankAccountId, CancellationToken cancellationToken);
        public Task<bool> IsExistActiveBankAccountsAsync(int userId, CancellationToken cancellationToken);
    }
}
