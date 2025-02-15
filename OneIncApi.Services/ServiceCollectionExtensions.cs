using Microsoft.Extensions.DependencyInjection;
using OneIncApi.Services.Interfaces;
using OneIncApi.Services.Services;
using OneIncApi.Services.Validators;
using FluentValidation;

namespace OneIncApi.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();

            services.AddScoped<IUserService, UserService>();
            return services;
        }
    }
}
