using Microsoft.Extensions.DependencyInjection;
using Multiblog.Core.Interface.Repositories;
using Multiblog.Core.Repositories.User;
using Multiblog.Core.Repository.Mail;
using Multiblog.Core.Repository.Qeue;
using Multiblog.Core.Repository.VerifyUser;

namespace Multiblog.Repository.DependencyInjection
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
