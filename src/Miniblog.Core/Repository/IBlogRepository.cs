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
        Task<IEnumerable<string>> GetCategoriesAsync(bool isAdmin);
        Task SavePostAsync(Post post);
        Task DeletePostAsync(Post post);
        Task AddCommentAsync(string id, Comment comment);
        Task UpdatePostAsync(Post existing);
    }
}