using IdentityModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Multiblog.Core.Models;
using Multiblog.Core.Repository;
using Multiblog.Service.Blog;
using Multiblog.Service.Interface;
using Multiblog.Service.UserService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Multiblog.Core.Services
{
    public class BlogPostService : IBlogPostService
    {

        private readonly IUserService _userService;

        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly IFileRepository _file;
        private readonly IHttpContextAccessor _contextAccessor;

        public BlogPostService(IHostingEnvironment env,
            IHttpContextAccessor contextAccessor,
            IBlogPostRepository blogPostRepository,
            IBlogRepository blogRepository,
        IFileRepository file,
            ILogger<BlogPostService> logger,
            IUserService userService)
        {
            _blogPostRepository = blogPostRepository;
            _blogRepository = blogRepository;
            _file = file;
            _contextAccessor = contextAccessor;
            _userService = userService;
        }

        public virtual async Task<IEnumerable<Post>> GetPostsAsync(string blogId, int count, int skip = 0)
        {
            bool isAdmin = await IsAdmin(blogId);

            var posts = await _blogPostRepository.GetPostsAsync(blogId, isAdmin, count, skip);

            return posts;
        }

        public virtual async Task<IEnumerable<Post>> GetPostsByCategory(string blogId, string category, int count, int skip = 0)
        {
            bool isAdmin = await IsAdmin(blogId);

            return await _blogPostRepository.GetPostsByCategoryAsync(category, isAdmin, count, skip);
        }

        public virtual async Task<Post> GetPostBySlug(string blogId, string slug)
        {
            return await _blogPostRepository.GetPostBySlugAsync(blogId, slug);
        }

        public async virtual Task<Post> GetPostById(string blogId, string id)
        {
            bool isAdmin = await IsAdmin(blogId);

            return await _blogPostRepository.GetPostByIdAsync(id, isAdmin);
        }

        public async virtual Task<IEnumerable<string>> GetCategoryAsync(string blogId)
        {
            IEnumerable<string> categories = await _blogPostRepository.GetCategoryAsync(blogId);

            return categories;
        }

        public async Task<string> SavePost(Post post)
        {
            string id = string.Empty;

            id = await _blogPostRepository.SavePostAsync(post);

            if (!string.IsNullOrEmpty(id) && post.Status == Status.Publish)
            {
                await _blogRepository.AddPostUrl(post.BlogId, id, post.Slug);
            }
            return id;
        }

        public async Task DeletePost(Post post)
        {
            await _blogPostRepository.DeletePostAsync(post);
        }

        public async Task<string> SaveFile(byte[] bytes, string fileName, string suffix = null)
        {
            return await _file.SaveFileAsync(bytes, fileName, suffix);
        }

        protected async Task<bool> IsAdmin(string blogId)
        {
            string sub = _contextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject)?.Value;

            if (string.IsNullOrEmpty(sub))
            {
                return false;
            }

            return await _userService.IfUserOwensBlogAsync(sub, blogId);
        }

        public async Task AddCommentAsync(string id, Comment comment)
        {
            await _blogPostRepository.AddCommentAsync(id, comment);
        }

        public async Task UpdatePostAsync(Post existing)
        {
            await _blogPostRepository.UpdatePostAsync(existing);
        }
    }
}
