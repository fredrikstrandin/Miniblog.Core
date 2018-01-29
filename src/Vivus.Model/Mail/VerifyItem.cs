using System;
using System.Collections.Generic;
using System.Text;

namespace Vivus.Model.Mail
{
    public class VerifyItem
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
    }
}
