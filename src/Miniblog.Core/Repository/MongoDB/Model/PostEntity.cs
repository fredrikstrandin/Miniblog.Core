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

        public string Title { get; set; }

        public string Slug { get; set; }
        public string SlugNormalize { get { return Slug.Normalize(); } set { } }
        public string Excerpt { get; set; }

        public string Content { get; set; }

        public DateTime PubDate { get; set; } = DateTime.UtcNow;

        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public bool IsPublished { get; set; } = true;

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
                ID = item.Id.ToString(),
                Title = item.Title,
                Categories = item.Categories,
                Content = item.Content,
                Excerpt = item.Excerpt,
                IsPublished = item.IsPublished,
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

            ObjectId.TryParse(item.ID, out id);
            
            PostEntity post = new PostEntity()
            {
                Id = id,
                Title = item.Title,
                Categories = item.Categories,
                Content = item.Content,
                Excerpt = item.Excerpt,
                IsPublished = item.IsPublished,
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
