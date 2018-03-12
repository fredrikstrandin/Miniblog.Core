using System.Collections.Generic;
using System.Threading.Tasks;
using Multiblog.Model;
using Multiblog.Model.Blog;

namespace Multiblog.Service.Blog
{
    public interface IBlogRepository
    {
        Task<List<string>> GetSearchableAsync();
        Task<bool> IsSearchableAsync(string subDomain);
        Task<string> GetBlogIdAsync(string name);
        Task AddPostUrl(string blogId, string id, string slug);
        Task<string> CreateAsync(BlogItem blogItem);
        Task<IEnumerable<BlogPostInfo>> GetBlogPostsAsync(string tenant);
    }
}