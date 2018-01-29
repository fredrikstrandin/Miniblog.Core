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
using Venter.Utilities;
using WilderMinds.MetaWeblog;

namespace Miniblog.Core.Services
{
    public class MetaWeblogService : IMetaWeblogProvider
    {
        private readonly IBlogService _blog;
        private readonly IConfiguration _config;
        private readonly IUserServices _userServices;
        private readonly IHttpContextAccessor _context;

        public MetaWeblogService(IBlogService blog, IConfiguration config, IHttpContextAccessor context, IUserServices userServices)
        {
            _blog = blog;
            _config = config;
            _userServices = userServices;
            _context = context;
        }

        public string AddPost(string blogid, string username, string password, WilderMinds.MetaWeblog.Post post, bool publish)
        {
            ValidateUserAsync(username, password).GetAwaiter();

            var newPost = new Models.Post
            {
                Title = post.title,
                Slug = !string.IsNullOrWhiteSpace(post.wp_slug) ? post.wp_slug : post.title.GenerateSlug(),
                Content = post.description,
                IsPublished = publish,
                Categories = post.categories
            };

            if (post.dateCreated != DateTime.MinValue)
            {
                newPost.PubDate = post.dateCreated;
            }

            _blog.SavePost(newPost).GetAwaiter().GetResult();

            return newPost.ID;
        }

        public bool DeletePost(string key, string postid, string username, string password, bool publish)
        {
            ValidateUserAsync(username, password).GetAwaiter();

            var post = _blog.GetPostById(postid).GetAwaiter().GetResult();

            if (post != null)
            {
                _blog.DeletePost(post).GetAwaiter().GetResult();
                return true;
            }

            return false;
        }

        public bool EditPost(string postid, string username, string password, WilderMinds.MetaWeblog.Post post, bool publish)
        {
            ValidateUserAsync(username, password).GetAwaiter();

            var existing = _blog.GetPostById(postid).GetAwaiter().GetResult();

            if (existing != null)
            {
                existing.Title = post.title;
                existing.Slug = post.wp_slug;
                existing.Content = post.description;
                existing.IsPublished = publish;
                existing.Categories = post.categories;

                if (post.dateCreated != DateTime.MinValue)
                {
                    existing.PubDate = post.dateCreated;
                }

                _blog.SavePost(existing).GetAwaiter().GetResult();

                return true;
            }

            return false;
        }

        public CategoryInfo[] GetCategories(string blogid, string username, string password)
        {
            ValidateUserAsync(username, password).GetAwaiter();

            return _blog.GetCategories().GetAwaiter().GetResult()
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
            ValidateUserAsync(username, password).GetAwaiter();

            var post = _blog.GetPostById(postid).GetAwaiter().GetResult();

            if (post != null)
            {
                return ToMetaWebLogPost(post);
            }

            return null;
        }

        public WilderMinds.MetaWeblog.Post[] GetRecentPosts(string blogid, string username, string password, int numberOfPosts)
        {
            ValidateUserAsync(username, password).GetAwaiter().GetResult();

            var sub = _context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject)?.Value;

            return _blog.GetPosts(numberOfPosts).GetAwaiter().GetResult().Select(ToMetaWebLogPost).ToArray();
        }

        public BlogInfo[] GetUsersBlogs(string key, string username, string password)
        {
            ValidateUserAsync(username, password).GetAwaiter();

            var request = _context.HttpContext.Request;
            string url = request.Scheme + "://" + request.Host;

            return new[] 
            {
                new BlogInfo {
                    blogid ="1",
                    blogName = "Odesus", //_config["blog:name"],
                    url = url
                },
                    new BlogInfo {
                    blogid ="2",
                    blogName = "Viktor", //_config["blog:name"],
                    url = url
                },
                new BlogInfo {
                    blogid ="1",
                    blogName = "Marija", //_config["blog:name"],
                    url = url
                }
            };
        }

        public MediaObjectInfo NewMediaObject(string blogid, string username, string password, MediaObject mediaObject)
        {
            ValidateUserAsync(username, password).GetAwaiter();
            byte[] bytes = Convert.FromBase64String(mediaObject.bits);
            string path = _blog.SaveFile(bytes, mediaObject.name).GetAwaiter().GetResult();

            return new MediaObjectInfo { url = path };
        }

        public UserInfo GetUserInfo(string key, string username, string password)
        {
            ValidateUserAsync(username, password).GetAwaiter();

            throw new NotImplementedException();
        }

        public int AddCategory(string key, string username, string password, NewCategory category)
        {
            ValidateUserAsync(username, password).GetAwaiter();
            throw new NotImplementedException();
        }

        private async Task ValidateUserAsync(string username, string password)
        {
            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "MetaWeblog.client", "KanBandyBliKul");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(username, password, "blog");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                throw new MetaWeblogException("Unauthorized");
            }
            
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, username));

            var stream = tokenResponse.AccessToken;
            var handler = new JwtSecurityTokenHandler();

            JwtSecurityToken jsonToken = (JwtSecurityToken)handler.ReadToken(stream);

            if (jsonToken?.Claims != null)
            {
                foreach (var item in jsonToken.Claims)
                {
                    identity.AddClaim(item);
                }
            }

            _context.HttpContext.User = new ClaimsPrincipal(identity);
        }

        private WilderMinds.MetaWeblog.Post ToMetaWebLogPost(Models.Post post)
        {
            var request = _context.HttpContext.Request;
            string url = request.Scheme + "://" + request.Host;

            return new WilderMinds.MetaWeblog.Post
            {
                postid = post.ID,
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
