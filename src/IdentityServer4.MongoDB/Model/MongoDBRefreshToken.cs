using IdentityServer4.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace IdentityServer4.MongoDB.Model
{
    public class MongoDBRefreshToken
    {
        [BsonId]
        public string key { get; set; }

        public RefreshToken value { get; set; }
    }
}
