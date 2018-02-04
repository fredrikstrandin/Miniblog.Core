using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Miniblog.Core.Repository.Model;
using Miniblog.Core.Repository;
using Miniblog.Core.Model.User;
using Miniblog.Core.Model.Setting;
using Miniblog.Core.Interface.Repositories;

namespace Miniblog.Core.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly MongoDBContext _context;
        private readonly ILogger<UserRepository> _logger;


        private readonly TelemetryClient _telemetry = new TelemetryClient();

        public UserRepository(ILogger<UserRepository> logger,
            IOptions<MongoDbDatabaseSetting> _dbStetting)
        {
            _logger = logger;

            _context = new MongoDBContext(_dbStetting.Value);
        }

        public async Task<string> CreateAsync(UserItem user)
        {
            UserEntity entity = user;

            if (entity != null && (await _context.UserEntityCollection.CountAsync(x => x.Email == entity.Email)) == 0)
            {
                await _context.UserEntityCollection.InsertOneAsync(entity);

                return entity.Id.ToString();
            }

            return null;
        }

        public async Task<string> GetEmailAsync(string userId)
        {
            if (!ObjectId.TryParse(userId, out ObjectId objectId))
            {
                throw new FormatException("UserId did not have excepted format.");
            }

            return await _context.UserEntityCollection
                .Find(u => u.Id == objectId)
                .Project(x => x.Email)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CheckIfExistAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("Email cant be null or empty.");
            }

            email = email.ToLower().Trim();

            return await _context.UserEntityCollection.CountAsync(u => u.Email == email) > 0;
        }

        public async Task<bool> CheckIfEmailVerifyAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("Email cant be null or empty.");
            }

            email = email.ToLower().Trim();

            return await _context.UserEntityCollection.Find(u => u.Email == email)
                .Project(x => x.EmailVerified)
                .FirstOrDefaultAsync();
        }

        public async Task<string> GetIdByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("Email cant be null or empty.");
            }

            var ret = await _context.UserEntityCollection.Find(u => u.Email == email.ToLower())
                .Project(x => x.Id)
                .FirstOrDefaultAsync();

            if (default(ObjectId) == ret)
            {
                return null;
            }

            return ret.ToString();

        }

        public async Task<bool> SetFavoriteAsync(string userId, string refId)
        {
            if (!ObjectId.TryParse(userId, out ObjectId Id))
            {
                throw new FormatException("UserId did not have excepted format.");
            }

            if (!ObjectId.TryParse(refId, out ObjectId RefId))
            {
                throw new FormatException("RefId did not have excepted format.");
            }

            var updatefilter = Builders<UserEntity>.Filter.Where(x => x.Id == Id);
            var update = Builders<UserEntity>.Update.AddToSet(x => x.Favorite, RefId);
            var result = await _context.UserEntityCollection.UpdateOneAsync(updatefilter, update);

            return result.MatchedCount > 0;
        }

        public async Task<bool> RemoveFavoriteAsync(string userId, string refId)
        {
            if (!ObjectId.TryParse(userId, out ObjectId Id))
            {
                throw new FormatException("UserId did not have excepted format.");
            }

            if (!ObjectId.TryParse(refId, out ObjectId RefId))
            {
                throw new FormatException("RefId did not have excepted format.");
            }

            var result = await _context.UserEntityCollection.UpdateOneAsync(
                Builders<UserEntity>.Filter.Eq(x => x.Id, Id),
                Builders<UserEntity>.Update.Pull(x => x.Favorite, RefId));

            return result.MatchedCount > 0;
        }

        public async Task<UserProfileItem> GetProfileAsync(string userId)
        {
            if (!ObjectId.TryParse(userId, out ObjectId objectId))
            {
                throw new FormatException("UserId did not have excepted format.");
            }

            var item = await _context.UserEntityCollection.Find(u => u.Id == objectId)
                .Project(x => new UserProfileItem()
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    DateOfBirth = x.DateOfBirth,
                    City = x.City,
                    ProfileImageUrl = x.ProfileImageUrl
                })
                .FirstOrDefaultAsync();

            return item;
        }

        public async Task<bool> UpdateProfileAsync(string userId, UserProfileItem item)
        {
            if (!ObjectId.TryParse(userId, out ObjectId Id))
            {
                throw new FormatException("UserId did not have excepted format.");
            }

            var filter = Builders<UserEntity>.Filter.Eq(x => x.Id, Id);

            var update = Builders<UserEntity>.Update
                .Set(x => x.Version, DateTime.UtcNow.Ticks);

            if (!string.IsNullOrEmpty(item.FirstName))
            {
                update = update.Set(x => x.FirstName, item.FirstName);
            }
            if (!string.IsNullOrEmpty(item.LastName))
            {
                update = update.Set(x => x.LastName, item.LastName);
            }

            if (item.DateOfBirth.HasValue)
            {
                update = update.Set(x => x.DateOfBirth, new DateTime(item.DateOfBirth.Value.Year, item.DateOfBirth.Value.Month, item.DateOfBirth.Value.Day, 0, 0, 0, DateTimeKind.Utc));
            }

            if (!string.IsNullOrEmpty(item.City))
            {
                update = update.Set(x => x.City, item.City);
            }
            if (!string.IsNullOrEmpty(item.ProfileImageUrl))
            {
                update = update.Set(x => x.ProfileImageUrl, item.ProfileImageUrl);
            }

            var ret = await _context.UserEntityCollection.UpdateOneAsync(filter, update);

            return ret.MatchedCount > 0;
        }

        public async Task<bool> DeleteAccountAsync(string userId)
        {
            if (!ObjectId.TryParse(userId, out ObjectId id))
            {
                throw new FormatException("UserId did not have excepted format.");
            }

            var ret = await _context.UserEntityCollection.DeleteOneAsync(x => x.Id == id);
            return ret.DeletedCount > 0;
        }

        public async Task RequestEmailChangeAsync(string newEmail, string oldEmail, string code)
        {
            await _context.RequestChangeEmailEntityCollection.InsertOneAsync(new RequestChangeEmailEntity()
            {
                NewEmail = newEmail,
                OldEmail = oldEmail,
                Code = code
            });
        }

        public async Task<string[]> CheckEmailChangeAsync(string code)
        {
            return await _context.RequestChangeEmailEntityCollection.Find(x => x.Code == code)
                .Project(x => new string[2] { x.NewEmail, x.OldEmail })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CommitEmailChange(string NewEmail, string OldEmail)
        {
            var filter = Builders<UserEntity>.Filter.And(Builders<UserEntity>.Filter.Where(x => x.Email == OldEmail));
            var update = Builders<UserEntity>.Update
                .Set(x => x.Email, NewEmail);

            var user = await _context.UserEntityCollection.UpdateOneAsync(filter, update);

            return user.MatchedCount > 0;
        }

        public async Task<bool> UpdateProvidersAsync(string email, string provider, string providerSubjectId, string publicProfileUrl)
        {
            var filter = Builders<UserEntity>.Filter.And(Builders<UserEntity>.Filter.Where(x => x.Email == email));
            var update = Builders<UserEntity>.Update.Push(x => x.Providers,
                new ProviderSubEntity()
                {
                    ProviderName = provider,
                    ProviderSubjectId = providerSubjectId,
                    PublicProfileUrl = publicProfileUrl
                });

            var result = await _context.UserEntityCollection.UpdateOneAsync(filter, update);

            return result.MatchedCount > 0;
        }

        public async Task<UserEmailItem> FindByExternalProviderAsync(string provider, string ProviderSubjectId)
        {
            try
            {
                var filter = Builders<UserEntity>.Filter.ElemMatch(x => x.Providers, x => x.ProviderName == provider && x.ProviderSubjectId == ProviderSubjectId);
                var res = await _context.UserEntityCollection.Find(filter)
                    .Project(x => new UserEmailItem() { Id = x.Id.ToString(), Email = x.Email, EmailVerified = x.EmailVerified })
                    .FirstOrDefaultAsync();

                return res;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                throw;
            }
        }

        public async Task<bool> SetVerifyEmailAsync(string userId)
        {
            if (!ObjectId.TryParse(userId, out ObjectId id))
            {
                return false;
            }

            var filter = Builders<UserEntity>.Filter.Where(x => x.Id == id);
            var update = Builders<UserEntity>.Update.Set(x => x.EmailVerified, true);

            var ret = await _context.UserEntityCollection.UpdateOneAsync(filter, update);

            return ret.MatchedCount > 0;
        }

        public Task<string> UpdateProvidersAsync(string email, string provider, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateLastLoginAsync(string userId)
        {
            if (!ObjectId.TryParse(userId, out ObjectId id))
            {
                return;
            }

            var filter = Builders<UserEntity>.Filter.Where(x => x.Id == id);
            var update = Builders<UserEntity>.Update.Set(x => x.LastLoginTime, DateTime.UtcNow);

            var ret = await _context.UserEntityCollection.UpdateOneAsync(filter, update);

        }
    }
}

