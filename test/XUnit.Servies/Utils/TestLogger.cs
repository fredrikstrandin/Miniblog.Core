using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnit.Test.Utils
{
    public static class TestLogger
    {
        public static ILogger<T> Create<T>()
        {
            return new LoggerFactory().CreateLogger<T>();
        }
    }
}
