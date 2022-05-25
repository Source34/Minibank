namespace Minibank.Core.Domains.Users.Services
{
    public interface IUserService
    {
        public Task<User> GetByIdAsync(int id, CancellationToken cancellationToken);
        public Task<List<User>> GetAllAsync(CancellationToken cancellationToken);
        public Task CreateAsync(User user, CancellationToken cancellationToken);
        public Task UpdateAsync(User user, CancellationToken cancellationToken);
        public Task DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
