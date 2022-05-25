using FluentValidation;
using Minibank.Core.Services;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Messages;
using Minibank.Core.Domains.Users.Services;
using Minibank.Core.Domains.Users.Validators;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Domains.BankAccounts.Repositories;
using ValidationException = Minibank.Core.Exceptions.ValidationException;

namespace MiniBank.Core.Domains.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<User> _userValidator;

        public UserService(IUserRepository userRepository, 
                           IBankAccountRepository bankAccountRepository,
                           IUnitOfWork unitOfWork,
                           IValidator<User> userValidator)
        {
            _userRepository = userRepository;
            _bankAccountRepository = bankAccountRepository;
            _unitOfWork = unitOfWork;
            _userValidator = userValidator;
        }

        public Task<User> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return _userRepository.GetByIdAsync(id, cancellationToken);
        }

        public Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
        {
            return _userRepository.GetAllAsync(cancellationToken);
        }

        public async Task CreateAsync(User user, CancellationToken cancellationToken)
        {
            await _userValidator.ValidateAsync(user, 
                options => options.IncludeRuleSets(UserValidator.IsValidLoginAndEmailRules)
                    .ThrowOnFailures(), 
                cancellation: cancellationToken);

            if (!await _userRepository.IsUniqeAsync(user.Login, user.Email, cancellationToken))
                throw new ValidationException(ErrorMessages.UserLoginOrEmailIsNotUniqe);


            await _userRepository.CreateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken)
        {
            await _userValidator.ValidateAsync(user,
                options => options.IncludeRuleSets(UserValidator.IsValidLoginAndEmailRules)
                    .ThrowOnFailures(),
                cancellation: cancellationToken);

            await _userRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            if(await _bankAccountRepository.IsExistActiveBankAccountsAsync(id, cancellationToken))
                throw new ValidationException(ErrorMessages.GetUserDeletingWithOpenBankAccountErrorMessage(id));

            await _userRepository.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}