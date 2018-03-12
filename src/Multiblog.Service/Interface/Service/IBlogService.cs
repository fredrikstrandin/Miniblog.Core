using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Multiblog.Model;
using Multiblog.Model.Blog;

namespace Multiblog.Service.Interface
{
    public interface IBlogService
    {
        Task<List<string>> GetSearchableAsync();
        Task<bool> IsSearchableAsync(string subDomain);
        Task<string> GetBlogIdAsync(string name);
        Task<string> CreateAsync(BlogItem blogItem);
        Task<IEnumerable<BlogPostInfo>> GetBlogPostsAsync(string tenant);
    }
}
