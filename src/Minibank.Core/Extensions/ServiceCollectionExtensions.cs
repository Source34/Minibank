using Minibank.Core.Domains.BankAccounts.Validators;
using Minibank.Core.Domains.Transactions.Validators;
using Minibank.Core.Domains.BankAccounts.Services;
using Minibank.Core.Domains.Transactions.Services;
using Microsoft.Extensions.DependencyInjection;
using Minibank.Core.Domains.Users.Validators;
using Minibank.Core.Domains.Users.Services;
using MiniBank.Core.Domains.Users.Services;
using Minibank.Core.Services;
using FluentValidation;

namespace Minibank.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddScoped<ICurrencyConverter, CurrencyConverter>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddValidatorsFromAssembly(typeof(UserValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(BankAccountValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(TransactionsValidator).Assembly);
            return services;
        }
    }
}