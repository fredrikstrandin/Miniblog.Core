using IdentityServer4.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace IdentityServer4.MongoDB.Model
{
    [BsonIgnoreExtraElements]
    public class MongoDBPersistedGrant
    {
        [BsonId]
        public string key { get; set; }
        public PersistedGrant PersistedGrant { get; set; }
    }
}
