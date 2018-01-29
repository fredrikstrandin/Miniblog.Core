using System;
using System.Collections.Generic;
using System.Text;
using Vivius.Model.Utils;

namespace Vivius.Model.User
{
    public class UserItem
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        private string _email { get; set; }
        public string Email { get { return _email; } set { _email = value?.ToLower().Trim() ?? null; } }
        public bool EmailVerified { get; set; }
        public string City { get; set; }
        public string ProfileImageUrl { get; set; }
        public List<string> Favorite { get; set; } = new List<string>();
        public DateTime DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public GPSPosition GPSPosition { get; set; }
        public List<ProviderItem> Providers { get; set; }
        public string Headline { get; set; }
        public string Summary { get; set; }
        public string Country { get; set; }
        public string Password { get; set; }
    }
}
