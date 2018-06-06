using Microsoft.Extensions.Options;
using Multiblog.Core.Model.Setting;
using Multiblog.Core.Models;
using Multiblog.Model;
using Multiblog.Service.Interface;
using Multiblog.Utilities;
using Multiblog.Utills.Extentions;
using NLipsum.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Multiblog.Service
{
    public class TestDataService : ITestDataService
    {
        private readonly IBlogService _blogService;
        private readonly IBlogPostService _blogPostService;
        private readonly BlogSettings _blogSettings;

        public TestDataService(IBlogService blogService,
            IBlogPostService blogPostService,
            IOptions<BlogSettings> blogSettings)
        {
            _blogService = blogService;
            _blogPostService = blogPostService;
            _blogSettings = blogSettings.Value;
        }

        public async Task CreateBlogsAsync()
        {
            List<string> ids = new List<string>();

            if (_blogSettings.Multitenant)
            {
                ids.Add(await _blogService.CreateAsync(new BlogItem()
                {
                    Id = "5a766911817a741fe8178c8a",
                    Title = "Fredriks blog ",
                    Description = "This blog is for test.",
                    SubDomain = "odesus",
                    PostsPerPage = 5
                }));

                ids.Add(await _blogService.CreateAsync(new BlogItem()
                {
                    Id = "5a766911817a741fe8178c8b",
                    Title = "Lena in the garden",
                    Description = "Flower and stuff.",
                    SubDomain = "flower",
                    PostsPerPage = 3
                }));

                ids.Add(await _blogService.CreateAsync(new BlogItem()
                {
                    Id = "5a766911817a741fe8178c99",
                    Title = "Pic of the day",
                    Description = "City in pcture.",
                    SubDomain = "pictures",
                    PostsPerPage = 3
                }));
            }
            else
            {
                ids.Add(await _blogService.CreateAsync(new BlogItem()
                {
                    Id = "5a766911817a741fe8178c99",
                    Title = "Singel blog",
                    Description = "Its a sampel of a simple blog.",
                    SubDomain = "www",
                    PostsPerPage = 3
                }));
            }
            
            var generator = new LipsumGenerator(Lipsums.LoremIpsum, false);
            Random ran = new Random((int)DateTime.UtcNow.Ticks);

            var options = new Paragraph
            {
                MinimumSentences = 3,
                MaximumSentences = 20,
                SentenceOptions = new Sentence
                {
                    MinimumWords = 3,
                    MaximumWords = 20,
                    FormatString = "{0}.",
                }
            };

            string description = string.Empty;
            
            //for (int i = 0; i < 50; i++)
            //{
            //    ids.Add(await _blogService.CreateAsync(new BlogItem()
            //    {
            //        Description = generator.GenerateParagraphs(ran.Next(1, 3), options).ToLine(),
            //        Title = generator.GenerateSentences(1).ToLine(false),
            //        PostsPerPage = ran.Next(3, 15),
            //        SubDomain = i.ToString()
            //    }));
            //}

            List<Post> listPosts = new List<Post>();

            foreach (var item in ids)
            {
                int count = ran.Next(1, 15);

                for (int i = 0; i < count; i++)
                {
                    DateTime pubDate = DateTime.UtcNow.GetRandomDate(DateTime.UtcNow.AddYears(-7));
                    Status status;

                    int ranStatus = ran.Next(100);

                    if (ranStatus <= 60)
                    {
                        status = Status.Publish;
                    }
                    else if (ranStatus <= 90)
                    {
                        status = Status.Draft;
                    }
                    else
                    {
                        status = Status.Private;
                    }

                    string title = generator.GenerateSentences(1).ToLine(false);

                    await _blogPostService.SavePost(new Post()
                    {
                        Title = title,
                        BlogId = item,
                        Slug = title,
                        Categories = generator.GenerateWords(ran.Next(2, 10)).ToList(),
                        Content = generator.GenerateParagraphs(ran.Next(1, 15), options).ToLine(),
                        Excerpt = generator.GenerateParagraphs(1, options).ToLine(false),
                        PubDate = pubDate,
                        LastModified = ran.Next(0, 1) == 1 ? pubDate : DateTime.UtcNow.GetRandomDate(pubDate),
                        Status = status
                    });
                }
            }

        }        
    }
}
