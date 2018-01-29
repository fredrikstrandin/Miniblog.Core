using System.Security.Claims;
using System.Threading.Tasks;
using Vivius.Model.User;

namespace Vivius.Interface.Repositories
{
    public interface IUserRepository
    {
        Task<string[]> CheckEmailChangeAsync(string code);
        Task<bool> CheckIfExistAsync(string email);
        Task<bool> CommitEmailChange(string NewEmail, string OldEmail);
        Task<string> CreateAsync(UserItem user);
        Task<bool> DeleteAccountAsync(string userId);
        Task<UserEmailItem> FindByExternalProviderAsync(string provider, string ProviderSubjectId);
        Task<string> GetIdByEmailAsync(string email);
        Task<UserProfileItem> GetProfileAsync(string userId);
        Task RequestEmailChangeAsync(string newEmail, string oldEmail, string code);
        Task<bool> SetVerifyEmailAsync(string userId);
        Task UpdateLastLoginAsync(string userId);
    }
}