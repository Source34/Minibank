using Moq;
using Xunit;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Minibank.Core.Services;
using Minibank.Core.Exceptions;
using System.Collections.Generic;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Messages;
using Minibank.Core.Domains.Users.Services;
using MiniBank.Core.Domains.Users.Services;
using Minibank.Core.Domains.Users.Validators;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Domains.BankAccounts.Repositories;
using ValidationException = Minibank.Core.Exceptions.ValidationException;

namespace Minibank.Core.Tests
{
    public class UserServiceTests
    {
        private readonly IUserService _userService;
        private readonly Mock<IBankAccountRepository> _fakeBankAccountRepository;
        private readonly Mock<IUserRepository> _fakeUserRepository;
        private readonly Mock<IUnitOfWork> _fakeUnitOfWork;

        public UserServiceTests()
        {
            _fakeUserRepository = new Mock<IUserRepository>();
            _fakeBankAccountRepository = new Mock<IBankAccountRepository>();
            _fakeUnitOfWork = new Mock<IUnitOfWork>();

            _userService = new UserService(
                    _fakeUserRepository.Object,
                    _fakeBankAccountRepository.Object,
                    _fakeUnitOfWork.Object, 
                    new UserValidator());
        }

        [Fact]
        public async Task GetByIdAsync_WhenUserExists_ShouldReturnUser()
        {
            //Arrange
            var user = new User()
            {
                UserId = 1,
                Login = "TestLogin",
                Email = "TestEmail"
            };

            _fakeUserRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            //Act
            var userSelected = await _userService.GetByIdAsync(user.UserId, It.IsAny<CancellationToken>());

            //Assert
            Assert.Equal(userSelected.UserId, user.UserId);
            Assert.Equal(userSelected.Login, user.Login);
            Assert.Equal(userSelected.Email, user.Email);
        }

        [Fact]
        public async Task GetByIdAsync_WhenUserDoesNotExists_ShouldThrowException()
        {
            //Arrange
            _fakeUserRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Throws(() => new ObjectNotFoundException(
                    ErrorMessages.GetObjectNotFoundErrorMessage(
                        ErrorMessages.GettingByIdErrorLegend,
                        typeof(User),
                        It.IsAny<int>())));

            //Act/Assert
            var exception = await Assert.ThrowsAsync<ObjectNotFoundException>(() 
                => _userService.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()));

            //Assert
            Assert.Equal(
                exception.Message, 
                ErrorMessages.GetObjectNotFoundErrorMessage(ErrorMessages.GettingByIdErrorLegend, typeof(User), It.IsAny<int>()));
        }

        [Fact]
        public async Task GetAllAsync_WhenExistsAnyUsers_ShouldReturnList()
        {
            //Arrange
            _fakeUserRepository
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>
                {
                    new ()
                    {
                        UserId = 1,
                        Login = "login_1",
                        Email = "email_1",
                    },
                    new ()
                    {
                        UserId = 2,
                        Login = "login_2",
                        Email = "email_2",
                    }
                });

            //Act
            var result = await _userService.GetAllAsync(It.IsAny<CancellationToken>());

            //Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllAsync_WhenDoesNotExistsUsers_ShouldReturnList()
        {
            //Arrange
            _fakeUserRepository
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>());

            //Act
            var result = await _userService.GetAllAsync(It.IsAny<CancellationToken>());

            //Assert
            Assert.Equal(0, result.Count);
        }

        [Fact]
        public async Task CreateAsync_SuccessWay_ShouldVerifyUnitOfWork()
        {
            //Arrange
            var user = new User()
            {
                UserId = 1,
                Login = "TestLogin",
                Email = "TestEmail"
            };

            _fakeUserRepository
                .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()));

            _fakeUserRepository.Setup(x => x.IsUniqeAsync(user.Login, user.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await _userService.CreateAsync(user, It.IsAny<CancellationToken>());

            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_UserIsNotUniqe_ShouldThrowException()
        {
            //Arrange
            var user = new User()
            {
                UserId = 1,
                Login = "TestLogin",
                Email = "TestEmail"
            };

            _fakeUserRepository
                .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()));

            _fakeUserRepository
                .Setup(x => x.IsUniqeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            //Act/Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() 
                => _userService.CreateAsync(user, It.IsAny<CancellationToken>()));

            //Assert
            Assert.Equal(exception.Message, ErrorMessages.UserLoginOrEmailIsNotUniqe);
        }

        [Fact]
        public async Task CreateAsync_NotValidLoginOrEmail_ShouldThrowException()
        {
            //Arrange
            var user = new User()
            {
                UserId = 1,
                Login = "TestLoginGreaterThenIsAccess",
                Email = "TestLoginGreaterThenIsAccess"
            };

            _fakeUserRepository
                .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()));

            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(()
                => _userService.CreateAsync(user, It.IsAny<CancellationToken>()));

            Assert.True(exception.Errors
                .Any(p => p.ErrorMessage == UserValidatorMessages.LoginIsTooLong));

            Assert.True(exception.Errors
                .Any(p => p.ErrorMessage == UserValidatorMessages.EmailIsTooLong));

            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_EmptyLoginOrEmail_ShouldThrowException()
        {
            //Arrange
            var user = new User()
            {
                UserId = 1,
                Login = "",
                Email = ""
            };

            _fakeUserRepository
                .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()));

            //Act/Assert
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(()
                => _userService.CreateAsync(user, It.IsAny<CancellationToken>()));

            //Assert
            Assert.True(exception.Errors
                .Any(p => p.ErrorMessage == UserValidatorMessages.LoginIsNullOrEmpty));

            Assert.True(exception.Errors
                .Any(p => p.ErrorMessage == UserValidatorMessages.EmailIsNullOrEmpty));

            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_SuccessUpdating_VerifyUnitOfWork()
        {
            //Arrange
            var user = new User()
            {
                UserId = 1,
                Login = "TestLogin",
                Email = "TestEmail"
            };

            _fakeUserRepository
                .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()));

            //Act
            await _userService.UpdateAsync(user, It.IsAny<CancellationToken>());

            //Assert
            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async Task UpdateAsync_UserNotFound_ShouldThrowException()
        {
            //Arrange
            var user = new User()
            {
                UserId = 1,
                Login = "TestLogin",
                Email = "TestEmail"
            };

            _fakeUserRepository
                .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ObjectNotFoundException(
                    ErrorMessages.GetObjectNotFoundErrorMessage(
                        ErrorMessages.UpdatingErrorLegend,
                        typeof(User),
                        user.UserId)));

            //Act/Assert
            var exception = await Assert.ThrowsAsync<ObjectNotFoundException>(() 
                => _userService.UpdateAsync(user, It.IsAny<CancellationToken>()));

            //Assert
            Assert.Equal(
                exception.Message, 
                ErrorMessages.GetObjectNotFoundErrorMessage(
                    ErrorMessages.UpdatingErrorLegend, 
                    typeof(User), 
                    user.UserId));

            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
        
        [Fact]
        public async Task UpdateAsync_NotValidLoginOrEmail_ShouldThrowException()
        {
            //Arrange
            var user = new User()
            {
                UserId = 1,
                Login = "",
                Email = "TestEmailGreaterThenIsAccess"
            };

            _fakeUserRepository
                .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()));

            //Act/Assert
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(()
                => _userService.UpdateAsync(user, It.IsAny<CancellationToken>()));

            //Assert
            Assert.True(exception.Errors
                .Any(p => p.ErrorMessage == UserValidatorMessages.LoginIsNullOrEmpty));

            Assert.True(exception.Errors
                .Any(p => p.ErrorMessage == UserValidatorMessages.EmailIsTooLong));

            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_SuccessDeleting_VerifyUnitOfWork()
        {
            //Arrange
            var user = new User()
            {
                UserId = 1,
                Login = "TestLogin",
                Email = "TestEmail"
            };

            _fakeBankAccountRepository
                .Setup(p => p.IsExistActiveBankAccountsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _fakeUserRepository
                .Setup(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()));

            //Act
            await _userService.DeleteAsync(user.UserId, It.IsAny<CancellationToken>());

            //Assert
            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ExistActiveBankAccount_ShouldThrowException()
        {
            //Arrange
            var user = new User()
            {
                UserId = 1,
                Login = "TestLogin",
                Email = "TestEmail"
            };

            _fakeUserRepository
                .Setup(x => x.DeleteAsync(It.IsAny<int>(), CancellationToken.None));

            _fakeBankAccountRepository
                .Setup(p => p.IsExistActiveBankAccountsAsync(
                    It.Is<int>(x => x == user.UserId), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            //Act/Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(()
                => _userService.DeleteAsync(user.UserId, It.IsAny<CancellationToken>()));

            //Assert
            Assert.Equal(
                exception.Message, 
                ErrorMessages.GetUserDeletingWithOpenBankAccountErrorMessage(user.UserId));

            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_UserNotFound_ShouldThrowException()
        {
            //Arrange
            var user = new User()
            {
                UserId = 1,
                Login = "TestLogin",
                Email = "TestEmail"
            };

            _fakeUserRepository
                .Setup(x => x.DeleteAsync(It.IsAny<int>(), CancellationToken.None))
                .ThrowsAsync(new ObjectNotFoundException(
                    ErrorMessages.GetObjectNotFoundErrorMessage(
                        ErrorMessages.DeletingErrorLegend,
                        typeof(User),
                        user.UserId)));

            _fakeBankAccountRepository
                .Setup(p => p.IsExistActiveBankAccountsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            //Act/Assert
            var exception = await Assert.ThrowsAsync<ObjectNotFoundException>(()
                => _userService.DeleteAsync(user.UserId, It.IsAny<CancellationToken>()));

            //Assert
            Assert.Equal(exception.Message, 
                ErrorMessages.GetObjectNotFoundErrorMessage(
                    ErrorMessages.DeletingErrorLegend, 
                    typeof(User), 
                    user.UserId));

            _fakeUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}