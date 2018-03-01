using Multiblog.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Multiblog.Model;

namespace Multiblog.Core.Repository
{
    public interface ITestDataRepository
    {
        Task<List<string>> CreateBlogsAsync(List<BlogItem> list);
        Task CreateBlogPost(List<Post> list);
    }
}