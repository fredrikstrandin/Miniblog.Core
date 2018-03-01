using System.Threading.Tasks;

namespace Multiblog.Service.Interface
{
    public interface IVerifyUserServices
    {
        Task<string> CreateCodeAsync(string userId);
        Task<bool> VerifyCodeAsync(string code, string userId);
        Task<string> GetUserFromVerifyCodeAsync(string code);
        Task DeleteVerifyCodeAsync(string code);
    }
}