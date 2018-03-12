using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Multiblog.Core.Models;

namespace Multiblog.Service.Interface
{
    public interface IBlogPostService
    {
        Task<IEnumerable<Post>> GetPostsAsync(string blogId, int count, int skip = 0);
        Task<IEnumerable<Post>> GetPostsByCategory(string blogId, string category, int count, int skip = 0);
        Task<Post> GetPostBySlug(string blogId, string slug);
        Task<Post> GetPostById(string blogId, string id);
        Task<IEnumerable<string>> GetCategoryAsync(string blogId);
        Task<string> SavePost(Post post);
        Task DeletePost(Post post);
        Task<string> SaveFile(byte[] bytes, string fileName, string suffix = null);
        Task AddCommentAsync(string iD, Comment comment);
        Task UpdatePostAsync(Post existing);
    }
}
