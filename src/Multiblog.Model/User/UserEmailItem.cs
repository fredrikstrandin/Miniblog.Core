using System;
using System.Collections.Generic;
using System.Text;

namespace Multiblog.Core.Model.User
{
    public class UserEmailItem
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
    }
}
