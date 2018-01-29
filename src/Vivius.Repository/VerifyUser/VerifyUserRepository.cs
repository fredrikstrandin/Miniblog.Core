using MongoDB.Bson;
using MongoDB.Driver;
using Vivius.Repository.MongoDBContext;
using Vivius.Repositories.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vivius.Model.Setting;
using Microsoft.Extensions.Options;
using Vivius.Repository.Model;

namespace Vivius.Repository.VerifyUser
{
    public class VerifyUserRepository : IVerifyUserRepository
    {
        private readonly MongoDBContext.MongoDBContext _context;

        public VerifyUserRepository(IOptions<MongoDbDatabaseSetting> dbStetting)
        {
            _context = new MongoDBContext.MongoDBContext(dbStetting.Value);
        }

        public async Task<string> CreateCodeAsync(string userId)
        {
            if(!ObjectId.TryParse(userId, out ObjectId id))
            {
                return string.Empty;
            }

            var ret = new VerifyEntity()
            {
                UserId = id,
                Expired = DateTime.UtcNow.AddDays(1)
            };

            await _context.VerifyEntityCollection.InsertOneAsync(ret);

            return ret.VerifyCode.ToString();
        }
        public async Task<bool> VerifyCodeAsync(string code, string userId)
        {
            if (!ObjectId.TryParse(code, out ObjectId codeid))
            {
                return false;
            }

            if(!ObjectId.TryParse(userId, out ObjectId userid))
            {
                return false;
            }

            var ret = await _context.VerifyEntityCollection.CountAsync(x => x.VerifyCode == codeid && x.UserId == userid && x.Expired > DateTime.UtcNow);

            return ret > 0;
        }

        public async Task<string> GetUserFromVerifyCodeAsync(string code)
        {
            if (!ObjectId.TryParse(code, out ObjectId id))
            {
                return null;
            }
            
            var ret = await _context.VerifyEntityCollection.Find(x => x.VerifyCode == id).FirstOrDefaultAsync();

            if (ret != null && ret.Expired > DateTime.UtcNow)
            {
                return ret.UserId.ToString();
            }

            return null;
        }

        public async Task DeleteVerifyCodeAsync(string code)
        {
            if (!ObjectId.TryParse(code, out ObjectId id))
            {
                return;
            }

            var ret = await _context.VerifyEntityCollection.DeleteOneAsync(x => x.VerifyCode == id);            
        }
    }
}
