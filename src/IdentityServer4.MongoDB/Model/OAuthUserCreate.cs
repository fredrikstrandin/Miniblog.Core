using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace IdentityServer4.MongoDB.Model
{
    public class OAuthUserCreate
    {   public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; }
        public List<Claim> Claims { get; set; }
        public List<Provider> Providers { get; set; }        
    }
}
