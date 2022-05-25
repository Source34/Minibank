using FluentValidation;
using Minibank.Core.Services;
using Minibank.Core.Domains.Enums;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Transactions;
using Minibank.Core.Domains.Transactions.Repositories;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.BankAccounts.Validators;
using Minibank.Core.Domains.Transactions.Validators;

namespace Minibank.Core.Domains.BankAccounts.Services
{
    public class BankAccountService : IBankAccountService
    {           
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly ICurrencyConverter _currencyConverter;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<BankAccount> _bankAccountValidator;
        private readonly IValidator<Transaction> _transactionValidator;
        private const decimal CommissionAmount = 0.02m;

        public BankAccountService(
            IBankAccountRepository bankAccountRepository, 
            ICurrencyConverter currencyConverter, 
            ITransactionRepository transactionRepository,
            IUnitOfWork unitOfWork,
            IValidator<BankAccount> bankAccountValidator,
            IValidator<Transaction> transactionValidator)
        {
            _bankAccountRepository = bankAccountRepository;
            _currencyConverter = currencyConverter;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _bankAccountValidator = bankAccountValidator;
            _transactionValidator = transactionValidator;
        }

        public async Task CreateAsync(
            int userId,
            CurrencyCore currencyCode,
            CancellationToken cancellationToken)
        {
            var tempBankAcc = new BankAccount()
            {
                Owner = new User() {UserId = userId},
                CurrencyCode = currencyCode
            };

            await _bankAccountValidator.ValidateAsync(tempBankAcc,
                options => options.IncludeRuleSets(BankAccountValidator.IsValidModelForCreatingRules).ThrowOnFailures(),
                cancellation: cancellationToken);

            await _bankAccountRepository.CreateAsync(userId, currencyCode, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<decimal> CalculateTransferСommissionAsync(
            decimal amount, 
            int fromAccountId,
            int toAccountId,
            CancellationToken cancellationToken)
        {
            var fromAccount = await _bankAccountRepository.GetByIdAsync(fromAccountId, cancellationToken);  
            var toAccount = await _bankAccountRepository.GetByIdAsync(toAccountId, cancellationToken);

            return await CalculateTransferСommission(amount, fromAccount, toAccount, cancellationToken);
        }

        private async Task<decimal> CalculateTransferСommission(
            decimal amount,
            BankAccount fromAccount,
            BankAccount toAccount, 
            CancellationToken cancellationToken)
        {
            var tempTransaction = new Transaction()
            {
                Amount = amount,
                CurrencyCode = fromAccount.CurrencyCode,
                FromAccount = fromAccount,
                ToAccount = toAccount
            };

            await _transactionValidator.ValidateAsync(tempTransaction,
                options => options.IncludeRuleSets(TransactionsValidator.IsValidTransactionModelRules)
                    .ThrowOnFailures(), 
                cancellation: cancellationToken);

            return fromAccount.Owner.UserId == toAccount.Owner.UserId ? 0.00m : Math.Round(amount * CommissionAmount, 2);
        }

        public async Task ExecuteTransferAsync(
            decimal amount,
            int fromAccountId,
            int toAccountId,
            CancellationToken cancellationToken)
        {
            var fromAccount = await _bankAccountRepository.GetByIdAsync(fromAccountId, cancellationToken); 
            var toAccount = await _bankAccountRepository.GetByIdAsync(toAccountId, cancellationToken);

            var calculatedCommission = await CalculateTransferСommission(amount, fromAccount, toAccount, cancellationToken);
            var incomingAmount = await _currencyConverter.ConvertAsync(amount - calculatedCommission, 
                                                            fromAccount.CurrencyCode, 
                                                            toAccount.CurrencyCode,
                                                            cancellationToken);

            fromAccount.Balance -= Math.Round(amount, 2);
            toAccount.Balance += Math.Round(incomingAmount, 2);
            
            var transaction = new Transaction()
            {
                Amount = amount,
                CurrencyCode = fromAccount.CurrencyCode,
                FromAccount = fromAccount,
                ToAccount = toAccount,
            };

            await _transactionRepository.CreateAsync(transaction, cancellationToken);
            await _bankAccountRepository.UpdateAsync(fromAccount, cancellationToken);
            await _bankAccountRepository.UpdateAsync(toAccount, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task CloseAsync(int id, CancellationToken cancellationToken)
        {
            var dbBankAccount = await _bankAccountRepository.GetByIdAsync(id, cancellationToken);

            var tempBankAcc = new BankAccount() {Balance = dbBankAccount.Balance};

            await _bankAccountValidator.ValidateAsync(tempBankAcc,
                options => options.IncludeRuleSets(BankAccountValidator.IsValidModelForClosingRules)
                    .ThrowOnFailures(),
                cancellation: cancellationToken);

            await _bankAccountRepository.CloseAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public Task<List<BankAccount>> GetAllAsync(CancellationToken cancellationToken)
        {
            return _bankAccountRepository.GetAllAsync(cancellationToken);
        }

        public Task<BankAccount> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return _bankAccountRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task UpdateAsync(BankAccount bankAccount, CancellationToken cancellationToken)
        {
            await _bankAccountRepository.UpdateAsync(bankAccount, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            await _bankAccountRepository.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
