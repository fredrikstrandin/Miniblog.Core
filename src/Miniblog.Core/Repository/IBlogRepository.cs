using System.Collections.Generic;
using System.Threading.Tasks;
using Miniblog.Core.Models;

namespace Miniblog.Core.Repository
{
    public interface IBlogRepository
    {
        Task<IEnumerable<Post>> GetPostsAsync(bool isAdmin, int count, int skip);
        Task<IEnumerable<Post>> GetPostsByCategoryAsync(string category, bool isAdmin);
        Task<Post> GetPostBySlugAsync(string slug, bool isAdmin);
        Task<Post> GetPostByIdAsync(string id, bool isAdmin);
        Task<string> SavePostAsync(Post post);
        Task DeletePostAsync(Post post);
        Task<List<string>> GetCategoryAsync(string id);
        Task AddCommentAsync(string id, Comment comment);
        Task UpdatePostAsync(Post existing);
    }
}