using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vivius.Repository.Model
{
    [BsonIgnoreExtraElements]
    internal class BaseEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }

        /// <summary>
        /// This attribute is for Etag
        /// </summary>
        public long? Version { get; set; } = DateTime.UtcNow.Ticks;
        [BsonIgnoreIfDefault]
        public bool IsHistory { get; set; }
        [BsonIgnoreIfDefault]
        public ObjectId CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
