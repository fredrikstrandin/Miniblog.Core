using System;
using System.Collections.Generic;
using System.Text;

namespace Multiblog.Core.Model.Mail
{
    public class VerifyMail
    {
        public EmailAdress From { get; set; }
        public EmailAdress To { get; set; }
        public string Subject { get; set; }
        public string VerifyUrl { get; set; }
    }
}
