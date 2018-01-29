using IdentityServer4.MongoDB.Model;
using IdentityServer4.MongoDB.Model.Setting;
using IdentityServer4.MongoDB.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace IdentityServer4.MongoDB.MonogDBContext
{
    public class MongoIdentityContext : IMongoIdentityContext
    {
        private MongoDatabaseSetting _requestLogging;
        private readonly ILogger _logger;

        protected IMongoClient _client;
        protected IMongoDatabase _database;

        public MongoIdentityContext(IOptions<MongoDatabaseSetting> requestLogging, 
            ILoggerFactory loggerFactory)
        {
            _requestLogging = requestLogging.Value;
            _logger = loggerFactory.CreateLogger<MongoIdentityContext>();

            _client = new MongoClient(_requestLogging.ConnectionString);
            _database = _client.GetDatabase(_requestLogging.Database);
        }


        public IMongoCollection<OAuthUserEntity> OAuthUserEntityCollection { get { return _database.GetCollection<OAuthUserEntity>("OAuthUserEntity"); } }
        public IMongoCollection<MongoDBPersistedGrant> MongoDBPersistedGrantCollection { get { return _database.GetCollection<MongoDBPersistedGrant>("PersistedGrant"); } }
        public IMongoCollection<TempUserClaimsEntity> TempUserClaimsEntityCollection { get { return _database.GetCollection<TempUserClaimsEntity>("TempUserClaimsEntity"); } }
    }
}
