using System.Collections.Generic;
using System.Threading.Tasks;
using Miniblog.Core.Model.User;

namespace Venter.Service.UserService
{
    public interface IUserService
    {
        Task<string[]> CheckEmailChangeAsync(string code);
        Task<bool> CheckIfExistAsync(string email);
        Task<bool> CommitEmailChange(string NewEmail, string OldEmail);
        Task<string> CreateAsync(UserItem user);
        Task<bool> DeleteAccountAsync(string userId);
        Task<UserEmailItem> FindByExternalProviderAsync(string provider, string userId);
        Task<string> GetIdByEmailAsync(string id);
        Task<UserProfileItem> GetProfileAsync(string sub);
        Task RequestEmailChangeAsync(string newEmail, string oldEmail, string code);
        Task<bool> SetVerifyEmailAsync(string userId);
        Task UpdateLastLogin(string id);
        Task<bool> IfUserOwensBlogAsync(string sub, string blogId);
    }
}