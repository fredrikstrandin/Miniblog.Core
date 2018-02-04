using System.Threading.Tasks;
using Miniblog.Core.Model.Mail;

namespace Miniblog.Core.Repository.Mail
{
    public interface IMailRepository
    {
        Task<bool> SendVerifyMail(VerifyMail mail);
        Task<bool> SendVerifyEmailChangeMail(VerifyMail mail);
        Task<bool> SendResetMailAsync(ResetMail mail);
    }
}