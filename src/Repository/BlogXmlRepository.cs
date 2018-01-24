using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Miniblog.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Miniblog.Core.Repository
{
    public class BlogXmlRepository : IBlogRepository
    {
        private readonly List<Post> _cache = new List<Post>();
        private readonly string _folder;

        public BlogXmlRepository(IHostingEnvironment env,
            ILogger<BlogXmlRepository> logger)
        {
            // We will mount /app/data in docker/k8s
            var mountedPath = "/app/data";
            if (Directory.Exists(mountedPath))
            {
                logger.LogInformation("Using mounted path '/app/data' for data");
                _folder = Path.Combine(mountedPath, "Posts");
            }
            else
            {
                logger.LogInformation("Using 'wwwroot/Posts' for data");

                _folder = Path.Combine(env.WebRootPath, "Posts");
            }
            
            Initialize();
        }

        public async Task<IEnumerable<Post>> GetPostsAsync(bool isAdmin, int count, int skip)
        {
            return await Task.Run(() =>
            {
                IEnumerable<Post> posts = _cache.Where(p => p.PubDate <= DateTime.UtcNow && (p.IsPublished || isAdmin))
                    .Skip(skip)
                    .Take(count);

                return posts;
            });
        }

        public async Task<IEnumerable<Post>> GetPostsByCategoryAsync(string category, bool isAdmin)
        {
            return await Task.Run(() =>
            {
                IEnumerable<Post> posts = from p in _cache
                                          where p.PubDate <= DateTime.UtcNow && (p.IsPublished || isAdmin)
                                          where p.Categories.Contains(category, StringComparer.OrdinalIgnoreCase)
                                          select p;

                return posts;
            });
        }

        public async Task<Post> GetPostBySlugAsync(string slug, bool isAdmin)
        {
            return await Task.Run(() =>
            {
                Post post = _cache.FirstOrDefault(p => p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
                
                if (post != null && post.PubDate <= DateTime.UtcNow && (post.IsPublished || isAdmin))
                {
                    return Task.FromResult(post);
                }

                return Task.FromResult<Post>(null);
            });            
        }

        public async Task<Post> GetPostByIdAsync(string id, bool isAdmin)
        {
            return await Task.Run(() =>
            {
                var post = _cache.FirstOrDefault(p => p.ID.Equals(id, StringComparison.OrdinalIgnoreCase));
                
                if (post != null && post.PubDate <= DateTime.UtcNow && (post.IsPublished || isAdmin))
                {
                    return Task.FromResult(post);
                }

                return Task.FromResult<Post>(null);
            });
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync(bool isAdmin)
        {

            return await Task.Run(() =>
            {
                var categories = _cache
                    .Where(p => p.IsPublished || isAdmin)
                    .SelectMany(post => post.Categories)
                    .Select(cat => cat.ToLowerInvariant())
                    .Distinct();

                return categories;
            });

        }

        public async Task SavePostAsync(Post post)
        {
            string filePath = GetFilePath(post);
            post.LastModified = DateTime.UtcNow;

            XDocument doc = new XDocument(
                            new XElement("post",
                                new XElement("title", post.Title),
                                new XElement("slug", post.Slug),
                                new XElement("pubDate", post.PubDate.ToString("yyyy-MM-dd HH:mm:ss")),
                                new XElement("lastModified", post.LastModified.ToString("yyyy-MM-dd HH:mm:ss")),
                                new XElement("excerpt", post.Excerpt),
                                new XElement("content", post.Content),
                                new XElement("ispublished", post.IsPublished),
                                new XElement("categories", string.Empty),
                                new XElement("comments", string.Empty)
                            ));

            XElement categories = doc.XPathSelectElement("post/categories");
            foreach (string category in post.Categories)
            {
                categories.Add(new XElement("category", category));
            }

            XElement comments = doc.XPathSelectElement("post/comments");
            foreach (Comment comment in post.Comments)
            {
                comments.Add(
                    new XElement("comment",
                        new XElement("author", comment.Author),
                        new XElement("email", comment.Email),
                        new XElement("date", comment.PubDate.ToString("yyyy-MM-dd HH:m:ss")),
                        new XElement("content", comment.Content),
                        new XAttribute("isAdmin", comment.IsAdmin),
                        new XAttribute("id", comment.ID)
                    ));
            }

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
            {
                await doc.SaveAsync(fs, SaveOptions.None, CancellationToken.None).ConfigureAwait(false);
            }

            if (!_cache.Contains(post))
            {
                _cache.Add(post);
                SortCache();
            }
        }

        public Task DeletePostAsync(Post post)
        {
            string filePath = GetFilePath(post);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            if (_cache.Contains(post))
            {
                _cache.Remove(post);
            }

            return Task.CompletedTask;
        }

        #region Private Method
        private void Initialize()
        {
            LoadPosts();
            SortCache();
        }

        private void LoadPosts()
        {
            if (!Directory.Exists(_folder))
                Directory.CreateDirectory(_folder);

            // Can this be done in parallel to speed it up?
            foreach (string file in Directory.EnumerateFiles(_folder, "*.xml", SearchOption.TopDirectoryOnly))
            {
                XElement doc = XElement.Load(file);

                Post post = new Post
                {
                    ID = Path.GetFileNameWithoutExtension(file),
                    Title = ReadValue(doc, "title"),
                    Excerpt = ReadValue(doc, "excerpt"),
                    Content = ReadValue(doc, "content"),
                    Slug = ReadValue(doc, "slug").ToLowerInvariant(),
                    PubDate = DateTime.Parse(ReadValue(doc, "pubDate")),
                    LastModified = DateTime.Parse(ReadValue(doc, "lastModified", DateTime.Now.ToString(CultureInfo.InvariantCulture)))                    
                };

                LoadCategories(post, doc);
                LoadComments(post, doc);
                _cache.Add(post);
            }
        }

        protected void SortCache()
        {
            _cache.Sort((p1, p2) => p2.PubDate.CompareTo(p1.PubDate));
        }

        private string GetFilePath(Post post)
        {
            return Path.Combine(_folder, post.ID + ".xml");
        }


        private static void LoadCategories(Post post, XElement doc)
        {
            XElement categories = doc.Element("categories");
            if (categories == null)
                return;

            List<string> list = new List<string>();

            foreach (var node in categories.Elements("category"))
            {
                list.Add(node.Value);
            }

            post.Categories = list.ToArray();
        }

        private static void LoadComments(Post post, XElement doc)
        {
            var comments = doc.Element("comments");

            if (comments == null)
                return;

            foreach (var node in comments.Elements("comment"))
            {
                Comment comment = new Comment()
                {
                    ID = ReadAttribute(node, "id"),
                    Author = ReadValue(node, "author"),
                    Email = ReadValue(node, "email"),
                    IsAdmin = bool.Parse(ReadAttribute(node, "isAdmin", "false")),
                    Content = ReadValue(node, "content"),
                    PubDate = DateTime.Parse(ReadValue(node, "date", "2000-01-01")),
                };

                post.Comments.Add(comment);
            }
        }

        private static string ReadValue(XElement doc, XName name, string defaultValue = "")
        {
            if (doc.Element(name) != null)
                return doc.Element(name)?.Value;

            return defaultValue;
        }

        private static string ReadAttribute(XElement element, XName name, string defaultValue = "")
        {
            if (element.Attribute(name) != null)
                return element.Attribute(name)?.Value;

            return defaultValue;
        }

        #endregion Private Method
    }
}
