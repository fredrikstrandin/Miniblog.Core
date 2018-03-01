using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Multiblog.Core.Repository
{
    public interface IFileRepository
    {
        Task<string> SaveFileAsync(byte[] bytes, string fileName, string suffix);
    }
}
