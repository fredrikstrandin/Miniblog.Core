using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vivius.Model.Mail;
using Vivius.Model.Setting;

namespace Vivius.Repository.Mail
{
    public class MailRepository : IMailRepository
    {
        private readonly APIKeySetting _apiKeySettings;
        private readonly MailSetting _mailSetting;

        public MailRepository(IOptions<APIKeySetting> apiKeySettings,
            IOptions<MailSetting> mailSetting)
        {
            _apiKeySettings = apiKeySettings.Value;
            _mailSetting = mailSetting.Value;
        }

        public async Task<bool> SendVerifyMail(VerifyMail mail)
        {
            var client = new SendGridClient(_apiKeySettings.SendGridAPIKey);
            SendGridMessage msg = new SendGridMessage()
            {
                Subject = mail.Subject,
                From = new EmailAddress(mail.From.Email, mail.From.Name),
                Personalizations = new List<Personalization>()
                {
                    new Personalization()
                    {
                        Tos = new List<EmailAddress>()
                        {
                            new EmailAddress(mail.To.Email, mail.To.Name)
                        },
                        Substitutions = new Dictionary<string, string>()
                        {
                            {"-verifyEmailURL-", mail.VerifyUrl}
                        }
                    }
                },
                //TODO: Borde vara en setting
                TemplateId = "",
                TrackingSettings = new TrackingSettings()
                {
                    ClickTracking = new ClickTracking()
                    {
                        Enable = false,
                        EnableText = false
                    }
                },
                MailSettings = new MailSettings()
                {
                    //TODO: Borde vara en setting
                    SandboxMode = new SandboxMode() { Enable = true }
                }
            };

            var response = await client.SendEmailAsync(msg);

            string json = await response.Body.ReadAsStringAsync();


            return (int)response.StatusCode >= 200 && (int)response.StatusCode <= 299;
        }

        public async Task<bool> SendVerifyEmailChangeMail(VerifyMail mail)
        {
            var client = new SendGridClient(_apiKeySettings.SendGridAPIKey);
            SendGridMessage msg = new SendGridMessage()
            {
                Subject = mail.Subject,
                From = new EmailAddress(mail.From.Email, mail.From.Name),
                Personalizations = new List<Personalization>()
                {
                    new Personalization()
                    {
                        Tos = new List<EmailAddress>()
                        {
                            new EmailAddress(mail.To.Email, mail.To.Name)
                        },
                        Substitutions = new Dictionary<string, string>()
                        {
                            {"-verifyEmailURL-", mail.VerifyUrl}
                        }
                    }
                },
                //TODO: Borde vara en setting
                TemplateId = "",
                TrackingSettings = new TrackingSettings()
                {
                    ClickTracking = new ClickTracking()
                    {
                        Enable = false,
                        EnableText = false
                    }
                },
                MailSettings = new MailSettings()
                {

                    //TODO: Borde vara en setting
                    SandboxMode = new SandboxMode() { Enable = true }
                }
            };

            var response = await client.SendEmailAsync(msg);

            string json = await response.Body.ReadAsStringAsync();


            return (int)response.StatusCode >= 200 && (int)response.StatusCode <= 299;
        }

        public async Task<bool> SendResetMailAsync(ResetMail mail)
        {
            var client = new SendGridClient(_apiKeySettings.SendGridAPIKey);
            SendGridMessage msg = new SendGridMessage()
            {
                Subject = mail.Subject,
                From = new EmailAddress(mail.From.Email, mail.From.Name),
                Personalizations = new List<Personalization>()
                {
                    new Personalization()
                    {
                        Tos = new List<EmailAddress>()
                        {
                            new EmailAddress(mail.To.Email, mail.To.Name)
                        },
                        Substitutions = new Dictionary<string, string>()
                        {
                            {"-resetPasswordURL-", mail.ResetUrl}
                        }
                    }
                },
                //TODO: Borde vara en setting
                TemplateId = "",
                TrackingSettings = new TrackingSettings()
                {
                    ClickTracking = new ClickTracking()
                    {
                        Enable = false,
                        EnableText = false
                    }
                },
                MailSettings = new MailSettings()
                {
                    //TODO: Borde vara en setting
                    SandboxMode = new SandboxMode() { Enable = true }
                }
            };

            var response = await client.SendEmailAsync(msg);

            return (int)response.StatusCode >= 200 && (int)response.StatusCode <= 299;
        }
    }
}
