using Multiblog.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Multiblog.Core.Model
{
    public class PostView : Post
    {
        public PostView(Post post)
        {
            if(post == null)
            {
                return;
            }

            Author = post.Author;
            BlogId = post.BlogId;
            Categories = post.Categories;
            Comments = post.Comments;
            Content = post.Content;
            Excerpt = post.Excerpt;
            Id = post.Id;
            LastModified = post.LastModified;
            PubDate = post.PubDate;
            Slug = post.Slug;
            Status = post.Status;
            Title = post.Title;            
        }

        public string Url { get; set; }
    }
}
