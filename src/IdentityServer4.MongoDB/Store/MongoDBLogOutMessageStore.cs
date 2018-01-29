using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.MongoDB.Store
{
    public class MongoMessageDBStore<TModel> : IMessageStore<TModel>
    {
        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Message<TModel>> ReadAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task WriteAsync(string id, Message<TModel> message)
        {
            
            throw new NotImplementedException();
        }

        public Task<string> WriteAsync(Message<TModel> message)
        {
            throw new NotImplementedException();
        }
    }
}
