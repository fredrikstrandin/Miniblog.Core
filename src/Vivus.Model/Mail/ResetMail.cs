using System;
using System.Collections.Generic;
using System.Text;

namespace Vivius.Model.Mail
{
    public class ResetMail
    {
        public EmailAdress From { get; set; }
        public EmailAdress To { get; set; }
        public string Subject { get; set; }
        public string ResetUrl { get; set; }
    }
}
