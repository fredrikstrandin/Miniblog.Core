using Microsoft.Extensions.Options;
using Multiblog.Core.Model.Setting;
using Multiblog.Core.Models;
using Multiblog.Core.Repository;
using Multiblog.Core.Repository.MongoDB.Model;
using Multiblog.Core.Repository.MongoDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multiblog.Model;

namespace Vivius.Repository
{
    public class TestDataRepository : ITestDataRepository
    {
        private readonly MongoDBContext _context;
        
        public TestDataRepository(IOptions<MongoDbDatabaseSetting> _dbStetting)
        {
            _context = new MongoDBContext(_dbStetting.Value);
        }

        public async Task CreateBlogPost(List<Post> list)
        {
            List<PostEntity> entitys = new List<PostEntity>();

            foreach (var item in list)
            {
                entitys.Add(item);
            }

            await _context.PostEntityCollection.InsertManyAsync(entitys);
        }

        public async Task<List<string>> CreateBlogsAsync(List<BlogItem> list)
        {
            List<BlogEntity> entitys = new List<BlogEntity>();

            foreach (var item in list)
            {
                entitys.Add(item);
            }

            await _context.BlogEntityCollection.InsertManyAsync(entitys);

            return entitys.Select(x => x.Id.ToString()).ToList();
        }
    }
}
