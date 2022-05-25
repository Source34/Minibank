using Microsoft.EntityFrameworkCore;
using Minibank.Data.Entities.BankAccounts;
using Minibank.Data.Entities.Transactions;
using Minibank.Data.Entities.Users;

namespace Minibank.Data.Contexts
{
    public class MinibankContext : DbContext
    {
        public DbSet<UserDbModel> Users { get; set; }
        public DbSet<TransactionDbModel> Transactions { get; set; }
        public DbSet<BankAccountDbModel> BankAccounts { get; set; }

        public MinibankContext() { }
        public MinibankContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MinibankContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);
            base.OnConfiguring(optionsBuilder);
        }
    }
}