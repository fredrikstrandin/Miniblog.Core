using IdentityServer4.Models;
using IdentityServer4.MongoDB.Model;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Vivus.OAuth.IdentityServer
{
    public class TenantRedirectUriValidator : IRedirectUriValidator
    {
        /// <summary>
        /// Checks if a given URI string is in a collection of strings (using ordinal ignore case comparison)
        /// </summary>
        /// <param name="uris">The uris.</param>
        /// <param name="requestedUri">The requested URI.</param>
        /// <returns></returns>
        private bool StringCollectionContainsString(ICollection<string> uris, string requestedUri)
        {
            if (uris == null) return false;

            return true; // uris.Contains(requestedUri, StringComparer.OrdinalIgnoreCase);
        }

        public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            if (client is TenantClient && ((TenantClient)client).Tenant == true)
            {
                return Task.FromResult(true);
            }


            return Task.FromResult(StringCollectionContainsString(client.RedirectUris, requestedUri));
        }

        public Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            if (client is TenantClient && ((TenantClient)client).Tenant == true)
            {
                string[] first = requestedUri.Split("//");

                if(first.Length > 1)
                {
                    string[] second = first[1].Split("/");
                    if(second.Length > 1)
                    {
                        string[] tredje = second[0].Split(".");
                        string url = string.Empty;

                        for (int i = 1; i < tredje.Length; i++)
                        {
                            url += tredje[i];
                        }
                    }
                }
                return Task.FromResult(true);
            }


            return Task.FromResult(StringCollectionContainsString(client.RedirectUris, requestedUri));
        }
    }
}
