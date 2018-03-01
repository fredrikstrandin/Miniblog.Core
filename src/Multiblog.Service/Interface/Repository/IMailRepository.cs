using System.Threading.Tasks;
using Multiblog.Core.Model.Mail;

namespace Multiblog.Core.Repository.Mail
{
    public interface IMailRepository
    {
        Task<bool> SendVerifyMail(VerifyMail mail);
        Task<bool> SendVerifyEmailChangeMail(VerifyMail mail);
        Task<bool> SendResetMailAsync(ResetMail mail);
    }
}