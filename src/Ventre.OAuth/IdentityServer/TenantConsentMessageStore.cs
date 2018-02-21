using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vivus.OAuth.IdentityServer
{
    public class TenantConsentMessageStore : IConsentMessageStore
    {
        private Dictionary<string, Message<ConsentResponse>> _dictionary = new Dictionary<string, Message<ConsentResponse>>();
        public Task DeleteAsync(string id)
        {
            if(_dictionary.ContainsKey(id))
            {
                _dictionary.Remove(id);
            }

            return Task.CompletedTask;
        }

        public Task<Message<ConsentResponse>> ReadAsync(string id)
        {
            if (_dictionary.ContainsKey(id))
            {
                return Task.FromResult<Message<ConsentResponse>>(_dictionary[id]); 
            }

            return Task.FromResult<Message<ConsentResponse>>(null);
        }

        public Task WriteAsync(string id, Message<ConsentResponse> message)
        {
            if (_dictionary.ContainsKey(id))
            {
                _dictionary[id] = message;
            }
            else
            {
                _dictionary.Add(id, message);
            }

            return Task.CompletedTask;
        }
    }
}
