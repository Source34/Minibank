using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minibank.Data.Entities.BankAccounts;

namespace Minibank.Data.Entities.Users
{
    public class UserDbModel
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public virtual List<BankAccountDbModel> BankAccounts { get; set; }
    }

    internal class Map : IEntityTypeConfiguration<UserDbModel>
    {
        public void Configure(EntityTypeBuilder<UserDbModel> builder)
        {
            builder.ToTable("bank_users")
                .HasKey(p => p.UserId)
                .HasName("pk_user_id");

            builder.Property(p => p.UserId);

            builder.Property(p => p.Login);

            builder.Property(p => p.Email);
        }
    }
}