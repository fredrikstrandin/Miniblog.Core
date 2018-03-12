using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Atom;
using Microsoft.SyndicationFeed.Rss;
using Multiblog.Core.Services;
using Multiblog.Service.Blog;
using Multiblog.Service.Interface;
using WebEssentials.AspNetCore.Pwa;
using Multiblog.Utilities;
using Multiblog.Model.Blog;
using System.IO;
using Multiblog.Core.Attribute;

namespace Multiblog.Core.Controllers
{
    [ServiceFilter(typeof(TenantAttribute))]
    public class RobotsController : Controller
    {
        private readonly IBlogPostService _blog;
        private readonly IBlogService _blogService;
        private readonly IOptionsSnapshot<BlogSettings> _settings;
        private readonly WebManifest _manifest;

        public RobotsController(IBlogPostService blog,
            IBlogService blogService,
            IOptionsSnapshot<BlogSettings> settings,
            WebManifest manifest)
        {
            _blog = blog;
            _blogService = blogService;
            _settings = settings;
            _manifest = manifest;
        }

        [Route("/robots.txt")]
        //[OutputCache(Profile = "default")]
        public async Task<IActionResult> RobotsTxt()
        {
            string tenant = Request.Tenant();
            
            string host = Request.Scheme + "://" + Request.Host;

            var sb = new StringBuilder();
            sb.AppendLine("User-agent: *");
            sb.AppendLine("Disallow:");
            sb.AppendLine($"sitemap: {host}/sitemap.xml");


            if (tenant == string.Empty)
            {
                foreach (var item in await _blogService.GetSearchableAsync())
                {
                    sb.AppendLine($"sitemap: {Request.Scheme}://{item}.{Request.Host}/sitemap.xml");
                }
            }

            return Ok(sb.ToString());
        }

        [Route("/sitemap.xml")]
        public async Task<IActionResult> SitemapXml()
        {
            string host = Request.Scheme + "://" + Request.Host;
            string tenant = Request.Tenant();

            if (!string.IsNullOrEmpty(tenant) && await _blogService.IsSearchableAsync(tenant))
            {
                byte[] byteXml;

                using (MemoryStream stream = new MemoryStream())
                {
                    using (var xml = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true }))
                    {
                        xml.WriteStartDocument();
                        xml.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                        foreach (BlogPostInfo item in await _blogService.GetBlogPostsAsync(tenant))
                        {
                            xml.WriteStartElement("url");
                            xml.WriteElementString("loc", $"{host}/blog/{item.Slug}");
                            xml.WriteElementString("lastmod", item.LastModified.ToString("yyyy-MM-ddThh:mmzzz"));
                            xml.WriteEndElement();
                        }

                        xml.WriteEndElement();
                    }

                    stream.Position = 0;
                    byteXml = new byte[(int)stream.Length];
                    await stream.ReadAsync(byteXml, 0, (int)stream.Length);
                }

                return File(byteXml, "application/xml");
            }

            return NoContent();
        }

        // <summary>
        // Can not be used in multi tenant env.
        // </summary>
        //[Route("/rsd.xml")]
        //public void RsdXml()
        //{
        //    string host = Request.Scheme + "://" + Request.Host;

        //    Response.ContentType = "application/xml";
        //    Response.Headers["cache-control"] = "no-cache, no-store, must-revalidate";

        //    using (var xml = XmlWriter.Create(Response.Body, new XmlWriterSettings { Indent = true }))
        //    {
        //        xml.WriteStartDocument();
        //        xml.WriteStartElement("rsd");
        //        xml.WriteAttributeString("version", "1.0");

        //        xml.WriteStartElement("service");

        //        xml.WriteElementString("enginename", "Multiblog.Core");
        //        xml.WriteElementString("enginelink", "https://github.com/fredrikstrandin/Multiblog.Core/");
        //        xml.WriteElementString("homepagelink", host);

        //        xml.WriteStartElement("apis");
        //        xml.WriteStartElement("api");
        //        xml.WriteAttributeString("name", "MetaWeblog");
        //        xml.WriteAttributeString("preferred", "true");
        //        xml.WriteAttributeString("apilink", host + "/metaweblog");
        //        xml.WriteAttributeString("blogid", "5a766911817a741fe8178c8a");
        //        xml.WriteEndElement(); // api

        //        xml.WriteEndElement(); // apis
        //        xml.WriteEndElement(); // service

        //        xml.WriteEndElement(); // rsd
        //    }
        //}

        [Route("/feed/{type}")]
        public async Task Rss(string type)
        {
            Response.ContentType = "application/xml";
            string host = Request.Scheme + "://" + Request.Host;

            using (XmlWriter xmlWriter = XmlWriter.Create(Response.Body, new XmlWriterSettings() { Async = true, Indent = true }))
            {
                var posts = await _blog.GetPostsAsync(null, 10);
                var writer = await GetWriter(type, xmlWriter, posts.Max(p => p.PubDate));

                foreach (Models.Post post in posts)
                {
                    var item = new AtomEntry
                    {
                        Title = post.Title,
                        Description = post.Content,
                        Id = host + post.GetLink(),
                        Published = post.PubDate,
                        LastUpdated = post.LastModified,
                        ContentType = "html",
                    };

                    foreach (string category in post.Categories)
                    {
                        item.AddCategory(new SyndicationCategory(category));
                    }

                    item.AddContributor(new SyndicationPerson(_settings.Value.Owner, "test@example.com"));
                    item.AddLink(new SyndicationLink(new Uri(item.Id)));

                    await writer.Write(item);
                }
            }
        }

        private async Task<ISyndicationFeedWriter> GetWriter(string type, XmlWriter xmlWriter, DateTime updated)
        {
            string host = Request.Scheme + "://" + Request.Host + "/";

            if (type.Equals("rss", StringComparison.OrdinalIgnoreCase))
            {
                var rss = new RssFeedWriter(xmlWriter);
                await rss.WriteTitle(_manifest.Name);
                await rss.WriteDescription(_manifest.Description);
                await rss.WriteGenerator("Multiblog.Core");
                await rss.WriteValue("link", host);
                return rss;
            }

            var atom = new AtomFeedWriter(xmlWriter);
            await atom.WriteTitle(_manifest.Name);
            await atom.WriteId(host);
            await atom.WriteSubtitle(_manifest.Description);
            await atom.WriteGenerator("Multiblog.Core", "https://github.com/madskristensen/Multiblog.Core", "1.0");
            await atom.WriteValue("updated", updated.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            return atom;
        }
    }
}
