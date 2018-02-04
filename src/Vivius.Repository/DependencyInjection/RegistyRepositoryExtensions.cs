using Microsoft.Extensions.DependencyInjection;
using Miniblog.Core.Interface.Repositories;
using Miniblog.Core.Repositories.User;
using Miniblog.Core.Repository.Mail;
using Miniblog.Core.Repository.Qeue;
using Miniblog.Core.Repository.VerifyUser;

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
