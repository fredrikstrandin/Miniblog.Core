using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Multiblog.Core.Repository.MongoDB.Model
{
    public class CategoryEntity
    {
        [BsonId]
        public string Name { get; set; }
        public int Count { get; set; }
    }
}