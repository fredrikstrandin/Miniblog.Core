using IdentityServer4.Models;
using IdentityServer4.MongoDB.Model;
using IdentityServer4.Validation;
using Microsoft.Extensions.Options;
using Multiblog.Core.Model.Setting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Multiblog.OAuth.IdentityServer
{
    public class TenantRedirectUriValidator : IRedirectUriValidator
    {
        private readonly UrlSetting _urlSetting;

        public TenantRedirectUriValidator(IOptions<UrlSetting> urlSetting)
        {
            _urlSetting = urlSetting.Value;
        }

        /// <summary>
        /// Checks if a given URI string is in a collection of strings (using ordinal ignore case comparison)
        /// </summary>
        /// <param name="uris">The uris.</param>
        /// <param name="requestedUri">The requested URI.</param>
        /// <returns></returns>
        private bool StringCollectionContainsString(ICollection<string> uris, string requestedUri)
        {
            if (uris == null)
            {
                return false;
            }

            foreach (var item in uris)
            {
                if (item.Equals(requestedUri, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            if (client is TenantClient && ((TenantClient)client).Tenant == true)
            {
                return CheckValidClientUrl(requestedUri, client);
            }

            return Task.FromResult(StringCollectionContainsString(client.RedirectUris, requestedUri));
        }

        public Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            if (client is TenantClient && ((TenantClient)client).Tenant == true)
            {
                return CheckValidClientUrl(requestedUri, client);
            }

            return Task.FromResult(StringCollectionContainsString(client.RedirectUris, requestedUri));
        }

        private static Task<bool> CheckValidClientUrl(string requestedUri, Client client)
        {
            string[] requestedUriSplit = requestedUri.Split("//");

            if (requestedUriSplit.Length > 1)
            {
                foreach (var item in client.RedirectUris)
                {
                    string[] redirectUrisSplit = item.Split("//");
                    if (requestedUriSplit[0].Equals(redirectUrisSplit[0], StringComparison.CurrentCultureIgnoreCase) &&
                        requestedUriSplit[1].EndsWith(redirectUrisSplit[1], StringComparison.CurrentCultureIgnoreCase))
                    {
                        return Task.FromResult(true);
                    }
                }

                return Task.FromResult(false);
            }
            else
            {
                return Task.FromResult(false);
            }
        }
    }
}
