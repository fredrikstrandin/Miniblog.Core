﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Miniblog.Core.Models;
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
                .Find(p => p.PubDate <= DateTime.UtcNow && (p.IsPublished || isAdmin))
                .Project(x => (Post)x)
                .Skip(skip)
                .Limit(count)
                .ToListAsync();

            return posts;
        }

        public async Task<IEnumerable<Post>> GetPostsByCategoryAsync(string category, bool isAdmin)
        {
            IEnumerable<Post> posts = await _context.PostEntityCollection
                .Find(p => p.PubDate <= DateTime.UtcNow && p.IsPublished &&
                    p.Categories.Contains(category))
                .Project(x => (Post)x)
                .ToListAsync();

            return posts;
        }

        public async Task<Post> GetPostBySlugAsync(string slug, bool isAdmin)
        {
            PostEntity post = await _context.PostEntityCollection
                .Find(p => p.SlugNormalize == slug && p.PubDate <= DateTime.UtcNow && p.IsPublished)
                .FirstOrDefaultAsync();

            return post;
        }

        public async Task<Post> GetPostByIdAsync(string id, bool isAdmin)
        {
            PostEntity post = null;

            if (ObjectId.TryParse(id, out ObjectId postId))
            {
                post = await _context.PostEntityCollection
                    .Find(p => p.Id == postId && p.PubDate <= DateTime.UtcNow && p.IsPublished)
                    .FirstOrDefaultAsync();
            }

            return post;
        }
        public async Task<IEnumerable<string>> GetCategoriesAsync(bool isAdmin)
        {
            var categories = await _context.CategorieEntityCollection
                    .Find(x => true)
                    .Project(x => x.Name)
                    .ToListAsync();

            return categories;
        }

        public async Task SavePostAsync(Post post)
        {
            post.LastModified = DateTime.UtcNow;

            //TODO: Check if update or Insert.

            await _context.PostEntityCollection.InsertOneAsync(post);
            foreach (var item in post.Categories)
            {
                if (await _context.CategorieEntityCollection.CountAsync(x => x.Name == item) == 0)
                {
                    await _context.CategorieEntityCollection.InsertOneAsync(new Model.CategorieEntity() { Name = item });
                }
            }
        }

        public async Task DeletePostAsync(Post post)
        {
            if (!ObjectId.TryParse(post.ID, out ObjectId id))
                return;

            await _context.PostEntityCollection.DeleteOneAsync(x => x.Id == id);
        }
        
        public async Task UpdatePostAsync(Post existing)
        {
            ObjectId postId;

            if (!ObjectId.TryParse(existing.ID, out postId))
            {
                return;
            }

            var filter = Builders<PostEntity>.Filter.And(Builders<PostEntity>.Filter.Where(x => x.Id == postId));
            var update = Builders<PostEntity>.Update
                .Set(x => x.Categories, existing.Categories)
                .Set(x => x.Content, existing.Content)
                .Set(x => x.Excerpt, existing.Excerpt)
                .Set(x => x.IsPublished, existing.IsPublished)
                .Set(x => x.LastModified, DateTime.UtcNow)
                .Set(x => x.PubDate, existing.PubDate)
                .Set(x => x.Slug, existing.Slug)
                .Set(x => x.Title, existing.Title);
            
            var post = await _context.PostEntityCollection.FindOneAndUpdateAsync(filter, update);
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