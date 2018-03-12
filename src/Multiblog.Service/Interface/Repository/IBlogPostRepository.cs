using System.Collections.Generic;
using System.Threading.Tasks;
using Multiblog.Core.Models;
using Multiblog.Model;

namespace Multiblog.Core.Repository
{
    public interface IBlogPostRepository
    {
        Task<IEnumerable<Post>> GetPostsAsync(string blogId, bool isAdmin, int count, int skip);
        Task<IEnumerable<Post>> GetPostsByCategoryAsync(string category, bool isAdmin, int count, int skip);
        Task<Post> GetPostBySlugAsync(string blogId, string slug);
        Task<Post> GetPostByIdAsync(string id, bool isAdmin);
        Task<string> SavePostAsync(Post post);
        Task DeletePostAsync(Post post);
        Task<List<string>> GetCategoryAsync(string id);
        Task AddCommentAsync(string id, Comment comment);
        Task UpdatePostAsync(Post existing);
        Task<BlogItem> FindBlogAsync(string subdomain);
    }
}