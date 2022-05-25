using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minibank.Data.Entities.BankAccounts;
using Minibank.Data.Entities.Enums;

namespace Minibank.Data.Entities.Transactions
{
    public class TransactionDbModel
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public CurrencyData CurrencyCode { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
        public virtual BankAccountDbModel FromAccount { get; set; }
        public virtual BankAccountDbModel ToAccount{ get; set; }
    }

    internal class Map : IEntityTypeConfiguration<TransactionDbModel>
    {
        public void Configure(EntityTypeBuilder<TransactionDbModel> builder)
        {
            builder.ToTable("transactions")
                .HasKey(p => p.TransactionId)
                .HasName("pk_transactions_id");

            builder.Property(p => p.TransactionId);

            builder.Property(p => p.Amount);

            builder.Property(p => p.CurrencyCode)
                .HasConversion<string>();

            builder.Property(p => p.FromAccountId);

            builder.Property(p => p.ToAccountId);
        }
    }
}