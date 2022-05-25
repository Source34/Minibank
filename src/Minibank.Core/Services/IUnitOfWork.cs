namespace Minibank.Core.Services
{
    public interface IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
