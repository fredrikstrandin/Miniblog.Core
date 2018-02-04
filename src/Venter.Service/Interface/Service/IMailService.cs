using System.Threading.Tasks;
using Vivus.Model.Mail;

namespace Miniblog.Core.Services.Mail
{
    public interface IMailService
    {
        Task<bool> SendVerifyMail(VerifyItem item);
        Task<bool> SendResetPasswordMail(VerifyItem item);
        Task<bool> SendVerifyEmailChangeMail(VerifyEmailChangeMail item);
    }
}