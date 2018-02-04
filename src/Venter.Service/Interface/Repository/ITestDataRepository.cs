using Miniblog.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vivus.Model;

namespace Miniblog.Core.Repository
{
    public interface ITestDataRepository
    {
        Task<List<string>> CreateBlogsAsync(List<BlogItem> list);
        Task CreateBlogPost(List<Post> list);
    }
}