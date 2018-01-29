using System.Threading.Tasks;
using IdentityServer4.MongoDB.Model;
using IdentityServer4.MongoDB.Model.Enum;
using System.Collections.Generic;
using System.Security.Claims;
using System;

namespace IdentityServer4.MongoDB.Service
{
    public interface IUserMongoDBService
    {
        Task<bool> ValidateCredentialsAsync(string username, string password);
        Task<OAuthUserEntity> FindByEmailAsync(string email);
        Task<string> FindIdByEmailAsync(string email);
        Task<OAuthUserEntity> FindBySubjectIdAsync(string id);
        Task<List<Claim>> FindClaimsBySubjectIdAsync(string id);
        Task<bool> SetPasswordAsync(string sub, string newPassword, string email);
        Task<string> CreateLoginAsync(OAuthUserCreate oAuthUserEntity);
        Task<bool> GetIfPasswordExists(string userId);
        Task<OAuthUserItem> FindByExternalProviderAsync(string provider, string subjectId);
        Task<bool> IsActiveAsync(string id);
        Task CreateTempOAuthUserAsync(string provider, string subjectId, List<Claim> claims, string token, DateTime? access_token);
        Task<List<Claim>> GetTempOAuthUserClaimsAsync(string provider, string subjectId);
        Task<TempOAuthUserAccessTokenItem> GetTempOAuthUserAccessTokenAsync(string provider, string subjectId);
        Task UpdateAccessToken(string provider, string subjectId, string access_token, DateTime? expiresAt);
    }
}