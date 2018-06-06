using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Multiblog.Core.Model.Setting;
using Multiblog.Core.Repository;
using Multiblog.Core.Repository.MongoDB.Model;
using Multiblog.Service.Interface;
using System.Threading.Tasks;

namespace Multiblog.Repository.Database
{
    public class MongoDatabaseTool : IDatabaseToolRepository
    {
        private readonly MongoDBContext _context;
        private readonly ILogger<MongoDatabaseTool> _logger;


        public MongoDatabaseTool(ILogger<MongoDatabaseTool> logger,
            IOptions<MongoDbDatabaseSetting> _dbStetting)
        {
            _logger = logger;
            _context = new MongoDBContext(_dbStetting.Value);
        }

        public MongoDatabaseTool()
        {
        }

        public async Task DeleteBlogStore()
        {
            _logger.LogInformation($"Droping {DatabaseName.BlogEntity}");
            await _context.Database.DropCollectionAsync(DatabaseName.BlogEntity);

            _logger.LogInformation($"Droping {DatabaseName.CategoryEntity}");
            await _context.Database.DropCollectionAsync(DatabaseName.CategoryEntity);

            _logger.LogInformation($"Droping {DatabaseName.PostEntity}");
            await _context.Database.DropCollectionAsync(DatabaseName.PostEntity);
        }

        public async Task DropAllIndexAsync()
        {
            var doc = await _context.Database.ListCollectionsAsync();
            
        }

        public async Task CreateIndexAsync()
        {
            await _context.BlogEntityCollection.Indexes.CreateOneAsync(Builders<BlogEntity>.IndexKeys.Ascending(x => x.SubDomainNormalize), new CreateIndexOptions() { Name = "SubDomain" });
        }
    }
}
