using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Multiblog.Core.Attribute;
using Multiblog.Core.Models;
using Multiblog.Model;
using Multiblog.Service.Interface;
using Multiblog.Utilities;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using WebEssentials.AspNetCore.Pwa;

namespace Multiblog.Core.Controllers
{
    [ServiceFilter(typeof(TenantAttribute))]
    public class BlogController : Controller
    {
        private readonly IBlogPostService _blogPostService;
        private readonly IBlogService _blogService;
        private readonly IOptionsSnapshot<BlogSettings> _settings;
        private readonly WebManifest _manifest;

        public BlogController(IBlogPostService blogPostService,
            IBlogService blogService,
            IOptionsSnapshot<BlogSettings> settings,
            WebManifest manifest)
        {
            _blogPostService = blogPostService;
            _blogService = blogService;
            _settings = settings;
            _manifest = manifest;
        }

        [Route("/{page:int?}")]
        [OutputCache(Profile = "default")]
        public async Task<IActionResult> Index([FromRoute]int page = 0)
        {
            int PostsPerPage = 2;
            string blogId = null;

            if (RouteData.Values.ContainsKey("tenant") && RouteData.Values["tenant"] is BlogItem)
            {
                BlogItem item = RouteData.Values["tenant"] as BlogItem;

                blogId = item.Id;
                ViewData["Title"] = item.Title;
                ViewData["Description"] = item.Description;
                PostsPerPage = item.PostsPerPage;
            }
            else
            {
                ViewData["Title"] = _manifest.Name;
                ViewData["Description"] = _manifest.Description;
                PostsPerPage = _settings.Value.PostsPerPage;
            }

            ViewData["prev"] = $"/{page + 1}/";
            ViewData["next"] = $"/{(page <= 1 ? null : page - 1 + "/")}";

            var posts = await _blogPostService.GetPostsAsync(blogId, PostsPerPage, PostsPerPage * page);

            return View("~/Views/Blog/Index.cshtml", posts);
        }

        [Route("/blog/category/{category}/{page:int?}")]
        [OutputCache(Profile = "default")]
        public async Task<IActionResult> Category(string category, int page = 0)
        {
            var posts = (await _blogPostService.GetPostsByCategory(null, category, _settings.Value.PostsPerPage, _settings.Value.PostsPerPage * page));
            ViewData["Title"] = _manifest.Name + " " + category;
            ViewData["Description"] = $"Articles posted in the {category} category";
            ViewData["prev"] = $"/blog/category/{category}/{page + 1}/";
            ViewData["next"] = $"/blog/category/{category}/{(page <= 1 ? null : page - 1 + "/")}";
            return View("~/Views/Blog/Index.cshtml", posts);
        }

        // This is for redirecting potential existing URLs from the old Multiblog URL format
        [Route("/post/{slug}")]
        [HttpGet]
        public IActionResult Redirects(string slug)
        {
            return LocalRedirectPermanent($"/blog/{slug}");
        }

        [Route("/blog/{slug?}")]
        [OutputCache(Profile = "default")]
        public async Task<IActionResult> Post(string slug)
        {
            if (!string.IsNullOrEmpty(Request.Tenant()))
            {
                string blogId = await _blogService.GetBlogIdAsync(Request.Tenant());
                var post = await _blogPostService.GetPostBySlug(blogId, slug);

                if (post != null)
                {
                    return View(post);
                }
            }
            return NotFound();
        }

        [Route("/blog/edit/{id?}")]
        [HttpGet, Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View(new Post());
            }

            var post = await _blogPostService.GetPostById(null, id);

            if (post != null)
            {
                return View(post);
            }

            return NotFound();
        }

        [Route("/blog/{slug?}")]
        [HttpPost, Authorize, AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UpdatePost(Post post)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", post);
            }

            if (RouteData.Values.ContainsKey("tenant") && RouteData.Values["tenant"] is BlogItem)
            {
                BlogItem blogItem =  RouteData.Values["tenant"] as BlogItem;

                if (string.IsNullOrEmpty(post.BlogId))
                {
                    post.BlogId = blogItem.Id;
                }

                var existing = await _blogPostService.GetPostById(blogItem.Id, post.Id) ?? post;
                string categories = Request.Form["categories"];

                existing.Categories = categories.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim().ToLowerInvariant()).ToList();
                existing.Title = post.Title.Trim();
                existing.Slug = !string.IsNullOrWhiteSpace(post.Slug) ? post.Slug : post.Title.GenerateSlug();
                existing.Status = post.Status;
                existing.Content = post.Content.Trim();
                existing.Excerpt = post.Excerpt.Trim();

                existing.Content = await SaveFilesToDiskAsync(existing.Content);

                await _blogPostService.SavePost(existing);

                return Redirect(post.GetLink());
            }
            else
            {
                return NotFound();
            }
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

        [Route("/blog/deletepost/{id}")]
        [HttpPost, Authorize, AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeletePost(string id)
        {
            var existing = await _blogPostService.GetPostById(null, id);

            if (existing != null)
            {
                await _blogPostService.DeletePost(existing);
                return Redirect("/");
            }

            return NotFound();
        }

        [Route("/blog/comment/{postId}")]
        [HttpPost]
        public async Task<IActionResult> AddComment(string postId, Comment comment)
        {
            var post = await _blogPostService.GetPostById(null, postId);

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

        [Route("/blog/comment/{postId}/{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(string postId, string commentId)
        {
            var post = await _blogPostService.GetPostById(null, postId);

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
            await _blogPostService.SavePost(post);

            return Redirect(post.GetLink() + "#comments");
        }
    }
}
