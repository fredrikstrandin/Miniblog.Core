using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Miniblog.Core.Repository.MongoDB.Models
{
    internal class UserEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string NickName { get; set; }
        public string About { get; set; }
        public List<ObjectId> BlogOwnerList { get; set; }
    }
}