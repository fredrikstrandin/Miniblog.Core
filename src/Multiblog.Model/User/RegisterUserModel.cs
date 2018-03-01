using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Multiblog.Model.User
{
    public class RegisterUserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; }
        public string CountryCodes { get; set; }
        public string ReturnUrl { get; set; }
        public string ProviderName { get; set; }
        public string ProviderSubjectId { get; set; }
        public List<string> Roles { get; set; }
        public List<Claim> Claims { get; set; }
        public bool EmailVerified { get; set; }
        public string Id { get; set; }        
    }
}
