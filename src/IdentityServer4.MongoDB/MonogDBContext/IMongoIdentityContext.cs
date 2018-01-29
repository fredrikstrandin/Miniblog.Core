using IdentityServer4.MongoDB.Model;
using MongoDB.Driver;

namespace IdentityServer4.MongoDB.MonogDBContext
{
    public interface IMongoIdentityContext
    {
        IMongoCollection<MongoDBPersistedGrant> MongoDBPersistedGrantCollection { get; }
        IMongoCollection<OAuthUserEntity> OAuthUserEntityCollection { get; }
        IMongoCollection<TempUserClaimsEntity> TempUserClaimsEntityCollection { get; }
    }
}