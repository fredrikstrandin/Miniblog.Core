using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using Venter.Utilities;
using WilderMinds.MetaWeblog;

namespace Miniblog.Core.Repository.MongoDB.Model
{
    public class BlogEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public String Title { get; set; }
        public String SubTitle { get; set; }
        public string SubDomain { get; set; }
        public string SubDomainNormalize { get { return SubDomain.GenerateSlug(); } private set { } }
        public int PostsPerPage { get; set; }
        public int CommentsCloseAfterDays { get; set; }
        public List<CategoryEntity> Categorys { get; set; }
    }
}
