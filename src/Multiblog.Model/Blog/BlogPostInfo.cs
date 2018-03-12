using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;

namespace Multiblog.Model.Blog
{
    public class BlogPostInfo
    {
        public string Slug { get; set; }
        public DateTime LastModified { get; set; }
        public ObjectId PostId { get; set; }
    }
}
