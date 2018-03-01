using System;
using System.Collections.Generic;
using System.Text;

namespace Multiblog.Core.Model.Utils
{
    public class ProviderItem
    {
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the provider subject identifier.
        /// </summary>
        public string ProviderSubjectId { get; set; }
        public string PublicProfileUrl { get; set; }
    }
}
