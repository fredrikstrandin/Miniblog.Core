using System.Threading.Tasks;

namespace Multiblog.Service.Interface
{
    public interface IOAuthServices
    {
        Task ValidateUserAsync(string username, string password);
    }
}