using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4.MongoDB.Model.Enum
{
    public enum UserStatus
    {
        Valid,
        WoringPassword,
        EmailNotVeryfied,
        NotExist
    }
}
