using Minibank.Core.Services;
using Minibank.Data.Contexts;
using Minibank.Data.HttpClients;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Data.Entities.Users.Repositories;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.Transactions.Repositories;
using Minibank.Data.Entities.BankAccounts.Repositories;
using Minibank.Data.Entities.Transactions.Repositories;

namespace Minibank.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
           var ss =  configuration["ExchangeSourceUrl"];
            services.AddHttpClient<ICurrencyExchangeProvider, CurrencyExchangeProvider>(options =>
            {
                options.BaseAddress = new Uri(configuration["ExchangeSourceUrl"]);
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IUnitOfWork, MinibankUnitOfWork>();
            services.AddDbContext<MinibankContext>(options => options
                                    .UseNpgsql(configuration["ConnectionString"])
                                    .UseSnakeCaseNamingConvention());

            return services;
        }
    }
}