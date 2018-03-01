using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Multiblog.Core.Repository
{
    public class FileDiskRepository : IFileRepository
    {
        private readonly string _folder;

        public FileDiskRepository(IHostingEnvironment env, 
            ILogger<FileDiskRepository> logger)
        {
            var mountedPath = "/app/data";
            if (Directory.Exists(mountedPath))
            {
                logger.LogInformation("Using mounted path '/app/data' for data");
                _folder = Path.Combine(mountedPath, "Posts");
            }
            else
            {
                logger.LogInformation("Using 'wwwroot/Posts' for data");

                _folder = Path.Combine(env.WebRootPath, "Posts");
            }

        }
        public async Task<string> SaveFileAsync(byte[] bytes, string fileName, string suffix)
        {
            suffix = suffix ?? DateTime.UtcNow.Ticks.ToString();

            string ext = Path.GetExtension(fileName);
            string name = Path.GetFileNameWithoutExtension(fileName);

            string relative = $"files/{name}_{suffix}{ext}";
            string absolute = Path.Combine(_folder, relative);
            string dir = Path.GetDirectoryName(absolute);

            Directory.CreateDirectory(dir);
            using (var writer = new FileStream(absolute, FileMode.CreateNew))
            {
                await writer.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
            }

            return "/Posts/" + relative;
        }
    }
}
