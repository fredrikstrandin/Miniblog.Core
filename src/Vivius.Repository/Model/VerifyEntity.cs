using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miniblog.Core.Repository.Model
{
    [BsonIgnoreExtraElements]
    public class VerifyEntity
    {
        [BsonId]
        public ObjectId VerifyCode { get; set; }
        public ObjectId UserId { get; set; }
        public DateTime Expired { get; set; }

    }
}
