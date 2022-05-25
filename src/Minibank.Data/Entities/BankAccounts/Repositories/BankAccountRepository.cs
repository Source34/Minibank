using AutoMapper;
using Minibank.Data.Contexts;
using Minibank.Core.Exceptions;
using Minibank.Core.Domains.Enums;
using Minibank.Core.Domains.Users;
using Minibank.Data.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Minibank.Core.Domains.Messages;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Data.Entities.Transactions;
using Minibank.Core.Domains.BankAccounts.Repositories;

namespace Minibank.Data.Entities.BankAccounts.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly IMapper _mapper;
        private readonly MinibankContext _context;
        public BankAccountRepository(MinibankContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<BankAccount> GetByIdAsync(int bankAccountId, CancellationToken cancellationToken)
        {
            var dbBankAccount = await _context.BankAccounts
                .Include(p => p.Owner)
                .Include(p => p.OutcommingTransactions)
                .Include(p => p.IncommingTransactions)
                .AsNoTracking()
                .FirstOrDefaultAsync(bankAcc => bankAcc.BankAccountId == bankAccountId, cancellationToken);
            
            if (dbBankAccount == null)
            {
                throw new ObjectNotFoundException(
                    ErrorMessages.GetObjectNotFoundErrorMessage(
                        ErrorMessages.GettingByIdErrorLegend, 
                        typeof(BankAccount), 
                        bankAccountId));
            }

            return _mapper.Map<BankAccount>(dbBankAccount);
        }
        
        public async Task CreateAsync(int userId, CurrencyCore currencyCode, CancellationToken cancellationToken)
        {
            var dbUser = await _context.Users
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

            if (dbUser == null)
            {
                throw new ObjectNotFoundException(
                    ErrorMessages.GetObjectNotFoundErrorMessage(
                        ErrorMessages.CreatingErrorLegend, 
                        typeof(User), 
                        userId));
            }

            var dbBankAccount = new BankAccountDbModel()
            {
                Owner = dbUser,
                Balance = 100.00m,
                CurrencyCode = (CurrencyData)currencyCode,
                IsActive = true,
                OpeningTimestamp = DateTime.Now.ToUniversalTime(),
                ClosingTimestamp = null,
                IncommingTransactions = new List<TransactionDbModel>(),
                OutcommingTransactions = new List<TransactionDbModel>()
            };

            _context.BankAccounts.Add(dbBankAccount);
        }
        
        public async Task DeleteAsync(int bankAccountId, CancellationToken cancellationToken)
        {
            var dbBankAccount = await _context.BankAccounts
                .FirstOrDefaultAsync(it => it.BankAccountId == bankAccountId, cancellationToken);

            if (dbBankAccount == null)
            {
                throw new ObjectNotFoundException(
                    ErrorMessages.GetObjectNotFoundErrorMessage(
                        ErrorMessages.DeletingErrorLegend, 
                        typeof(BankAccount), 
                        bankAccountId));
            }

            _context.BankAccounts.Remove(dbBankAccount);
        }

        public async Task CloseAsync(int bankAccountId, CancellationToken cancellationToken)
        {
            var dbBankAccount = await _context.BankAccounts
                .FirstOrDefaultAsync(it => it.BankAccountId == bankAccountId, cancellationToken);

            if (dbBankAccount == null)
            {
                throw new ObjectNotFoundException(
                    ErrorMessages.GetObjectNotFoundErrorMessage(
                        ErrorMessages.ClosingBankAccountErrorLegend, 
                        typeof(BankAccount), 
                        bankAccountId));
            }

            dbBankAccount.IsActive = false;
            dbBankAccount.ClosingTimestamp = DateTime.Now.ToUniversalTime();
        }

        public async Task UpdateAsync(BankAccount bankAccount, CancellationToken cancellationToken)
        {
            var dbBankAccount = await _context.BankAccounts
                .FirstOrDefaultAsync(it => it.BankAccountId == bankAccount.BankAccountId, cancellationToken);

            if (dbBankAccount == null)
            {
                throw new ObjectNotFoundException(
                    ErrorMessages.GetObjectNotFoundErrorMessage(
                        ErrorMessages.UpdatingErrorLegend, 
                        typeof(BankAccount), 
                        bankAccount.BankAccountId));
            }

            dbBankAccount.Balance = bankAccount.Balance; 
            dbBankAccount.IsActive = bankAccount.IsActive;
        }
        
        public Task<List<BankAccount>> GetAllAsync(CancellationToken cancellationToken)
        {
            return  _context.BankAccounts
                .Include(p => p.Owner)
                .Include(p => p.OutcommingTransactions)
                .Include(p => p.IncommingTransactions)
                .AsNoTracking()
                .Select(tr => _mapper.Map<BankAccount>(tr))
                .ToListAsync(cancellationToken);
        }
        
        public Task<bool> IsExistActiveBankAccountsAsync(int userId, CancellationToken cancellationToken)
        {
            return _context.BankAccounts
                .AnyAsync(p => p.Owner.UserId == userId && p.IsActive, cancellationToken);
        }
    }
}