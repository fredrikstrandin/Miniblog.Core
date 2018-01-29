using System.Threading.Tasks;
using Vivius.Model.Mail;

namespace Vivius.Repository.Mail
{
    public interface IMailRepository
    {
        Task<bool> SendVerifyMail(VerifyMail mail);
        Task<bool> SendVerifyEmailChangeMail(VerifyMail mail);
        Task<bool> SendResetMailAsync(ResetMail mail);
    }
}