using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Multiblog.Core.Model.Setting;
using Multiblog.Core.Repository;
using Multiblog.Core.Repository.MongoDB.Model;
using Multiblog.Model;
using Multiblog.Model.Blog;
using Multiblog.Service.Blog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Multiblog.Repository.Blog
{
    public class BlogRepository : IBlogRepository
    {
        private readonly MongoDBContext _context;
        
        public BlogRepository(IOptions<MongoDbDatabaseSetting> _dbStetting)
        {
            _context = new MongoDBContext(_dbStetting.Value);
        }

        public async Task AddPostUrl(string blogId, string postId, string slug)
        {
            if (ObjectId.TryParse(blogId, out ObjectId id) && ObjectId.TryParse(postId, out ObjectId postid))
            {
                var filter = Builders<BlogEntity>.Filter
                            .And(Builders<BlogEntity>.Filter.Where(x => x.Id == id));
                var update = Builders<BlogEntity>.Update.AddToSet(x => x.BlogPostinfo, new BlogPostInfo() { PostId = postid, Slug = slug, LastModified = DateTime.UtcNow });

                var temp = await _context.BlogEntityCollection.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });
            }            
        }

        public async Task<string> CreateAsync(BlogItem blogItem)
        {
            if(string.IsNullOrEmpty(blogItem.Id))
            {
                blogItem.Id = ObjectId.GenerateNewId().ToString();
            }

            await _context.BlogEntityCollection.InsertOneAsync(blogItem);
            
            return blogItem.Id;
        }

        public async Task<string> GetBlogIdAsync(string name)
        {
            return await _context.BlogEntityCollection
                .Find(x => x.SubDomainNormalize == name.Normalize())
                .Project(x => x.Id.ToString())
                .FirstOrDefaultAsync();
        }

        public async Task<string> GetSubDomainAsync(string blogId)
        {
            if (ObjectId.TryParse(blogId, out ObjectId id))
            {
                return await _context.BlogEntityCollection
                    .Find(x => x.Id == id)
                    .Project(x => x.SubDomainNormalize)
                    .FirstOrDefaultAsync();
            }
            else
            {
                return string.Empty;
            }
        }

        public async Task<IEnumerable<BlogPostInfo>> GetBlogPostsAsync(string tenant)
        {
            var ret = await _context.BlogEntityCollection
                .Find(x => x.SubDomainNormalize == tenant.Normalize())
                .Project(x => x.BlogPostinfo)
                .FirstOrDefaultAsync();

            return ret as IEnumerable<BlogPostInfo>;
        }

        public async Task<IEnumerable<string>> GetSearchableAsync()
        {
            List<string> ret =  await _context.BlogEntityCollection
                .Find(x => x.AllowSearchEngine)
                .Project(x => x.SubDomainNormalize)
                .ToListAsync();

            return ret as IEnumerable<string>;
        }
        
        public async Task<bool> IsSearchableAsync(string subDomain)
        {
            return await _context.BlogEntityCollection.CountAsync(x => x.SubDomainNormalize == subDomain.Normalize() && x.AllowSearchEngine == true) > 0;
                
        }

        public async Task<BlogItem> FindBlogAsync(string subdomain)
        {
            return await _context.BlogEntityCollection.Find(x => x.SubDomainNormalize == subdomain)
                .FirstOrDefaultAsync();
        }
    }
}
