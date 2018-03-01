using Microsoft.Extensions.DependencyInjection;
using Multiblog.Service.Interface;
using Multiblog.Service.Queue;
using Multiblog.Service.UserService;
using Multiblog.Core.Services.Mail;
using Multiblog.Core.Services.VerifyUser;

namespace Multiblog.Services.DependencyInjection
{
    public static class RegistyServicesExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IVerifyUserServices, VerifyUserServices>();
            services.AddScoped<IVerifyUserServices, VerifyUserServices>();
            services.AddScoped<IQeueMessageServices, QeueMessageServices>();

            return services;
        }
    }
}
