using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnit.Multiblog.Utils
{
    public static class TestLogger
    {
        public static ILogger<T> Create<T>()
        {
            return new LoggerFactory().CreateLogger<T>();
        }
    }
}
