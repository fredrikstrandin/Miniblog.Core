using Miniblog.Core.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Miniblog.Core.Repository.MongoDB.Models
{
    public class CommentSubEntity
    {
        public ObjectId CommentId { get; set; }

        public string Author { get; set; }

        public string Email { get; set; }

        public string Content { get; set; }

        public DateTime PubDate { get; set; }

        public bool IsAdmin { get; set; }

        public static implicit operator CommentSubEntity(Comment item)
        {
            ObjectId id;

            if (item == null)
                return null;

            if (!ObjectId.TryParse(item.Id, out id))
            {
                id = ObjectId.GenerateNewId();
            }

            CommentSubEntity comment = new CommentSubEntity()
            {
                CommentId = id,
                Author = item.Author,
                Content = item.Content,
                Email = item.Email,
                IsAdmin = item.IsAdmin,
                PubDate = item.PubDate
            };

            return comment;
        }

        public static implicit operator Comment(CommentSubEntity item)
        {
            if (item == null)
                return null;
            
            Comment comment = new Comment()
            {
                Id = item.CommentId.ToString(),
                Author = item.Author,
                Content = item.Content,
                Email = item.Email,
                IsAdmin = item.IsAdmin,
                PubDate = item.PubDate
            };

            return comment;
        }
    }
}
