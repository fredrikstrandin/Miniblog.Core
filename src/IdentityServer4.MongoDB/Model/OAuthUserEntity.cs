using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Security.Claims;
using IdentityServer4.MongoDB.Model;

namespace IdentityServer4.MongoDB.Model
{
    [BsonIgnoreExtraElements]
    public class OAuthUserEntity 
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonRequired]
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        [BsonIgnoreIfDefault]
        public string Password { get; set; }
        [BsonIgnoreIfDefault]
        public byte[] Salt { get; set; }
        [BsonIgnoreIfNull]
        public List<Provider> Providers { get; set; }
        [BsonIgnoreIfNull]
        public List<string> Roles { get; set; }
        [BsonIgnoreIfNull]
        public List<Claim> Claims { get; set; }
        public bool IsActive { get; set; }        
    }
}