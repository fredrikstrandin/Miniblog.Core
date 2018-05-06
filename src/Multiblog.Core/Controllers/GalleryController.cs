using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Multiblog.Core.Attribute;
using Multiblog.Core.Model;
using Multiblog.Core.Model.Setting;
using Multiblog.Core.Models;
using Multiblog.Model;
using Multiblog.Service.Interface;
using Multiblog.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using WebEssentials.AspNetCore.Pwa;

namespace Multiblog.Core.Controllers
{
    [ServiceFilter(typeof(TenantAttribute))]
    [Route("[controller]")]
    public class GalleryController : Controller
    {
        private readonly IBlogPostService _blogPostService;
        private readonly IBlogService _blogService;
        private readonly IOptionsSnapshot<BlogSettings> _settings;
        private readonly WebManifest _manifest;

        public GalleryController(IBlogPostService blogPostService,
            IBlogService blogService,
            IOptionsSnapshot<BlogSettings> settings,
            WebManifest manifest)
        {
            _blogPostService = blogPostService;
            _blogService = blogService;
            _settings = settings;
            _manifest = manifest;
        }

        [Route("{page:int?}")]
        //[OutputCache(Profile = "default")]
        public async Task<IActionResult> Index([FromRoute]int page = 0)
        {
            int PostsPerPage = 2;
            string blogId = null;

            if (RouteData.Values.ContainsKey("tenant") && RouteData.Values["tenant"] is BlogItem)
            {
                BlogItem tenant = RouteData.Values["tenant"] as BlogItem;

                blogId = tenant.Id;
                ViewData["Title"] = tenant.Title;
                ViewData["Description"] = tenant.Description;
                PostsPerPage = tenant.PostsPerPage;
            }
            else
            {
                ViewData["Title"] = _manifest.Name;
                ViewData["Description"] = _manifest.Description;
                PostsPerPage = _settings.Value.PostsPerPage;
            }

            ViewData["prev"] = $"/{page + 1}/";
            ViewData["next"] = $"/{(page <= 1 ? null : page - 1 + "/")}";

            IEnumerable<Post> posts = await _blogPostService.GetPostsAsync(blogId, PostsPerPage, PostsPerPage * page);

            string host = Request.Scheme + "://" + Request.Host;

            List<PostView> postview = new List<PostView>();

            foreach (var item in posts)
            {
                PostView post = new PostView(item);

                if (RouteData.Values["tenant"] is BlogItem)
                {
                    post.Url = $"{Request.Scheme}://{Request.Host}/blog/{item.Slug}";
                }
                else
                {
                    string SubDomain = await _blogService.GetSubDomainAsync(item.BlogId);
                    post.Url = $"{Request.Scheme}://{SubDomain}.{Request.Host}/blog/{item.Slug}";
                }

                postview.Add(post);
            }

            return View("~/Views/Blog/Index.cshtml", postview);
        }

        [Route("category/{category}/{page:int?}")]
        //[OutputCache(Profile = "default")]
        public async Task<IActionResult> Category(string category, int page = 0)
        {
            string blogId = null;

            if (RouteData.Values.ContainsKey("tenant") && RouteData.Values["tenant"] is BlogItem)
            {
                BlogItem blog = RouteData.Values["tenant"] as BlogItem;
                blogId = blog.Id;
                ViewData["Title"] = $"{blog.Title} {category}";
            }
            else
            {
                ViewData["Title"] = $"{_manifest.Name} {category}";
            }

            var posts = (await _blogPostService.GetPostsByCategory(blogId, category, _settings.Value.PostsPerPage, _settings.Value.PostsPerPage * page));

            ViewData["Description"] = $"Articles posted in the {category} category";
            ViewData["prev"] = $"/blog/category/{category}/{page + 1}/";
            ViewData["next"] = $"/blog/category/{category}/{(page <= 1 ? null : page - 1 + "/")}";

            return View("~/Views/Blog/Index.cshtml", posts);
        }
        
        [Route("{slug?}")]
        //[OutputCache(Profile = "default")]
        public async Task<IActionResult> Post(string slug)
        {
            if (RouteData.Values.ContainsKey("tenant") && RouteData.Values["tenant"] is BlogItem)
            {
                BlogItem blog = RouteData.Values["tenant"] as BlogItem;

                Post temp = await _blogPostService.GetPostBySlug(blog.Id, slug);
                if (temp != null)
                {
                    var post = new PostView(temp);

                    post.Url = $"{Request.Scheme}://{Request.Host}/blog/{post.Slug}";
                    return View(post);
                }
            }

            return NotFound();
        }

        [Route("edit/{id?}")]
        [HttpGet, Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            if (RouteData.Values.ContainsKey("tenant") && RouteData.Values["tenant"] is BlogItem)
            {
                BlogItem blog = RouteData.Values["tenant"] as BlogItem;
                
                if (string.IsNullOrEmpty(id))
                {
                    return View(new Post());
                }

                var post = await _blogPostService.GetPostById(blog.Id, id);

                if (post != null)
                {
                    return View(post);
                }
            }

            return NotFound();
        }

        [Route("{slug?}")]
        [HttpPost, Authorize, AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UpdatePost(Post post)
        {

            if (!ModelState.IsValid)
            {
                return View("Edit", post);
            }

            if (RouteData.Values.ContainsKey("tenant") && RouteData.Values["tenant"] is BlogItem)
            {
                BlogItem blog = RouteData.Values["tenant"] as BlogItem;

                var existing = await _blogPostService.GetPostById(null, post.Id) ?? post;
                string categories = Request.Form["categories"];

                existing.Categories = categories.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim().ToLowerInvariant()).ToList();
                existing.Title = post.Title.Trim();
                existing.Slug = !string.IsNullOrWhiteSpace(post.Slug) ? post.Slug : post.Title.GenerateSlug();
                existing.Status = post.Status;
                existing.Content = post.Content.Trim();
                existing.Excerpt = post.Excerpt.Trim();

                existing.Content = await SaveFilesToDiskAsync(existing.Content);

                await _blogPostService.SavePost(existing);

                return Redirect(existing.GetLink());
            }

            return NotFound();
        }

        private async Task<string> SaveFilesToDiskAsync(string content)
        {
            string returnContent;

            var imgRegex = new Regex("<img[^>].+ />", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var base64Regex = new Regex("data:[^/]+/(?<ext>[a-z]+);base64,(?<base64>.+)", RegexOptions.IgnoreCase);

            foreach (Match match in imgRegex.Matches(content))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<root>" + match.Value + "</root>");

                var img = doc.FirstChild.FirstChild;
                var srcNode = img.Attributes["src"];
                var fileNameNode = img.Attributes["data-filename"];

                // The HTML editor creates base64 DataURIs which we'll have to convert to image files on disk
                if (srcNode != null && fileNameNode != null)
                {
                    var base64Match = base64Regex.Match(srcNode.Value);
                    if (base64Match.Success)
                    {
                        byte[] bytes = Convert.FromBase64String(base64Match.Groups["base64"].Value);
                        srcNode.Value = await _blogPostService.SaveFile(bytes, fileNameNode.Value).ConfigureAwait(false);

                        img.Attributes.Remove(fileNameNode);
                        returnContent = content.Replace(match.Value, img.OuterXml);
                    }
                }
            }

            return content;
        }

        [Route("deletepost/{id}")]
        [HttpPost, Authorize, AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeletePost(string id)
        {
            if (RouteData.Values.ContainsKey("tenant") && RouteData.Values["tenant"] is BlogItem)
            {
                BlogItem blog = RouteData.Values["tenant"] as BlogItem;

                var existing = await _blogPostService.GetPostById(blog.Id, id);

                if (existing != null)
                {
                    await _blogPostService.DeletePost(existing);
                    return Redirect("/");
                }
            }

            return NotFound();
        }

        [Route("comment/{postId}")]
        [HttpPost]
        public async Task<IActionResult> AddComment(string postId, Comment comment)
        {
            if (RouteData.Values.ContainsKey("tenant") && RouteData.Values["tenant"] is BlogItem)
            {
                BlogItem blog = RouteData.Values["tenant"] as BlogItem;

                var post = await _blogPostService.GetPostById(blog.Id, postId);

                if (!ModelState.IsValid)
                {
                    return View("Post", post);
                }

                if (post == null || !post.AreCommentsOpen(_settings.Value.CommentsCloseAfterDays))
                {
                    return NotFound();
                }

                comment.IsAdmin = User.Identity.IsAuthenticated;
                comment.Content = comment.Content.Trim();
                comment.Author = comment.Author.Trim();
                comment.Email = comment.Email.Trim();

                // the website form key should have been removed by javascript
                // unless the comment was posted by a spam robot
                if (!Request.Form.ContainsKey("website"))
                {
                    post.Comments.Add(comment);
                    await _blogPostService.AddCommentAsync(post.Id, comment);
                }

                return Redirect(post.GetLink() + "#" + comment.Id);
            }

            return NotFound();
        }

        [Route("comment/{postId}/{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(string postId, string commentId)
        {
            if (RouteData.Values.ContainsKey("tenant") && RouteData.Values["tenant"] is BlogItem)
            {
                BlogItem blog = RouteData.Values["tenant"] as BlogItem;

                var post = await _blogPostService.GetPostById(blog.Id, postId);

                if (post == null)
                {
                    return NotFound();
                }

                var comment = post.Comments.FirstOrDefault(c => c.Id.Equals(commentId, StringComparison.OrdinalIgnoreCase));

                if (comment == null)
                {
                    return NotFound();
                }

                post.Comments.Remove(comment);

                //Todo: Change to delete comment
                await _blogPostService.SavePost(post);

                return Redirect(post.GetLink() + "#comments");
            }

            return NotFound();
        }

        [Route("error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
