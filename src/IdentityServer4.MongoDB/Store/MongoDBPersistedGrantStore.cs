using IdentityServer4.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using IdentityServer4.MongoDB.MonogDBContext;
using MongoDB.Driver;
using IdentityServer4.MongoDB.Model;

namespace IdentityServer4.MongoDB.Store
{
    /// <summary>
    /// In-memory persisted grant store
    /// </summary>
    public class MongoDBPersistedGrantStore : IPersistedGrantStore
    {
        private readonly ILogger<MongoDBPersistedGrantStore> _logger;
        private readonly IMongoIdentityContext _context;

        public MongoDBPersistedGrantStore(ILogger<MongoDBPersistedGrantStore> logger,
            IMongoIdentityContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            var filter = Builders<MongoDBPersistedGrant>.Filter.Eq(x => x.key, grant.Key);
            var update = Builders<MongoDBPersistedGrant>.Update
                .Set(q => q.PersistedGrant, grant);
            
            var result = await _context.MongoDBPersistedGrantCollection.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });

            return;
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            var ret = await _context.MongoDBPersistedGrantCollection.Find(x => x.key == key)
                .Project(x => x.PersistedGrant)
                .FirstOrDefaultAsync();

            return ret;
            
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var ret = await _context.MongoDBPersistedGrantCollection.Find(x => x.PersistedGrant.SubjectId == subjectId)
                .Project(x => x.PersistedGrant)
                .ToListAsync();

            return ret;            
        }

        public async Task RemoveAsync(string key)
        {
            var ret = await _context.MongoDBPersistedGrantCollection.DeleteOneAsync(x => x.key == key);

            return;
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            var ret = await _context.MongoDBPersistedGrantCollection.DeleteManyAsync(x => x.PersistedGrant.ClientId == clientId && x.PersistedGrant.SubjectId == subjectId);

            return;
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var ret = await _context.MongoDBPersistedGrantCollection
                .DeleteManyAsync(x => x.PersistedGrant.ClientId == clientId && x.PersistedGrant.SubjectId == subjectId && x.PersistedGrant.Type == type);

            return;
        }
    }
}
