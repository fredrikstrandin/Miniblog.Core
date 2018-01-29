using IdentityServer4.MongoDB.Model;
using IdentityServer4.MongoDB.MonogDBContext;
using IdentityServer4.MongoDB.Service;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer4.MongoDB.Repository
{
    public class UserMongoDBRepository : IUserMongoDBRepository
    {
        private readonly IMongoIdentityContext _context;
        private readonly ILogger<UserMongoDBRepository> _logger;
        private readonly IPasswordService _passwordService;

        public UserMongoDBRepository(IMongoIdentityContext context,
            ILogger<UserMongoDBRepository> logger,
            IPasswordService passwordService)
        {
            _context = context;
            _logger = logger;
            _passwordService = passwordService;
        }

        public async Task<string> FindIdByEmailAsync(string email)
        {
            string user = await _context.OAuthUserEntityCollection.Find(x => x.Email == email.ToLower())
                .Project(x => x.Id.ToString())
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<OAuthUserEntity> FindByEmailAsync(string email)
        {
            OAuthUserEntity user = await _context.OAuthUserEntityCollection.Find(x => x.Email == email.ToLower())
                //.Project<OAuthUserEntity>(Builders<OAuthUserEntity>.Projection
                //                                    .Exclude(x => x.CreatedBy))
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<OAuthUserEntity> FindBySubjectIdAsync(string subjectId)
        {
            if (ObjectId.TryParse(subjectId, out ObjectId id))
            {
                return await _context.OAuthUserEntityCollection.Find(x => x.Id == id)
                .FirstOrDefaultAsync();
            }
            else
            {
                return null;
            }
        }

        public async Task<List<Claim>> FindClaimsBySubjectIdAsync(string subjectId)
        {
            if (ObjectId.TryParse(subjectId, out ObjectId id))
            {
                return await _context.OAuthUserEntityCollection.Find(x => x.Id == id)
                    .Project(x => x.Claims)
                    .FirstOrDefaultAsync();
            }
            else
            {
                return null;
            }
        }
        public async Task<string> UpdateAsync(string email, string provider, string userId)
        {
            var filter = Builders<OAuthUserEntity>.Filter.And(Builders<OAuthUserEntity>.Filter.Where(x => x.Email == email));
            var update = Builders<OAuthUserEntity>.Update.Push(x => x.Providers, new Provider() { ProviderName = provider, ProviderSubjectId = userId });

            var user = await _context.OAuthUserEntityCollection.FindOneAndUpdateAsync(filter, update);

            return user?.Id.ToString();
        }

        public async Task<bool> SetPasswordAsync(string userId, string newPassword, string email)
        {
            if (!ObjectId.TryParse(userId, out ObjectId Id))
            {
                return false;
            }

            var salt = _passwordService.GenerateSalt();

            var password = _passwordService.CreateHash(newPassword, salt);

            var filter = Builders<OAuthUserEntity>.Filter.Where(x => x.Id == Id);
            var update = Builders<OAuthUserEntity>.Update.Set(x => x.Salt, salt)
                .Set(x => x.Password, password);

            var ret = await _context.OAuthUserEntityCollection.UpdateOneAsync(filter, update);

            if (ret.MatchedCount == 0)
            {
                await _context.OAuthUserEntityCollection.InsertOneAsync(new OAuthUserEntity()
                {
                    Id = Id,
                    Email = email,
                    Password = password,
                    Salt = salt
                });

                return true;
            }
            else
            {
                return ret.ModifiedCount > 0;
            }
        }

        public async Task<bool> GetIfPasswordExists(string userId)
        {
            if (!ObjectId.TryParse(userId, out ObjectId id))
            {
                return false;
            }

            var filter = Builders<OAuthUserEntity>.Filter.Where(x => x.Id == id);

            string password = await _context.OAuthUserEntityCollection.Find(filter)
                .Project(x => x.Password)
                .FirstOrDefaultAsync();

            return !string.IsNullOrEmpty(password);
        }

        public async Task<bool> CreateOAuthAsync(OAuthUserItem item)
        {
            if (await _context.OAuthUserEntityCollection.CountAsync(x => x.Email == item.Email) > 0)
            {
                var filter = Builders<OAuthUserEntity>.Filter.Where(x => x.Email == item.Email);

                var update = Builders<OAuthUserEntity>.Update
                    .AddToSet(x => x.Providers, item.Providers[0]);

                //TODO: Här skall läggas till claims och roler 


                var ret = await _context.OAuthUserEntityCollection.UpdateOneAsync(filter, update);
            }
            else
            {
                await _context.OAuthUserEntityCollection.InsertOneAsync(new OAuthUserEntity()
                {
                    Email = item.Email,
                    Providers = item.Providers,
                    Roles = item.Roles,
                    Claims = item.Claims
                });
            }

            return true;
        }

        public async Task<string> CreateLoginAsync(OAuthUserItem item)
        {
            if (item == null)
            {
                return null;
            }

            if (await _context.OAuthUserEntityCollection.CountAsync(x => x.Email == item.Email) == 0)
            {
                ObjectId id = ObjectId.GenerateNewId();

                item.Claims.Add(new Claim("sub", id.ToString()));

                OAuthUserEntity user = new OAuthUserEntity()
                {
                    Id = id,
                    Email = item.Email,
                    EmailVerified = item.EmailVerified,
                    Password = item.Password,
                    Salt = item.Salt,
                    Providers = item.Providers,
                    IsActive = item.IsActive,
                    Roles = item.Roles,
                    Claims = item.Claims
                };

                await _context.OAuthUserEntityCollection.InsertOneAsync(user);

                return user.Id.ToString();
            }

            return null;
        }

        public async Task<bool> InsertRole(string userId, string role)
        {
            if (!ObjectId.TryParse(userId, out ObjectId id))
            {
                return false;
            }

            var filter = Builders<OAuthUserEntity>.Filter.And(
                Builders<OAuthUserEntity>.Filter.Eq(x => x.Id, id));

            var roles = await _context.OAuthUserEntityCollection.Find(filter)
                .Project(x => x.Roles)
                .FirstOrDefaultAsync();

            if (roles != null)
            {
                if (!roles.Contains(role))
                {
                    var updatefilter = Builders<OAuthUserEntity>.Filter.Where(x => x.Id == id);
                    var update = Builders<OAuthUserEntity>.Update.AddToSet(x => x.Roles, role);

                    var result = await _context.OAuthUserEntityCollection.UpdateOneAsync(updatefilter, update);
                }

                return true;
            }

            return false;
        }
        public async Task<bool> RemoveRole(string userId, string role)
        {
            if (!ObjectId.TryParse(userId, out ObjectId id))
            {
                return false;
            }

            var filter = Builders<OAuthUserEntity>.Filter.And(
                Builders<OAuthUserEntity>.Filter.Eq(x => x.Id, id));

            var roles = await _context.OAuthUserEntityCollection.Find(filter)
                .Project(x => x.Roles)
                .FirstOrDefaultAsync();

            if (roles != null)
            {
                if (!roles.Contains(role))
                {
                    var updatefilter = Builders<OAuthUserEntity>.Filter.Where(x => x.Id == id);
                    var update = Builders<OAuthUserEntity>.Update.Pull(x => x.Roles, role);

                    var result = await _context.OAuthUserEntityCollection.UpdateOneAsync(updatefilter, update);
                }

                return true;
            }

            return false;
        }

        public async Task<bool> UpsertClaim(string userId, Claim claim)
        {
            if (!ObjectId.TryParse(userId, out ObjectId id))
            {
                return false;
            }

            var filter = Builders<OAuthUserEntity>.Filter.And(
                Builders<OAuthUserEntity>.Filter.Eq(x => x.Id, id));

            var claims = await _context.OAuthUserEntityCollection.Find(filter)
                .Project(x => new { Id = x.Id, Claims = x.Claims })
                .FirstOrDefaultAsync();

            if (claims?.Id != null)
            {
                if (claims?.Claims?.Where(x => x.Type == claim.Type && x.Value == claim.Value)?.FirstOrDefault() == null)
                {
                    var updatefilter = Builders<OAuthUserEntity>.Filter.Where(x => x.Id == id);
                    var update = Builders<OAuthUserEntity>.Update.AddToSet(x => x.Claims, claim);

                    var result = await _context.OAuthUserEntityCollection.UpdateOneAsync(updatefilter, update);
                }
                else
                {
                    var updatefilter = Builders<OAuthUserEntity>.Filter.Where(x => x.Id == id && x.Claims.Any(y => y.Type == claim.Type && y.Value == claim.Value));
                    var update = Builders<OAuthUserEntity>.Update.SetOnInsert(x => x.Claims[-1], claim);

                    var result = await _context.OAuthUserEntityCollection.UpdateOneAsync(updatefilter, update);
                }

                return true;
            }


            return false;
        }

        public async Task<bool> RemoveClaim(string userId, Claim claim)
        {
            if (!ObjectId.TryParse(userId, out ObjectId id))
            {
                return false;
            }

            var filter = Builders<OAuthUserEntity>.Filter.And(
                Builders<OAuthUserEntity>.Filter.Eq(x => x.Id, id));

            var claims = await _context.OAuthUserEntityCollection.Find(filter)
                .Project(x => x.Claims)
                .FirstOrDefaultAsync();

            if (claims != null)
            {
                if (!claims.Contains(claim))
                {
                    var updatefilter = Builders<OAuthUserEntity>.Filter.Where(x => x.Id == id);
                    var update = Builders<OAuthUserEntity>.Update.Pull(x => x.Claims, claim);

                    var result = await _context.OAuthUserEntityCollection.UpdateOneAsync(updatefilter, update);
                }

                return true;
            }

            return false;
        }

        public async Task<OAuthUserItem> FindByExternalProviderAsync(string provider, string subjectId)
        {
            var filter = Builders<OAuthUserEntity>.Filter.ElemMatch(x => x.Providers, x => x.ProviderName == provider && x.ProviderSubjectId == subjectId);
            OAuthUserItem res = await _context.OAuthUserEntityCollection.Find(filter)
                    .Project(x => new OAuthUserItem() { Id = x.Id.ToString(), Email = x.Email, Claims = x.Claims, Roles = x.Roles })
                    .FirstOrDefaultAsync();

            return res;
        }

        public async Task<bool> IsActiveAsync(string subjectId)
        {
            if (!ObjectId.TryParse(subjectId, out ObjectId id))
            {
                return false;
            }

            return await _context.OAuthUserEntityCollection.Find(x => x.Id == id)
                .Project(x => x.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task UpsertTempOAuthUserAsync(string provider, string subjectId, List<Claim> claims, string access_token, DateTime? expiresAt)
        {
            var updatefilter = Builders<TempUserClaimsEntity>.Filter.Where(x => x.ProviderSubjectKey == $"{provider}{subjectId}");
            var update = Builders<TempUserClaimsEntity>.Update
                .Set(x => x.Claims, claims)
                .Set(x => x.AccessToken, access_token);

            if (expiresAt.HasValue)
            {
                update = update.Set(x => x.ExpiresAt, expiresAt);
            }

            var result = await _context.TempUserClaimsEntityCollection.UpdateOneAsync(updatefilter, update, new UpdateOptions() { IsUpsert = true });
        }

        public async Task<List<Claim>> GetTempOAuthUserClaimsAsync(string provider, string subjectId)
        {
            return await _context.TempUserClaimsEntityCollection.Find(x => x.ProviderSubjectKey == $"{provider}{subjectId}")
                .Project(x => x.Claims)
                .FirstOrDefaultAsync();
        }

        public async Task<TempOAuthUserAccessTokenItem> GetTempOAuthUserAccessTokenAsync(string provider, string subjectId)
        {
            return await _context.TempUserClaimsEntityCollection.Find(x => x.ProviderSubjectKey == $"{provider}{subjectId}")
                .Project(x => new TempOAuthUserAccessTokenItem()
                {
                    AccessToken = x.AccessToken,
                    CreatedOn = x.ExpiresAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAccessToken(string provider, string subjectId, string accesstoken, DateTime? expiresAt)
        {
            var updatefilter = Builders<OAuthUserEntity>.Filter.ElemMatch(x => x.Providers, x => x.ProviderName == provider && x.ProviderSubjectId == subjectId);

            var update = Builders<OAuthUserEntity>.Update
                .Set(x => x.Providers.ElementAt(-1).AccessToken, accesstoken);

            if (expiresAt.HasValue)
            {
                update = update.Set(x => x.Providers.ElementAt(-1).AccessTokenExpiresAt, expiresAt);
            }

            var result = await _context.OAuthUserEntityCollection.UpdateOneAsync(updatefilter, update);
        }
    }
}
