using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace IdentityServer4.MongoDB.Model
{
    public class TempOAuthUserAccessTokenItem
    {
        public string AccessToken { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
