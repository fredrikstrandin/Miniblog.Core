using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Multiblog.Core.Models;
using WilderMinds.MetaWeblog;
using Multiblog.Service.UserService;
using Multiblog.Utilities;
using Multiblog.Service.Interface;

namespace Multiblog.Core.Services
{
    public class MetaWeblogService : IMetaWeblogProvider
    {
        private readonly IBlogPostService _blog;
        private readonly IUserService _userServices;
        private readonly IHttpContextAccessor _context;
        private readonly IOAuthServices _oAuthServices;

        public MetaWeblogService(IBlogPostService blog, 
            IHttpContextAccessor context, 
            IUserService userServices, 
            IOAuthServices oAuthServices)
        {
            _blog = blog;
            _userServices = userServices;
            _context = context;
            _oAuthServices = oAuthServices;
        }

        public string AddPost(string blogid, string username, string password, WilderMinds.MetaWeblog.Post post, bool publish)
        {
            _oAuthServices.ValidateUserAsync(username, password).GetAwaiter().GetResult();

            var newPost = new Models.Post
            {
                BlogId = blogid,
                Title = post.title,
                Slug = !string.IsNullOrWhiteSpace(post.wp_slug) ? post.wp_slug : post.title.GenerateSlug(),
                Content = post.description,
                Status = publish ? Status.Publish : Status.Draft,
                Categories = post.categories
            };

            if (post.dateCreated != DateTime.MinValue)
            {
                newPost.PubDate = post.dateCreated;
            }

            return _blog.SavePost(newPost).GetAwaiter().GetResult();
        }

        public bool DeletePost(string key, string postid, string username, string password, bool publish)
        {
            _oAuthServices.ValidateUserAsync(username, password).GetAwaiter().GetResult();

            var post = _blog.GetPostById(null, postid).GetAwaiter().GetResult();

            if (post != null)
            {
                _blog.DeletePost(post).GetAwaiter().GetResult();
                return true;
            }

            return false;
        }

        public bool EditPost(string postid, string username, string password, WilderMinds.MetaWeblog.Post post, bool publish)
        {
            _oAuthServices.ValidateUserAsync(username, password).GetAwaiter().GetResult();

            var existing = _blog.GetPostById(null, postid).GetAwaiter().GetResult();

            if (existing != null)
            {
                existing.Title = post.title;
                existing.Slug = post.wp_slug;
                existing.Content = post.description;
                existing.Status = publish ? Status.Publish : Status.Draft;
                existing.Categories = post.categories;

                if (post.dateCreated != DateTime.MinValue)
                {
                    existing.PubDate = post.dateCreated;
                }

                _blog.UpdatePostAsync(existing).GetAwaiter().GetResult();

                return true;
            }

            return false;
        }

        public CategoryInfo[] GetCategories(string blogid, string username, string password)
        {
            _oAuthServices.ValidateUserAsync(username, password).GetAwaiter().GetResult();

            return _blog.GetCategoryAsync(blogid).GetAwaiter().GetResult()
                           .Select(cat =>
                               new CategoryInfo
                               {
                                   categoryid = cat,
                                   title = cat
                               })
                           .ToArray();
        }

        public WilderMinds.MetaWeblog.Post GetPost(string postid, string username, string password)
        {
            _oAuthServices.ValidateUserAsync(username, password).GetAwaiter().GetResult();

            var post = _blog.GetPostById(null, postid).GetAwaiter().GetResult();

            if (post != null)
            {
                return ToMetaWebLogPost(post);
            }

            return null;
        }

        public WilderMinds.MetaWeblog.Post[] GetRecentPosts(string blogId, string username, string password, int numberOfPosts)
        {
            _oAuthServices.ValidateUserAsync(username, password).GetAwaiter().GetResult();

            var sub = _context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject)?.Value;

            return _blog.GetPostsAsync(blogId, numberOfPosts).GetAwaiter().GetResult().Select(ToMetaWebLogPost).ToArray();
        }

        public BlogInfo[] GetUsersBlogs(string key, string username, string password)
        {
            _oAuthServices.ValidateUserAsync(username, password).GetAwaiter().GetResult();

            var request = _context.HttpContext.Request;
            string url = request.Scheme + "://" + request.Host;

            return new[]
            {
                new BlogInfo {
                    blogid ="5a766911817a741fe8178c8a",
                    blogName = "Odesus", //_config["blog:name"],
                    url = request.Scheme + "://" + "odesus" + "." + request.Host
                },
                    new BlogInfo {
                    blogid ="5a766911817a741fe8178c8b",
                    blogName = "Flower", //_config["blog:name"],
                    url = request.Scheme + "://" + "flower" + "." + request.Host
                },
                new BlogInfo {
                    blogid ="5a766911817a741fe8178c99",
                    blogName = "Pic of the day", //_config["blog:name"],
                    url = request.Scheme + "://" + "pictures" + "." + request.Host
                }
            };
        }

        public MediaObjectInfo NewMediaObject(string blogid, string username, string password, MediaObject mediaObject)
        {
            _oAuthServices.ValidateUserAsync(username, password).GetAwaiter().GetResult();
            byte[] bytes = Convert.FromBase64String(mediaObject.bits);
            string path = _blog.SaveFile(bytes, mediaObject.name).GetAwaiter().GetResult();

            return new MediaObjectInfo { url = path };
        }

        public UserInfo GetUserInfo(string key, string username, string password)
        {
            _oAuthServices.ValidateUserAsync(username, password).GetAwaiter().GetResult();

            throw new NotImplementedException();
        }

        public int AddCategory(string key, string username, string password, NewCategory category)
        {
            _oAuthServices.ValidateUserAsync(username, password).GetAwaiter().GetResult();
            throw new NotImplementedException();
        }
        
        private WilderMinds.MetaWeblog.Post ToMetaWebLogPost(Models.Post post)
        {
            var request = _context.HttpContext.Request;
            string url = request.Scheme + "://" + request.Host;

            return new WilderMinds.MetaWeblog.Post
            {
                postid = post.Id,
                title = post.Title,
                wp_slug = post.Slug,
                permalink = url + post.GetLink(),
                dateCreated = post.PubDate,
                description = post.Content,
                categories = post.Categories.ToArray()
            };
        }
    }
}
