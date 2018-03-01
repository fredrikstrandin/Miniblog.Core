using Multiblog.Core.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Multiblog.Model;
using NLipsum.Core;
using Multiblog.Utilities;
using Multiblog.Core.Models;
using System.Linq;
using Multiblog.Utills.Extentions;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Multiblog.Service.UserService;
using IdentityServer4.MongoDB.Service;

namespace Multiblog.Service
{
    public class TestDataService : ITestDataService
    {
        private readonly ITestDataRepository _testDataRepository;

        public TestDataService(ITestDataRepository testDataRepository)
        {
            _testDataRepository = testDataRepository;
        }

        public async Task CreateBlogsAsync()
        {
            List<BlogItem> list = new List<BlogItem>()
            {
                new BlogItem()
                {
                    Title = "Fredriks blog ",
                    Description = "Why not.",
                    SubDomain = "fredrik",
                    PostsPerPage = 5
                },
                new BlogItem()
                {
                    Title = "Lena in the garden",
                    Description = "Flower and stuff.",
                    SubDomain = "flower",
                    PostsPerPage = 3
                }
            };

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

            for (int i = 0; i < 5000; i++)
            {
                list.Add(new BlogItem()
                {
                    Description = generator.GenerateParagraphs(ran.Next(1, 3), options).ToLine(),
                    Title = generator.GenerateSentences(1).ToLine(false),
                    PostsPerPage = ran.Next(3, 15),
                    SubDomain = i.ToString()
                });
            }

            List<string> ids = await _testDataRepository.CreateBlogsAsync(list);

            List<Post> listPosts = new List<Post>();

            foreach (var item in ids)
            {
                int count = ran.Next(1, 70);

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

                    listPosts.Add(new Post()
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

                await _testDataRepository.CreateBlogPost(listPosts);
                listPosts.Clear();
            }

        }

        /// <summary>
        /// Calculates the lenght in bytes of an object 
        /// and returns the size 
        /// </summary>
        /// <param name="TestObject"></param>
        /// <returns></returns>
        private int GetObjectSize(object TestObject)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            byte[] Array;
            bf.Serialize(ms, TestObject);
            Array = ms.ToArray();
            return Array.Length;
        }
        private async Task CreateBlogPost()
        {
            var generator = new LipsumGenerator(Lipsums.LoremIpsum, false);

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
            int paragraphCount = 12;

            var paragraphs = generator.GenerateParagraphs(paragraphCount, options);
        }
    }
}
