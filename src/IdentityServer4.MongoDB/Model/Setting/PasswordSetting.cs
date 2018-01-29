using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4.MongoDB.Model.Setting
{
    public class PasswordSetting
    {
        public int PasswordMinLength { get; set; }
        public int PasswordMaxLength { get; set; }
        public int RetryInSecond { get; set; }
        public int MaxRetry { get; set; }
        public int HaschCount { get; set; }
    }
}
