using MongoDB.Bson.Serialization;
using System;
using System.Security.Claims;

namespace IdentityServer4.MongoDB.Serializer
{
    public class ClaimProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            if (type == typeof(Claim))
            {
                return new ClaimSerializer();
            }

            return null;
        }
    }
}
