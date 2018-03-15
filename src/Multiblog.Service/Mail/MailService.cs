using System.Linq;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Multiblog.Core.Model.Setting;
using Multiblog.Core.Repository.Mail;
using Multiblog.Core.Repository.VerifyUser;
using Multiblog.Core.Model.Mail;
using Multiblog.Model.Mail;

namespace Multiblog.Core.Services.Mail
{
    public class MailService : IMailService
    {
        private readonly IMailRepository _mailRepository;
        private readonly IVerifyUserRepository _verifyUserRepository;
        private readonly UrlSettings _urlSetting;
        private readonly MailSetting _mailSetting;


        public MailService(IMailRepository mailRepository,
            IVerifyUserRepository verifyUserRepository,
            IOptions<UrlSettings> urlSetting,
            IOptions<MailSetting> mailSetting)
        {
            _mailRepository = mailRepository;
            _verifyUserRepository = verifyUserRepository;
            _urlSetting = urlSetting.Value;
            _mailSetting = mailSetting.Value;
        }

        public async Task<bool> SendVerifyMail(VerifyItem item)
        {
            string code = await _verifyUserRepository.CreateCodeAsync(item.UserId);

            return await _mailRepository.SendVerifyMail(new VerifyMail()
            {
                From = new EmailAdress() { Email = _mailSetting.InfoMailAdress, Name = _mailSetting.InfoMailName },
                To = new EmailAdress() { Email = item.Email, Name = item.FullName },
                Subject = $"{item.FirstName} Välkommen till Poolia, bekräfta din epost.",
                VerifyUrl = $"{_urlSetting.APIServerUrl}/api/oauth/verifyaccount/{code}"
            });
        }

        public async Task<bool> SendResetPasswordMail(VerifyItem item)
        {
            string code = await _verifyUserRepository.CreateCodeAsync(item.UserId);

            return await _mailRepository.SendResetMailAsync(new ResetMail()
            {
                From = new EmailAdress() { Email = _mailSetting.InfoMailAdress, Name = _mailSetting.InfoMailName },
                To = new EmailAdress() { Email = item.Email, Name = item.FullName },
                Subject = $"{item.FirstName} här kan du återställa ditt lösenord.",
                ResetUrl = $"{_urlSetting.ClientUrl}/account/change-password?hash={code}"
            });
        }
                
        public async Task<bool> SendVerifyEmailChangeMail(VerifyEmailChangeMail item)
        {
            return await _mailRepository.SendVerifyMail(new VerifyMail()
            {
                From = new EmailAdress() { Email = _mailSetting.InfoMailAdress, Name = _mailSetting.InfoMailName },
                To = new EmailAdress() { Email = item.Email, Name = item.FullName },
                Subject = $"{item.FirstName}, din begäran om att byta epost adress.",
                VerifyUrl = $"{_urlSetting.APIServerUrl}/api/oauth/verifyemailchange/{item.Code}"
            });

        }
    }
}
