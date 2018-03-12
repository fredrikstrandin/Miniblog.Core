using Multiblog.Core.Repository;
using Multiblog.Model;
using Multiblog.Model.Blog;
using Multiblog.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Multiblog.Service.Blog
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        private static Dictionary<string, string> _blogIds = new Dictionary<string, string>();

        public BlogService(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<List<string>> GetSearchableAsync()
        {
            return await _blogRepository.GetSearchableAsync();            
        }

        public async Task<bool> IsSearchableAsync(string subDomain)
        {
            return await _blogRepository.IsSearchableAsync(subDomain);
        }

        public async Task<string> GetBlogIdAsync(string name)
        {
            if(_blogIds.ContainsKey(name))
            {
                return _blogIds[name];
            }
            else
            {
                string id = await _blogRepository.GetBlogIdAsync(name);

                if (!string.IsNullOrEmpty(id))
                {
                    _blogIds.Add(name, id);
                }

                return id;
            }
        }

        public async Task<string> CreateAsync(BlogItem blogItem)
        {
            return await _blogRepository.CreateAsync(blogItem);
        }

        public async Task<IEnumerable<BlogPostInfo>> GetBlogPostsAsync(string tenant)
        {
            return await _blogRepository.GetBlogPostsAsync(tenant);
        }
    }
}
