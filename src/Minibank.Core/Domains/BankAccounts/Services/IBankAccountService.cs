using Minibank.Core.Domains.Enums;

namespace Minibank.Core.Domains.BankAccounts.Services
{
    public interface IBankAccountService
    {       
        public Task CreateAsync(int userId, CurrencyCore currencyCode, CancellationToken cancellationToken);
        public Task UpdateAsync(BankAccount bankAccount, CancellationToken cancellationToken);
        public Task DeleteAsync(int id, CancellationToken cancellationToken);
        public Task CloseAsync(int id, CancellationToken cancellationToken);  
        public Task<List<BankAccount>> GetAllAsync(CancellationToken cancellationToken); 
        public Task<BankAccount> GetByIdAsync(int id, CancellationToken cancellationToken);  
        public Task<decimal> CalculateTransferСommissionAsync(decimal amount, int fromAccountId, int toAccountId, CancellationToken cancellationToken);
        public Task ExecuteTransferAsync(decimal amount, int fromAccountId, int toAccountId, CancellationToken cancellationToken);
    }
}
