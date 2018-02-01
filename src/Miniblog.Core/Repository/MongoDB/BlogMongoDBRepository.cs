using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Miniblog.Core.Models;
using Miniblog.Core.Repository.MongoDB.Model;
using Miniblog.Core.Repository.MongoDB.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Core.Repository.MongoDB
{
    public class BlogMongoDBRepository : IBlogRepository
    {
        private readonly MongoDBContext _context;
        private readonly ILogger<BlogMongoDBRepository> _logger;

        public BlogMongoDBRepository(ILogger<BlogMongoDBRepository> logger,
            IOptions<MongoDbDatabaseSetting> _dbStetting)
        {
            _logger = logger;
            _context = new MongoDBContext(_dbStetting.Value);
        }

        public async Task<IEnumerable<Post>> GetPostsAsync(bool isAdmin, int count, int skip)
        {
            IEnumerable<Post> posts = await _context.PostEntityCollection
                .Find(p => p.PubDate <= DateTime.UtcNow && (p.Status == Status.Publish || isAdmin))
                .Project(x => (Post)x)
                .Skip(skip)
                .Limit(count)
                .ToListAsync();

            return posts;
        }

        public async Task<IEnumerable<Post>> GetPostsByCategoryAsync(string category, bool isAdmin)
        {
            IEnumerable<Post> posts = await _context.PostEntityCollection
                .Find(p => p.PubDate <= DateTime.UtcNow && p.Status == Status.Publish &&
                    p.Categories.Contains(category))
                .Project(x => (Post)x)
                .ToListAsync();

            return posts;
        }

        public async Task<Post> GetPostBySlugAsync(string slug, bool isAdmin)
        {
            PostEntity post = await _context.PostEntityCollection
                .Find(p => p.SlugNormalize == slug && p.PubDate <= DateTime.UtcNow && p.Status == Status.Publish)
                .FirstOrDefaultAsync();

            return post;
        }

        public async Task<Post> GetPostByIdAsync(string id, bool isAdmin)
        {
            PostEntity post = null;

            if (ObjectId.TryParse(id, out ObjectId postId))
            {
                post = await _context.PostEntityCollection
                    .Find(p => p.Id == postId && p.PubDate <= DateTime.UtcNow && p.Status == Status.Publish)
                    .FirstOrDefaultAsync();
            }

            return post;
        }
        
        public async Task<string> SavePostAsync(Post post)
        {
            post.LastModified = DateTime.UtcNow;

            PostEntity postEntity = post;

            await _context.PostEntityCollection.InsertOneAsync(postEntity);

            if (post.Status == Status.Publish)
            {
                foreach (var item in post.Categories)
                {
                    var filter = Builders<CategoryEntity>.Filter
                        .And(Builders<CategoryEntity>.Filter.Where(x => x.Name == item));
                    var update = Builders<CategoryEntity>.Update.Inc(x => x.Count, 1);

                    var temp = await _context.CategorieEntityCollection.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });
                }
            }

            return postEntity.Id.ToString();
        }

        public async Task DeletePostAsync(Post post)
        {
            if (!ObjectId.TryParse(post.ID, out ObjectId id))
                return;

            await _context.PostEntityCollection.DeleteOneAsync(x => x.Id == id);
        }
        
        public async Task UpdatePostAsync(Post existing)
        {
            if (!ObjectId.TryParse(existing.ID, out ObjectId postId))
            {
                return;
            }

            if (!ObjectId.TryParse(existing.BlogId, out ObjectId blogId))
            {
                return;
            }

            if (existing.Status == Status.Publish)
            {
                List<string> categoryNew = new List<string>();

                List<string> categorysOld = (await GetCategoryAsync(existing.ID)) ?? new List<string>();

                foreach (var item in existing.Categories)
                {
                    if (categorysOld.Contains(item))
                    {
                        categorysOld.Remove(item);
                    }
                    else
                    {
                        categoryNew.Add(item);
                    }
                }

                foreach (var item in categoryNew)
                {
                    await _context.CategorieEntityCollection.UpdateOneAsync(
                        Builders<CategoryEntity>.Filter
                            .And(Builders<CategoryEntity>.Filter.Where(x => x.Name == item)),
                        Builders<CategoryEntity>.Update.Inc(x => x.Count, 1),
                        new UpdateOptions() { IsUpsert = true });
                }

                foreach (var item in categorysOld)
                {
                    await _context.CategorieEntityCollection.UpdateOneAsync(
                        Builders<CategoryEntity>.Filter
                            .And(Builders<CategoryEntity>.Filter.Where(x => x.Name == item)),
                        Builders<CategoryEntity>.Update.Inc(x => x.Count, -1),
                        new UpdateOptions() { IsUpsert = true });
                }
            }

            var filter = Builders<PostEntity>.Filter.And(Builders<PostEntity>.Filter.Where(x => x.Id == postId));
            var update = Builders<PostEntity>.Update
                .Set(x => x.BlogId, blogId)
                .Set(x => x.Categories, existing.Categories)
                .Set(x => x.Content, existing.Content)
                .Set(x => x.Excerpt, existing.Excerpt)
                .Set(x => x.Status, existing.Status)
                .Set(x => x.LastModified, DateTime.UtcNow)
                .Set(x => x.PubDate, existing.PubDate)
                .Set(x => x.Slug, existing.Slug)
                .Set(x => x.Title, existing.Title);
            
            var post = await _context.PostEntityCollection.FindOneAndUpdateAsync(filter, update);
        }

        public async Task<List<string>> GetCategoryAsync(string id)
        {
            if(!ObjectId.TryParse(id, out ObjectId blogId))
            {
                return new List<string>();
            }

            var filter = Builders<BlogEntity>.Filter.ElemMatch(x => x.Categorys, x => x.Count > 0) 
                & Builders<BlogEntity>.Filter.Eq(x => x.Id, blogId);
            
            List<CategoryEntity> ret = await _context.BlogEntityCollection.Find(filter)
                .Project(x => x.Categorys)
                .FirstOrDefaultAsync();

            return ret.Select(x => x.Name).ToList();
        }

        public async Task AddCommentAsync(string id, Comment comment)
        {
            ObjectId postId;

            if (!ObjectId.TryParse(id, out postId))
                postId = ObjectId.GenerateNewId();

            CommentSubEntity com = comment;

            var filter = Builders<PostEntity>.Filter.And(Builders<PostEntity>.Filter.Where(x => x.Id == postId));
            var update = Builders<PostEntity>.Update.AddToSet(x => x.Comments, com);

            var post = await _context.PostEntityCollection.FindOneAndUpdateAsync(filter, update);
        }
    }
}
