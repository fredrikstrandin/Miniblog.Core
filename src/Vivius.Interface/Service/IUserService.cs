using System.Collections.Generic;
using System.Threading.Tasks;
using Vivius.Model.User;

namespace Venter.Service.UserService
{
    public interface IUserService
    {
        Task<string[]> CheckEmailChangeAsync(string code);
        Task<bool> CheckIfExistAsync(string email);
        Task<bool> CommitEmailChange(string NewEmail, string OldEmail);
        Task<string> CreateAsync(UserItem user, bool sendEmail = true);
        Task<bool> DeleteAccountAsync(string userId);
        Task<UserEmailItem> FindByExternalProviderAsync(string provider, string userId);
        Task<string> GetIdByEmailAsync(string id);
        Task<UserProfileItem> GetProfileAsync(string sub);
        Task RequestEmailChangeAsync(string newEmail, string oldEmail, string code);
        Task<bool> SetVerifyEmailAsync(string userId);
        Task UpdateLastLogin(string id);
    }
}