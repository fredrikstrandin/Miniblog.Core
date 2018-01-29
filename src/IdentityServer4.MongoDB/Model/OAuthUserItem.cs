using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer4.MongoDB.Model
{
    public class OAuthUserItem
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public byte[] Salt { get; set; }
        public List<string> Roles { get; set; }
        public List<Claim> Claims { get; set; }
        public List<Provider> Providers { get; set; }
        public bool IsActive { get; set; }
        public bool EmailVerified { get; set; }
    }
}