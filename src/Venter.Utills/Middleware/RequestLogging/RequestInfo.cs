using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Net;
using System;

namespace Venter.Utilities.Middleware.RequestLogging
{
    public class RequestInfo
    {
        public IPAddress RemoteIpAddress { get; set; }
        public int RemotePort { get; set; }

        public string Method { get; set; }
        public string Path { get; set; }
        public int StatusCode { get; set; }
        public string QueryString { get; set; }

        public long Millisecond { get; set; }
        [BsonIgnoreIfNull]
        public System.Exception Exception { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class RequestInfoMongoDb : RequestInfo
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public RequestInfoMongoDb(RequestInfo ri)
        {
            Path = ri.Path;
            StatusCode = ri.StatusCode;
            RemotePort = ri.RemotePort;
            RemoteIpAddress = ri.RemoteIpAddress;
            Method = ri.Method;
            QueryString = ri.QueryString;
            Exception = ri.Exception;
        }
    }
}
