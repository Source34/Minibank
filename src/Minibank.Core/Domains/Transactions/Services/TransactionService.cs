using Minibank.Core.Domains.Transactions.Repositories;
using Minibank.Core.Services;

namespace Minibank.Core.Domains.Transactions.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        public TransactionService(ITransactionRepository transactionRepository, IUnitOfWork unitOfWork)
        {
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
        }   
        
        public async Task CreateAsync(Transaction transaction, CancellationToken cancellationToken)
        {
            await _transactionRepository.CreateAsync(transaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public Task<Transaction> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return _transactionRepository.GetByIdAsync(id, cancellationToken);
        }        

        public Task<List<Transaction>> GetAllAsync(CancellationToken cancellationToken)
        {
            return _transactionRepository.GetAllAsync(cancellationToken);
        }
    }
}