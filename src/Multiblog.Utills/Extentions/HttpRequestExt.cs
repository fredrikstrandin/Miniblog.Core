using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Multiblog.Utilities
{
    public static class HttpRequestExt
    {
        /// <summary>
        /// return tenant
        /// </summary>
        public static string Tenant(this HttpRequest request)
        {
            if(request.Host.HasValue)
            {
                string[] host = request.Host.Host.Split('.', StringSplitOptions.RemoveEmptyEntries);
                if(host.Length > 1)
                {
                    return host[0];
                }
            }

            return string.Empty;
        }        
    }
}
