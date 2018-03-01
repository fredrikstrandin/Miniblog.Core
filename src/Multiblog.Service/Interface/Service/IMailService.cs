using System.Threading.Tasks;
using Multiblog.Model.Mail;

namespace Multiblog.Core.Services.Mail
{
    public interface IMailService
    {
        Task<bool> SendVerifyMail(VerifyItem item);
        Task<bool> SendResetPasswordMail(VerifyItem item);
        Task<bool> SendVerifyEmailChangeMail(VerifyEmailChangeMail item);
    }
}