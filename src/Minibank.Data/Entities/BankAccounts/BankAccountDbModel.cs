using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Minibank.Data.Entities.Transactions;
using Minibank.Data.Entities.Enums;
using Minibank.Data.Entities.Users;

namespace Minibank.Data.Entities.BankAccounts
{
    public class BankAccountDbModel
    {
        public int BankAccountId { get; set; }
        public int OwnerId { get; set; }
        public virtual UserDbModel Owner { get; set; }
        public decimal Balance { get; set; }
        public CurrencyData CurrencyCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime OpeningTimestamp { get; set; }
        public DateTime? ClosingTimestamp { get; set; }
        public virtual List<TransactionDbModel> IncommingTransactions { get; set; }
        public virtual List<TransactionDbModel> OutcommingTransactions { get; set; }
    }

    internal class Map : IEntityTypeConfiguration<BankAccountDbModel>
    {
        public void Configure(EntityTypeBuilder<BankAccountDbModel> builder)
        {
            builder.ToTable("bank_accounts")
                .HasKey(p => p.BankAccountId)
                .HasName("pk_bank_accounts_id");

            builder.Property(p => p.BankAccountId);

            builder.Property(p => p.OwnerId);

            builder.Property(p => p.Balance);

            builder.Property(p => p.CurrencyCode)
                .HasConversion<string>();

            builder.Property(p => p.IsActive);

            builder.Property(p => p.OpeningTimestamp);

            builder.Property(p => p.ClosingTimestamp);

            builder.HasOne(p => p.Owner)
                .WithMany(p => p.BankAccounts)
                .HasForeignKey(p => p.OwnerId);

            builder.HasMany(p => p.OutcommingTransactions)
                .WithOne(p => p.FromAccount)
                .HasForeignKey(p => p.FromAccountId);

            builder.HasMany(p => p.IncommingTransactions)
                .WithOne(p => p.ToAccount)
                .HasForeignKey(p => p.ToAccountId);
        }
    }
}