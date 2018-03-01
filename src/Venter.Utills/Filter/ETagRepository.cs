using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Multiblog.Utilities.Filter
{
    public static class ETagRepository
    {
        private static readonly IMongoClient Client;
        private static readonly IMongoDatabase Database;

        static ETagRepository()
        {
            Client = new MongoClient("mongodb://localhost:27017");
            Database = Client.GetDatabase("ProjectQ_Development");
        }
        
        public static async Task<long> GetETagAsync(string collection, string key, string id)
        {
            var coll = Database.GetCollection<BsonDocument>(collection);
                        
            var projection = Builders<long>.Projection.Include(key).Exclude("_id");

            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));
            var versionProjection = Builders<BsonDocument>.Projection
                                        .Include(key)
                                        .Exclude("_id");

            var query = coll.Find(Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id)))
                .Project<BsonDocument>(versionProjection);

            var o = await query.FirstOrDefaultAsync();

            if (o != null)
            {
                return o[key].AsInt64;
            }
            else
            {
                return 0;
            }
            
        }

        public static void SetETagAsync(string collection, string key, string id, long value)
        {

        }
    }
}
