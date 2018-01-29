using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4.MongoDB.Model
{
    public class Provider
    {
        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the provider subject identifier.
        /// </summary>
        public string ProviderSubjectId { get; set; }
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiresAt { get; set; }
    }

}
