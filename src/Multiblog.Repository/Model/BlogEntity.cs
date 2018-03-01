using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using Multiblog.Utilities;
using Multiblog.Model;

namespace Multiblog.Core.Repository.MongoDB.Model
{
    public class BlogEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public string SubDomain { get; set; }
        public string SubDomainNormalize { get { return SubDomain.GenerateSlug(); } private set { } }
        public int PostsPerPage { get; set; }
        public int CommentsCloseAfterDays { get; set; }
        public List<CategoryEntity> Categorys { get; set; }

        public static implicit operator BlogItem(BlogEntity item)
        {
            if (item == null)
                return null;

            var blog = new BlogItem()
            {
                Id = item?.Id.ToString() ?? string.Empty,
                Title = item.Title,
                Description = item.Description,
                PostsPerPage = item.PostsPerPage,
                SubDomain = item.SubDomain
            };

            return blog;
        }

        public static implicit operator BlogEntity(BlogItem item)
        {
            if (item == null)
                return null;

            ObjectId.TryParse(item.Id, out ObjectId id);
            
            var blog = new BlogEntity()
            {
                Id = id,
                Title = item.Title,
                Description = item.Description,
                PostsPerPage = item.PostsPerPage,
                SubDomain = item.SubDomain
            };

            return blog;
        }
    }
}
