using Miniblog.Core.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Miniblog.Core.Repository.MongoDB.Models
{
    public class PostEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId  BlogId { get; set; }
        public string Title { get; set; }

        public string Slug { get; set; }
        public string SlugNormalize
        {
            get
            {
                if(Slug == null && Title == null)
                {
                    return string.Empty;
                }

                if (Slug == null)
                {
                    Slug = Title;                    
                }

                return Slug.Normalize();                
            }
            set { }
        }
        public string Excerpt { get; set; }

        public string Content { get; set; }

        public DateTime PubDate { get; set; } = DateTime.UtcNow;

        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public Status Status { get; set; }

        [BsonIgnoreIfNull]
        public IList<string> Categories { get; set; } = new List<string>();

        [BsonIgnoreIfNull]
        public IList<CommentSubEntity> Comments { get; set; }

        public static implicit operator Post(PostEntity item)
        {
            if (item == null)
                return null;

            Post post = new Post()
            {
                Id = item.Id.ToString(),
                Title = item.Title,
                Categories = item.Categories,
                Content = item.Content,
                Excerpt = item.Excerpt,
                Status = item.Status,
                LastModified = item.LastModified,
                PubDate = item.PubDate,
                Slug = item.Slug
            };
            
            foreach (var comment in item.Comments ?? new List<CommentSubEntity>())
            {
                post.Comments.Add(comment);
            }
                
            return post;
        }

        public static implicit operator PostEntity(Post item)
        {
            ObjectId id;

            if (item == null)
                return null;

            ObjectId.TryParse(item.Id, out id);
            ObjectId.TryParse(item.BlogId, out ObjectId blogId);
            
            PostEntity post = new PostEntity()
            {
                Id = id,
                BlogId = blogId,
                Title = item.Title,
                Categories = item.Categories,
                Content = item.Content,
                Excerpt = item.Excerpt,
                Status = item.Status,
                LastModified = item.LastModified,
                PubDate = item.PubDate,
                Slug = item.Slug
            };
            
            foreach (var comment in item.Comments ?? new List<Comment>())
            {
                if(post.Comments == null)
                {
                    post.Comments = new List<CommentSubEntity>();
                }

                post.Comments.Add(comment);
            }

            return post;
        }
    }
}
