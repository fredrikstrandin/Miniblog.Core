using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Miniblog.Core.Models;
using Miniblog.Core.Repository;

namespace Miniblog.Core.Services
{
    public class FileBlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IFileRepository _file;
        private readonly IHttpContextAccessor _contextAccessor;

        public FileBlogService(IHostingEnvironment env,
            IHttpContextAccessor contextAccessor,
            IBlogRepository blogRepository,
            IFileRepository file,
            ILogger<FileBlogService> logger)
        {
            _blogRepository = blogRepository;
            _file = file;
            _contextAccessor = contextAccessor;
        }

        public virtual async Task<IEnumerable<Post>> GetPosts(int count, int skip = 0)
        {
            bool isAdmin = IsAdmin();

            var posts = await _blogRepository.GetPostsAsync(isAdmin, count, skip);

            return posts;
        }

        public virtual Task<IEnumerable<Post>> GetPostsByCategory(string category)
        {
            bool isAdmin = IsAdmin();

            var posts = _blogRepository.GetPostsByCategoryAsync(category, isAdmin);

            return posts;

        }

        public virtual Task<Post> GetPostBySlug(string slug)
        {
            bool isAdmin = IsAdmin();

            var post = _blogRepository.GetPostBySlugAsync(slug, isAdmin);

            return post;
        }

        public async virtual Task<Post> GetPostById(string id)
        {
            bool isAdmin = IsAdmin();

            Post post = await _blogRepository.GetPostByIdAsync(id, isAdmin);

            return post;
        }

        public async virtual Task<IEnumerable<string>> GetCategories()
        {
            bool isAdmin = IsAdmin();

            IEnumerable<string> categories = await _blogRepository.GetCategoriesAsync(isAdmin);
                        
            return categories;
        }

        public async Task SavePost(Post post)
        {
            await _blogRepository.SavePostAsync(post);            
        }

        public async Task DeletePost(Post post)
        {
            await _blogRepository.DeletePostAsync(post);            
        }

        public async Task<string> SaveFile(byte[] bytes, string fileName, string suffix = null)
        {
            return await _file.SaveFileAsync(bytes, fileName, suffix);            
        }

        protected bool IsAdmin()
        {
            return _contextAccessor.HttpContext?.User?.Identity.IsAuthenticated == true;
        }

    }
}
