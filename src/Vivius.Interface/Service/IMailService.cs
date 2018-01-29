using System.Threading.Tasks;
using Vivus.Model.Mail;

namespace Vivius.Services.Mail
{
    public interface IMailService
    {
        Task<bool> SendVerifyMail(VerifyItem item);
        Task<bool> SendResetPasswordMail(VerifyItem item);
        Task<bool> SendVerifyEmailChangeMail(VerifyEmailChangeMail item);
    }
}