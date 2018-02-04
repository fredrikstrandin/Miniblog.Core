using Microsoft.Extensions.Options;
using Miniblog.Core.Model.Setting;
using Miniblog.Core.Models;
using Miniblog.Core.Repository;
using Miniblog.Core.Repository.MongoDB.Model;
using Miniblog.Core.Repository.MongoDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vivus.Model;

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
