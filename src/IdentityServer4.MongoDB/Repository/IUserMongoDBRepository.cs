using System.Threading.Tasks;
using MongoDB.Bson;
using IdentityServer4.MongoDB.Service;
using IdentityServer4.MongoDB.Model;
using System.Security.Claims;
using System.Collections.Generic;
using System;

namespace IdentityServer4.MongoDB.Repository
{
    public interface IUserMongoDBRepository
    {
        Task<OAuthUserEntity> FindBySubjectIdAsync(string subjectId);
        Task<List<Claim>> FindClaimsBySubjectIdAsync(string subjectId);
        Task<OAuthUserEntity> FindByEmailAsync(string email);
        Task<bool> SetPasswordAsync(string userId, string newPassword, string email);
        Task<string> CreateLoginAsync(OAuthUserItem item);
        Task<bool> GetIfPasswordExists(string userId);
        Task<bool> InsertRole(string UserId, string role);
        Task<string> FindIdByEmailAsync(string email);
        Task<OAuthUserItem> FindByExternalProviderAsync(string provider, string subjectId);
        Task<bool> IsActiveAsync(string id);
        Task UpsertTempOAuthUserAsync(string provider, string subjectId, List<Claim> claims, string access_token, DateTime? expiresAt);
        Task<List<Claim>> GetTempOAuthUserClaimsAsync(string provider, string subjectId);
        Task<TempOAuthUserAccessTokenItem> GetTempOAuthUserAccessTokenAsync(string provider, string subjectId);
        Task UpdateAccessToken(string provider, string subjectId, string accesstoken, DateTime? expiresAt);
    }
}