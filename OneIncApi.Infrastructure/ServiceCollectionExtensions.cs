using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneIncApi.Infrastructure.Data;
using OneIncApi.Infrastructure.Repositories;
using OneIncApi.Services.Interfaces;

namespace OneIncApi.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                 .AddScoped<ISqlConnectionFactory, SqlConnectionFactory>()
                 .AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
