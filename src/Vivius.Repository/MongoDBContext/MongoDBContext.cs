using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using Vivius.Repository.Model;
using Vivius.Model.Setting;

namespace Vivius.Repository.MongoDBContext
{
    internal class MongoDBContext
    {
        private readonly MongoDbDatabaseSetting _dbStetting;
        private readonly ILogger<MongoDBContext> _logger;

        protected IMongoClient Client;
        public IMongoDatabase Database { get; set; }

        public MongoDBContext(MongoDbDatabaseSetting dbStetting)
        {
            _dbStetting = dbStetting;

            Client = new MongoClient(_dbStetting.ConnectionString);
            Database = Client.GetDatabase(_dbStetting.Database);
        }
        
        internal IMongoCollection<UserEntity> UserEntityCollection => Database.GetCollection<UserEntity>("UserEntity");
        internal IMongoCollection<RequestChangeEmailEntity> RequestChangeEmailEntityCollection => Database.GetCollection<RequestChangeEmailEntity>("RequestChangeEmailEntity");
        internal IMongoCollection<VerifyEntity> VerifyEntityCollection => Database.GetCollection<VerifyEntity>("VerifyEntity");
    }
}
