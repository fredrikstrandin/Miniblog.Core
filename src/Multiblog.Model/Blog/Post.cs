using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Multiblog.Model;
using Multiblog.Utilities;

namespace Multiblog.Core.Models
{
    public class Post
    {
        public string Id { get; set; }
        public string BlogId { get; set; }
        public Author Author { get; set; }
        [Required]
        public string Title { get; set; }

        private string _slug { get; set; }
        public string Slug
        {
            get
            {
                return _slug;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _slug = Title.GenerateSlug(); 
                }
                else
                {
                    _slug = value.GenerateSlug();
                }
            }
        }

        [Required]
        public string Excerpt { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime PubDate { get; set; } = DateTime.UtcNow;

        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public Status Status { get; set; }

        public IList<string> Categories { get; set; } = new List<string>();

        public IList<Comment> Comments { get; protected set; } = new List<Comment>();
    }

    public static class PostExt
    {        
        public static string GetLink(this Post post)
        {
            return $"/blog/{post.Slug}/";
        }

        public static bool AreCommentsOpen(this Post post, int commentsCloseAfterDays)
        {
            return post.PubDate.AddDays(commentsCloseAfterDays) >= DateTime.UtcNow;
        }

        public static string RenderContent(this Post post)
        {
            var result = post.Content;

            if(string.IsNullOrEmpty(result))
            {
                return result;
            }

            // Set up lazy loading of images/iframes
            result = result.Replace(" src=\"", " src=\"data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==\" data-src=\"");

            // Youtube content embedded using this syntax: [youtube:xyzAbc123]
            var video = "<div class=\"video\"><iframe width=\"560\" height=\"315\" title=\"YouTube embed\" src=\"about:blank\" data-src=\"https://www.youtube-nocookie.com/embed/{0}?modestbranding=1&amp;hd=1&amp;rel=0&amp;theme=light\" allowfullscreen></iframe></div>";
            result = Regex.Replace(result, @"\[youtube:(.*?)\]", m => string.Format(video, m.Groups[1].Value));

            return result;
        }
    }
}
