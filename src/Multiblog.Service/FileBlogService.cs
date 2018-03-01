using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using IdentityModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Multiblog.Core.Models;
using Multiblog.Core.Repository;
using Multiblog.Service.UserService;
using Multiblog.Model;

namespace Multiblog.Core.Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IFileRepository _file;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserService _userService;

        public BlogService(IHostingEnvironment env,
            IHttpContextAccessor contextAccessor,
            IBlogRepository blogRepository,
            IFileRepository file,
            ILogger<BlogService> logger,
            IUserService userService)
        {
            _blogRepository = blogRepository;
            _file = file;
            _contextAccessor = contextAccessor;
            _userService = userService;
        }

        public virtual async Task<IEnumerable<Post>> GetPostsAsync(string blogId, int count, int skip = 0)
        {
            bool isAdmin = await IsAdmin(blogId);

            var posts = await _blogRepository.GetPostsAsync(blogId, isAdmin, count, skip);

            return posts;
        }

        public virtual async Task<IEnumerable<Post>> GetPostsByCategory(string blogId, string category, int count, int skip = 0)
        {
            bool isAdmin = await IsAdmin(blogId);

            return await _blogRepository.GetPostsByCategoryAsync(category, isAdmin, count, skip);
        }

        public virtual async Task<Post> GetPostBySlug(string blogId, string slug)
        {
            bool isAdmin = await IsAdmin(blogId);

            return await _blogRepository.GetPostBySlugAsync(slug, isAdmin);            
        }

        public async virtual Task<Post> GetPostById(string blogId, string id)
        {
            bool isAdmin = await IsAdmin(blogId);

            return await _blogRepository.GetPostByIdAsync(id, isAdmin);
        }

        public async virtual Task<IEnumerable<string>> GetCategoryAsync(string blogId)
        {
            IEnumerable<string> categories = await _blogRepository.GetCategoryAsync(blogId);

            return categories;
        }

        public async Task<string> SavePost(Post post)
        {
            return await _blogRepository.SavePostAsync(post);
        }

        public async Task DeletePost(Post post)
        {
            await _blogRepository.DeletePostAsync(post);
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
            await _blogRepository.AddCommentAsync(id, comment);
        }

        public async Task UpdatePostAsync(Post existing)
        {
            await _blogRepository.UpdatePostAsync(existing);
        }
    }
}
