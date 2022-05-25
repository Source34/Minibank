using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Minibank.Core.Domains.Transactions;
using Minibank.Core.Domains.Transactions.Repositories;
using Minibank.Core.Exceptions;
using Minibank.Data.Contexts;

namespace Minibank.Data.Entities.Transactions.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IMapper _mapper;
        private readonly MinibankContext _context;

        public TransactionRepository(MinibankContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task CreateAsync(Transaction transaction, CancellationToken cancellationToken)
        {
            var fromAccount = await _context.BankAccounts
                .FirstOrDefaultAsync(it => it.BankAccountId == transaction.FromAccount.BankAccountId, 
                    cancellationToken);

            var toAccount = await _context.BankAccounts
                .FirstOrDefaultAsync(it => it.BankAccountId == transaction.ToAccount.BankAccountId, 
                    cancellationToken);

            if (fromAccount == null || toAccount == null)
            {
                throw new ObjectNotFoundException($"Ошибка получения BankAccount! " +
                                                  $"Объекты BankAccount c Id = {transaction.FromAccount.BankAccountId} и/или" +
                                                  $"{transaction.FromAccount.BankAccountId} не найдены!");
            }

            var newTransaction  = _mapper.Map<TransactionDbModel>(transaction);
            newTransaction.FromAccount = fromAccount;
            newTransaction.ToAccount = toAccount;

            _context.Add(newTransaction);
        }
        
        public Task<List<Transaction>> GetAllAsync(CancellationToken cancellationToken)
        {
            return _context.Transactions
                .Include(p => p.FromAccount)
                .Include(p => p.ToAccount)
                .AsNoTracking()
                .Select(tr => _mapper.Map<Transaction>(tr))
                .ToListAsync(cancellationToken);
        }

        public async Task<Transaction> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var dbTransaction = await _context.Transactions
                .Include(p => p.FromAccount)
                .Include(p => p.ToAccount)
                .AsNoTracking()
                .FirstOrDefaultAsync(it => it.TransactionId == id, cancellationToken);

            if (dbTransaction == null)
            {
                throw new ObjectNotFoundException($"Ошибка получения! Объект Transaction c Id = {id} не найден!");
            }

            return _mapper.Map<Transaction>(dbTransaction);
        }
    }
}