using Moq;
using Xunit;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Minibank.Core.Services;
using Minibank.Core.Exceptions;
using System.Collections.Generic;
using Minibank.Core.Domains.Enums;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Messages;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.Transactions;
using Minibank.Core.Domains.BankAccounts.Services;
using Minibank.Core.Domains.BankAccounts.Validators;
using Minibank.Core.Domains.Transactions.Validators;
using Minibank.Core.Domains.Transactions.Repositories;
using Minibank.Core.Domains.BankAccounts.Repositories;

namespace Minibank.Core.Tests
{
    public class BankAccountServiceTests
    {   
        private readonly Mock<IUnitOfWork> _fakeUnitOfWork;
        private readonly IBankAccountService _bankAccountService;
        private readonly Mock<ICurrencyConverter> _fakeCurrencyConverter;
        private readonly Mock<ITransactionRepository> _fakeTransactionRepository;
        private readonly Mock<IBankAccountRepository> _fakeBankAccountRepository;

        public BankAccountServiceTests()
        {
            _fakeUnitOfWork = new Mock<IUnitOfWork>();
            _fakeCurrencyConverter = new Mock<ICurrencyConverter>();
            _fakeTransactionRepository = new Mock<ITransactionRepository>();
            _fakeBankAccountRepository = new Mock<IBankAccountRepository>();

            _bankAccountService = new BankAccountService(
                    _fakeBankAccountRepository.Object,
                    _fakeCurrencyConverter.Object,
                    _fakeTransactionRepository.Object,
                    _fakeUnitOfWork.Object, 
                    new BankAccountValidator(), 
                    new TransactionsValidator());
        }

        [Fact]
        public async Task GetByIdAsync_WhenBankAccountIsExists_ShouldReturnBankAccount()
        {
            //Arrange
            var bankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 100,
                CurrencyCode = CurrencyCore.RUB
            };

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(bankAccount);

            //Act
            var selectedBankAccount = await _bankAccountService.GetByIdAsync(bankAccount.BankAccountId, It.IsAny<CancellationToken>());

            //Assert
            Assert.Equal(selectedBankAccount.BankAccountId, bankAccount.BankAccountId);
            Assert.Equal(selectedBankAccount.Balance, bankAccount.Balance);
            Assert.Equal(selectedBankAccount.CurrencyCode, bankAccount.CurrencyCode);
        }
        
        [Fact]
        public async Task GetByIdAsync_WhenBankAccountDoesNotExist_ShouldThrowException()
        {
            //Arrange
            var bankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 100,
                CurrencyCode = CurrencyCore.RUB
            };

            
            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ObjectNotFoundException(
                                ErrorMessages.GetObjectNotFoundErrorMessage(
                                    ErrorMessages.GettingByIdErrorLegend,
                                    typeof(BankAccount),
                                    bankAccount.BankAccountId)));

            //Act/Assert
            var exception = await Assert.ThrowsAsync<ObjectNotFoundException>(() 
                => _bankAccountService.GetByIdAsync(bankAccount.BankAccountId, It.IsAny<CancellationToken>()));

            //Assert
            Assert.Equal(exception.GetType(), typeof(ObjectNotFoundException));
        }

        [Fact]
        public async Task GetAllAsync_WhenExistsAnyBankAccount_ShouldReturnList()
        {
            //Arrange
            _fakeBankAccountRepository
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<BankAccount>
                {
                    new ()
                    {
                        BankAccountId = 1,
                        Balance = 100,
                        CurrencyCode = CurrencyCore.RUB
                    },
                    new ()
                    {
                        BankAccountId = 2,
                        Balance = 150,
                        CurrencyCode = CurrencyCore.RUB
                    }
                });

            var result = await _bankAccountService.GetAllAsync(It.IsAny<CancellationToken>());

            //Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllAsync_WhenDoesNotExistsBankAccount_ShouldReturnList()
        {
            //Arrange
            _fakeBankAccountRepository
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<BankAccount>());

            //Act
            var result = await _bankAccountService.GetAllAsync(It.IsAny<CancellationToken>());

            //Assert
            Assert.Equal(0, result.Count);
        }

        [Fact]
        public async Task CreateAsync_SuccessCreating_ShouldVerifyUnitOfWork()
        {
            //Arrange
            var bankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 100,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            _fakeBankAccountRepository.Setup(x =>
                x.CreateAsync(It.IsAny<int>(), It.IsAny<CurrencyCore>(), It.IsAny<CancellationToken>()));

            //Act
            await _bankAccountService.CreateAsync(
                bankAccount.Owner.UserId, 
                bankAccount.CurrencyCode, 
                It.IsAny<CancellationToken>());

            //Assert
            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WrongCurrencyCode_ShouldThrowException()
        {
            //Arrange
            var bankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 100,
                Owner = new User() {UserId = 23 },
                CurrencyCode = (CurrencyCore)4
            };

            //Act/Assert
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(()
                => _bankAccountService.CreateAsync(
                    bankAccount.Owner.UserId, 
                    bankAccount.CurrencyCode, 
                    It.IsAny<CancellationToken>()));

            //Assert
            Assert.True(exception.Errors
                .Any(p => p.ErrorMessage == BankAccountValidatorMessages.InvalidCurrencyCode));

            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_OwnerDoesNotExist_ShouldThrowException()
        {
            //Arrange
            var bankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 100,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            _fakeBankAccountRepository
                .Setup(x => x.CreateAsync(
                    It.IsAny<int>(), 
                    It.IsAny<CurrencyCore>(), 
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ObjectNotFoundException(
                    ErrorMessages.GetObjectNotFoundErrorMessage(
                        ErrorMessages.CreatingErrorLegend, 
                        typeof(User), 
                        bankAccount.Owner.UserId)));

            //Act/Assert
             await Assert.ThrowsAsync<ObjectNotFoundException>(() => _bankAccountService.CreateAsync(
                    bankAccount.Owner.UserId,
                    bankAccount.CurrencyCode,
                    It.IsAny<CancellationToken>()));

             //Assert
            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_SuccessUpdating_ShouldVerifyUnitOfWork()
        {
            //Arrange
            var bankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 100,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            _fakeBankAccountRepository
                .Setup(x => x.UpdateAsync(It.IsAny<BankAccount>(), It.IsAny<CancellationToken>()));

            //Act
            await _bankAccountService.UpdateAsync(bankAccount, It.IsAny<CancellationToken>());

            //Assert
            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_BankAccountDoesNotExist_ShouldThrowException()
        {
            //Arrange
            var bankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 100,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            _fakeBankAccountRepository
                .Setup(x => x.UpdateAsync(
                    It.IsAny<BankAccount>(), 
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ObjectNotFoundException(
                    ErrorMessages.GetObjectNotFoundErrorMessage(
                        ErrorMessages.UpdatingErrorLegend,
                        typeof(BankAccount),
                        bankAccount.BankAccountId)));

            //Act
            await Assert.ThrowsAsync<ObjectNotFoundException>(() 
                => _bankAccountService.UpdateAsync(bankAccount, It.IsAny<CancellationToken>()));

            //Assert
            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_SuccessDeleting_ShouldVerifyUnitOfWork()
        {
            //Arrange
            var bankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 100,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            _fakeBankAccountRepository
                .Setup(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()));

            //Act
            await _bankAccountService.DeleteAsync(bankAccount.BankAccountId, It.IsAny<CancellationToken>());

            //Assert
            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_BankAccountDoesNotExist_ShouldThrowException()
        {
            //Arrange
            var bankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 100,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            _fakeBankAccountRepository
                .Setup(x => x.DeleteAsync(
                    It.IsAny<int>(), 
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(
                    new ObjectNotFoundException(
                        ErrorMessages.GetObjectNotFoundErrorMessage(
                            ErrorMessages.CreatingErrorLegend, 
                            typeof(BankAccount), 
                            bankAccount.BankAccountId)));

            //Act/Assert
            var exception = await Assert.ThrowsAsync<ObjectNotFoundException>(()
                => _bankAccountService.DeleteAsync(bankAccount.BankAccountId, It.IsAny<CancellationToken>()));

            //Assert
            Assert.Equal(
                exception.Message, 
                ErrorMessages.GetObjectNotFoundErrorMessage(
                    ErrorMessages.CreatingErrorLegend, 
                    typeof(BankAccount), 
                    bankAccount.BankAccountId));

            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CloseAsync_SuccessClosing_ShouldVerifyUnitOfWork()
        {
            //Arrange
            var bankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 0,
                IsActive = true,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(bankAccount);

            //Act
            await _bankAccountService.CloseAsync(bankAccount.BankAccountId, It.IsAny<CancellationToken>());

            //Assert
            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CloseAsync_BankAccountNotFoundInClosing_ShouldThrowException()
        {
            //Arrange
            var bankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 0,
                IsActive = true,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(bankAccount);
            
            _fakeBankAccountRepository
                .Setup(x => x.CloseAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(
                    new ObjectNotFoundException(
                        ErrorMessages.GetObjectNotFoundErrorMessage(
                            ErrorMessages.ClosingBankAccountErrorLegend,
                            typeof(BankAccount),
                            bankAccount.BankAccountId)));

            //Act/Assert
            var exception = await Assert.ThrowsAsync<ObjectNotFoundException>(()
                => _bankAccountService.CloseAsync(bankAccount.BankAccountId, It.IsAny<CancellationToken>()));

            //Assert
            Assert.Equal(
                exception.Message,
                ErrorMessages.GetObjectNotFoundErrorMessage(
                    ErrorMessages.ClosingBankAccountErrorLegend,
                    typeof(BankAccount),
                    bankAccount.BankAccountId));

            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
        
        [Fact]
        public async Task CloseAsync_BankAccountNotFoundInGetById_ShouldThrowException()
        {
            //Arrange
            var bankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 0,
                IsActive = true,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ObjectNotFoundException(
                    ErrorMessages.GetObjectNotFoundErrorMessage(
                        ErrorMessages.GettingByIdErrorLegend,
                        typeof(BankAccount),
                        bankAccount.BankAccountId)));

            //Act/Assert
            var exception = await Assert.ThrowsAsync<ObjectNotFoundException>(()
                => _bankAccountService.CloseAsync(bankAccount.BankAccountId, It.IsAny<CancellationToken>()));

            //Assert
            Assert.Equal(
                exception.Message, 
                ErrorMessages.GetObjectNotFoundErrorMessage(
                    ErrorMessages.GettingByIdErrorLegend,
                    typeof(BankAccount),
                    bankAccount.BankAccountId));

            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CalculateTransferСommissionAsync_SuccessWayDiffOwners_ReturnDecimal()
        {
            //Arrange
            decimal amount = 53;

            var fromBankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 100,
                IsActive = true,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            var toBankAccount = new BankAccount()
            {
                BankAccountId = 2,
                Balance = 100,
                IsActive = true,
                Owner = new User() { UserId = 24 },
                CurrencyCode = CurrencyCore.RUB
            };

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == fromBankAccount.BankAccountId), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromBankAccount);

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == toBankAccount.BankAccountId), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(toBankAccount);

            //Act
            decimal res = await _bankAccountService.CalculateTransferСommissionAsync(
                                amount,
                                fromBankAccount.BankAccountId,
                                toBankAccount.BankAccountId, 
                                It.IsAny<CancellationToken>());

            //Assert
            Assert.Equal((decimal)1.06, res);
        }

        [Fact]
        public async Task CalculateTransferСommissionAsync_SuccessWaySingleOwner_ReturnDecimal()
        {
            //Arrange
            decimal amount = 50;

            var fromBankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 100,
                IsActive = true,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            var toBankAccount = new BankAccount()
            {
                BankAccountId = 2,
                Balance = 100,
                IsActive = true,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == fromBankAccount.BankAccountId), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromBankAccount);

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == toBankAccount.BankAccountId), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(toBankAccount);

            //Act
            decimal res = await _bankAccountService.CalculateTransferСommissionAsync(
                                amount,
                                fromBankAccount.BankAccountId,
                                toBankAccount.BankAccountId,
                                It.IsAny<CancellationToken>());

            //Assert
            Assert.Equal(0, res);
        }

        [Fact]
        public async Task CalculateTransferСommissionAsync_InvalidAmount_ShouldThrowException()
        {
            //Arrange
            decimal amount = -50;

            var fromBankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 100,
                IsActive = true,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            var toBankAccount = new BankAccount()
            {
                BankAccountId = 2,
                Balance = 100,
                IsActive = true,
                Owner = new User() { UserId = 24 },
                CurrencyCode = CurrencyCore.RUB
            };

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == fromBankAccount.BankAccountId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromBankAccount);

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == toBankAccount.BankAccountId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(toBankAccount);

            //Act
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(()
                    => _bankAccountService.CalculateTransferСommissionAsync(
                            amount,
                            fromBankAccount.BankAccountId,
                            toBankAccount.BankAccountId,
                            It.IsAny<CancellationToken>()));

            //Assert
            Assert.True(exception.Errors
                .Any(p => p.ErrorMessage == TransactionsValidatorMessages.InvalidAmount));
        }

        [Fact]
        public async Task CalculateTransferСommissionAsync_IsNotActiveAccounts_ShouldThrowException()
        {
            //Arrange
            decimal amount = 50;

            var fromBankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 100,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            var toBankAccount = new BankAccount()
            {
                BankAccountId = 2,
                Balance = 100,
                Owner = new User() { UserId = 24 },
                CurrencyCode = CurrencyCore.RUB
            };

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == fromBankAccount.BankAccountId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromBankAccount);

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == toBankAccount.BankAccountId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(toBankAccount);

            //Act/Assert
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(()
                => _bankAccountService.CalculateTransferСommissionAsync(
                    amount,
                    fromBankAccount.BankAccountId,
                    toBankAccount.BankAccountId,
                    It.IsAny<CancellationToken>()));

            //Assert
            Assert.True(exception.Errors
                .Any(p => p.ErrorMessage == TransactionsValidatorMessages.FromAccountNotActive));

            Assert.True(exception.Errors
                .Any(p => p.ErrorMessage == TransactionsValidatorMessages.ToAccountNotActive));
        }

        [Fact]
        public async Task CalculateTransferСommissionAsync_NotEnoughMoney_ShouldThrowException()
        {
            //Arrange
            decimal amount = 150;

            var fromBankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 100,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            var toBankAccount = new BankAccount()
            {
                BankAccountId = 2,
                Balance = 100,
                Owner = new User() { UserId = 24 },
                CurrencyCode = CurrencyCore.RUB
            };

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == fromBankAccount.BankAccountId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromBankAccount);

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == toBankAccount.BankAccountId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(toBankAccount);

            //Act/Assert
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(()
                => _bankAccountService.CalculateTransferСommissionAsync(
                    amount,
                    fromBankAccount.BankAccountId,
                    toBankAccount.BankAccountId,
                    It.IsAny<CancellationToken>()));

            //Assert
            Assert.True(exception.Errors
                .Any(p => p.ErrorMessage == TransactionsValidatorMessages.NotEnoughMoney));
        }

        [Fact]
        public async Task ExecuteTransferAsync_SuccessWayDiffOwners_ShouldVerifyUnitOfWork()
        {
            //Arrange
            decimal amount = 25;

            var fromBankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 100,
                IsActive = true,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            var toBankAccount = new BankAccount()
            {
                BankAccountId = 2,
                Balance = 100,
                IsActive = true,
                Owner = new User() { UserId = 24 },
                CurrencyCode = CurrencyCore.RUB
            };

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == fromBankAccount.BankAccountId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromBankAccount);

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == toBankAccount.BankAccountId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(toBankAccount);

            _fakeCurrencyConverter
                .Setup(x => x.ConvertAsync(
                    amount, 
                    fromBankAccount.CurrencyCode, 
                    toBankAccount.CurrencyCode,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(amount);

            //Act
            await _bankAccountService.ExecuteTransferAsync(
                amount, 
                fromBankAccount.BankAccountId, 
                toBankAccount.BankAccountId, 
                It.IsAny<CancellationToken>());

            //Assert
            _fakeTransactionRepository.Verify(x => x.CreateAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Once);
            _fakeBankAccountRepository.Verify(x => x.UpdateAsync(It.IsAny<BankAccount>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteTransferAsync_SuccessWayDiffOwnersDiffCurrencies_ShouldVerifyUnitOfWork()
        {
            //Arrange
            decimal amount = 25;

            var fromBankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 100,
                IsActive = true,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            var toBankAccount = new BankAccount()
            {
                BankAccountId = 2,
                Balance = 100,
                IsActive = true,
                Owner = new User() { UserId = 24 },
                CurrencyCode = CurrencyCore.USD
            };

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == fromBankAccount.BankAccountId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromBankAccount);

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == toBankAccount.BankAccountId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(toBankAccount);

            //Act
            await _bankAccountService.ExecuteTransferAsync(
                amount,
                fromBankAccount.BankAccountId,
                toBankAccount.BankAccountId,
                It.IsAny<CancellationToken>());

            //Assert
            _fakeTransactionRepository.Verify(x => x.CreateAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Once);
            _fakeBankAccountRepository.Verify(x => x.UpdateAsync(It.IsAny<BankAccount>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteTransferAsync_InvalidAmount_ShouldThrowException()
        {
            //Arrange
            decimal amount = -25;

            var fromBankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 100,
                IsActive = true,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            var toBankAccount = new BankAccount()
            {
                BankAccountId = 2,
                Balance = 100,
                IsActive = true,
                Owner = new User() { UserId = 24 },
                CurrencyCode = CurrencyCore.RUB
            };

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == fromBankAccount.BankAccountId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromBankAccount);

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == toBankAccount.BankAccountId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(toBankAccount);

            //Act/Assert
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(()
                => _bankAccountService.ExecuteTransferAsync(
                        amount,
                        fromBankAccount.BankAccountId,
                        toBankAccount.BankAccountId,
                        It.IsAny<CancellationToken>()));

            //Assert
            Assert.True(exception.Errors
                .Any(p => p.ErrorMessage == TransactionsValidatorMessages.InvalidAmount));
        }
        
        [Fact]
        public async Task ExecuteTransferAsync_NotEnoughMoney_ShouldThrowException()
        {
            //Arrange
            decimal amount = 164;

            var fromBankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 100,
                IsActive = false,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            var toBankAccount = new BankAccount()
            {
                BankAccountId = 2,
                Balance = 100,
                IsActive = false,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == fromBankAccount.BankAccountId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromBankAccount);

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == toBankAccount.BankAccountId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(toBankAccount);

            //Act/Assert
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(()
                => _bankAccountService.ExecuteTransferAsync(
                    amount,
                    fromBankAccount.BankAccountId,
                    toBankAccount.BankAccountId,
                    It.IsAny<CancellationToken>()));

            Assert.True(exception.Errors
                .Any(p => p.ErrorMessage == TransactionsValidatorMessages.NotEnoughMoney));
        }

        [Fact]
        public async Task ExecuteTransferAsync_IsNotActiveAccounts_ShouldThrowException()
        {
            //Arrange
            decimal amount = 64;

            var fromBankAccount = new BankAccount()
            {
                BankAccountId = 1,
                Balance = 100,
                IsActive = false,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            var toBankAccount = new BankAccount()
            {
                BankAccountId = 2,
                Balance = 100,
                IsActive = false,
                Owner = new User() { UserId = 23 },
                CurrencyCode = CurrencyCore.RUB
            };

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == fromBankAccount.BankAccountId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromBankAccount);

            _fakeBankAccountRepository
                .Setup(x => x.GetByIdAsync(It.Is<int>(p => p == toBankAccount.BankAccountId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(toBankAccount);

            //Act/Assert
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(()
                => _bankAccountService.ExecuteTransferAsync(
                    amount,
                    fromBankAccount.BankAccountId,
                    toBankAccount.BankAccountId,
                    It.IsAny<CancellationToken>()));

            //Assert
            Assert.True(exception.Errors
                .Any(p => p.ErrorMessage == TransactionsValidatorMessages.FromAccountNotActive));

            Assert.True(exception.Errors
                .Any(p => p.ErrorMessage == TransactionsValidatorMessages.ToAccountNotActive));
        }
    }
}
