using System.Threading.Tasks;

namespace Multiblog.Service.Interface
{
    public interface IDatabaseToolRepository
    {
        Task CreateIndexAsync();
        Task DeleteBlogStore();
        Task DropAllIndexAsync();
    }
}