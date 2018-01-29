using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer4.MongoDB.Model
{
    public  class TempUserClaimsEntity
    {
        [BsonId]
        public string ProviderSubjectKey { get; set; }
        public List<Claim> Claims { get; set; }
        public string AccessToken { get; set; }
        [BsonIgnoreIfNull]
        public DateTime ExpiresAt { get; set; }
    }
}