using Microsoft.Extensions.DependencyInjection;
using Vivius.Interface.Repositories;
using Vivius.Repositories.User;
using Vivius.Repository.Mail;
using Vivius.Repository.Qeue;
using Vivius.Repository.VerifyUser;

namespace Vivus.Repository.DependencyInjection
{
    public static class RegistyRepositoryExtensions
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMailRepository, MailRepository>();
            services.AddScoped<IVerifyUserRepository, VerifyUserRepository>();
            services.AddScoped<IVerifyUserRepository, VerifyUserRepository>();
            services.AddScoped<IQeueMessageRepository, AzureQeueMessageRepository>();

            return services;
        }        
    }
}
