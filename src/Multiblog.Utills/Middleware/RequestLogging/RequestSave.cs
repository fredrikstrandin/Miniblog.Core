using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Multiblog.Utilities.Middleware.RequestLogging
{
    public class RequestSave : IRequestSave
    {
        private RequestLoggingMongoDb _requestLogging;
        private readonly ILogger _logger;

        protected IMongoClient Client;
        protected IMongoDatabase Database;
        protected IMongoCollection<RequestInfoMongoDb> Collection;

        public RequestSave(IOptions<RequestLoggingMongoDb> requestLogging, ILoggerFactory loggerFactory)
        {
            _requestLogging = requestLogging.Value;
            _logger = loggerFactory.CreateLogger<RequestSave>(); 

            Client = new MongoClient(_requestLogging.ConnectionString);
            Database = Client.GetDatabase(_requestLogging.Database);
            Collection = Database.GetCollection<RequestInfoMongoDb>(_requestLogging.Collection);            
        }

        public async Task IncomingAsync(HttpContext context, RequestInfo info)
        {
            RequestInfoMongoDb i = new RequestInfoMongoDb(info);

            await Collection.InsertOneAsync(i);
            
            context.Items.Add("requestId", i.Id);
        }

        public async Task OutgoingAsync(HttpContext context, int statusCode, long millisecond)
        {
            if (context.Items.ContainsKey("requestId"))
            {
                ObjectId id = (ObjectId)context.Items["requestId"];
                var filter = Builders<RequestInfoMongoDb>.Filter.Eq(x => x.Id, id);
                var update = Builders<RequestInfoMongoDb>.Update
                    .Set("StatusCode", statusCode)
                    .Set("Millisecond", millisecond);

                var result = await Collection.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = false });
            }
        }

        public async Task ExceptionAsync(ExceptionContext context)
        {
            if (context.HttpContext.Items.ContainsKey("requestId"))
            {
                ObjectId id = (ObjectId)context.HttpContext.Items["requestId"];
                var filter = Builders<RequestInfoMongoDb>.Filter.Eq(x => x.Id, id);
                var update = Builders<RequestInfoMongoDb>.Update
                    .Set("Exception", context.Exception);

                var result = await Collection.UpdateOneAsync(filter, update);
            }
        }
    }
}