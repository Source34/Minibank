using AutoMapper;
using Minibank.Data.Contexts;
using Minibank.Core.Exceptions;
using Minibank.Core.Domains.Users;
using Microsoft.EntityFrameworkCore;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.Messages;
using Minibank.Core.Domains.Users.Repositories;

namespace Minibank.Data.Entities.Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMapper _mapper;
        private readonly MinibankContext _context;

        public UserRepository(MinibankContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<User> GetByIdAsync(int userId, CancellationToken cancellationToken)
        {
            var dbUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(it => it.UserId == userId, cancellationToken);


            if (dbUser == null)
            {
                throw new ObjectNotFoundException(
                    ErrorMessages.GetObjectNotFoundErrorMessage(
                        ErrorMessages.GettingByIdErrorLegend,
                        typeof(User),
                        userId));
            }

            return _mapper.Map<User>(dbUser);
        }

        public Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
        {
            return _context.Users
                .AsNoTracking()
                .Select(user => _mapper.Map<User>(user))
                .ToListAsync(cancellationToken);
        }

        public async Task CreateAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(_mapper.Map<UserDbModel>(user), cancellationToken);
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var dbUser = await _context.Users
                .FirstOrDefaultAsync(it => it.UserId == user.UserId, cancellationToken);

            if (dbUser == null)
            {
                throw new ObjectNotFoundException(
                    ErrorMessages.GetObjectNotFoundErrorMessage(
                        ErrorMessages.UpdatingErrorLegend,
                        typeof(User),
                        user.UserId));
            }

            dbUser.Login = user.Login;
            dbUser.Email = user.Email;
        }

        public async Task DeleteAsync(int userId, CancellationToken cancellationToken)
        {
            var dbUser = await _context.Users
                .FirstOrDefaultAsync(it => it.UserId == userId, cancellationToken);

            if (dbUser == null)
            {
                throw new ObjectNotFoundException(
                    ErrorMessages.GetObjectNotFoundErrorMessage(
                        ErrorMessages.DeletingErrorLegend,
                        typeof(User),
                        userId));
            }

            _context.Users.Remove(dbUser);
        }

        public async Task<bool> IsExistUserAsync(int userId, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AnyAsync(p => p.UserId == userId, cancellationToken);
        }

        public async Task<bool> IsUniqeAsync(string login, string email, CancellationToken cancellationToken)
        {
            if (!await _context.Users.AnyAsync(cancellationToken))
                return true;

            return !await _context.Users
                .AnyAsync(p => p.Login == login || p.Email == email, cancellationToken);
        }
    }
}