using System;
using System.Collections.Generic;
using System.Text;

namespace Multiblog.Core.Model.Setting
{
    public class UrlSetting
    {
        public string OAuthServerUrl { get; set; }
        public string APIServerUrl { get; set; }
        public string ClientUrl { get; set; }      
        public int DotCountMin { get; set;  }
    }
}