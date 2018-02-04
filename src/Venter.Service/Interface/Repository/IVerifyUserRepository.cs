using System.Threading.Tasks;

namespace Miniblog.Core.Repository.VerifyUser
{
    public interface IVerifyUserRepository
    {
        Task<string> CreateCodeAsync(string userId);
        Task<string> GetUserFromVerifyCodeAsync(string code);
        Task<bool> VerifyCodeAsync(string code, string userId);
        Task DeleteVerifyCodeAsync(string code);
    }
}