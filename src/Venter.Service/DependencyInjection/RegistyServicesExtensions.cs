using Microsoft.Extensions.DependencyInjection;
using Venter.Service.Interface;
using Venter.Service.Queue;
using Venter.Service.UserService;
using Miniblog.Core.Services.Mail;
using Miniblog.Core.Services.VerifyUser;

namespace Vivus.Services.DependencyInjection
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
