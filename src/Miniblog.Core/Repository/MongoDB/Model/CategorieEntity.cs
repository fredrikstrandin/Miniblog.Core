using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Core.Repository.MongoDB.Model
{
    public class CategorieEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }
}
