using Minibank.Core.Services;

namespace Minibank.Data.Contexts
{
    public class MinibankUnitOfWork : IUnitOfWork
    {
        private readonly MinibankContext _context;

        public MinibankUnitOfWork(MinibankContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
