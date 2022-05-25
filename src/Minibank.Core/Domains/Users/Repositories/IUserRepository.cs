namespace Minibank.Core.Domains.Users.Repositories
{
    public interface IUserRepository
    {
        public Task<User> GetByIdAsync(int userId, CancellationToken cancellationToken);
        public Task<List<User>> GetAllAsync(CancellationToken cancellationToken);
        public Task CreateAsync(User user, CancellationToken cancellationToken);
        public Task UpdateAsync(User user, CancellationToken cancellationToken);
        public Task DeleteAsync(int userId, CancellationToken cancellationToken);
        public Task<bool> IsExistUserAsync(int userId, CancellationToken cancellationToken);
        public Task<bool> IsUniqeAsync(string login, string email, CancellationToken cancellationToken);
    }
}