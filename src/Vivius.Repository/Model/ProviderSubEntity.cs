using Miniblog.Core.Model.Utils;

namespace Miniblog.Core.Repository.Model
{
    internal class ProviderSubEntity
    {
        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the provider subject identifier.
        /// </summary>
        public string ProviderSubjectId { get; set; }

        public string PublicProfileUrl { get; set; }

        public static implicit operator ProviderItem(ProviderSubEntity item)
        {
            return new ProviderItem()
            {
                ProviderSubjectId = item.ProviderSubjectId,
                ProviderName = item.ProviderName,
                PublicProfileUrl = item.PublicProfileUrl
            };
        }

        public static implicit operator ProviderSubEntity(ProviderItem item)
        {
            return new ProviderSubEntity()
            {
                ProviderSubjectId = item.ProviderSubjectId,
                ProviderName = item.ProviderName,
                PublicProfileUrl = item.PublicProfileUrl
            };
        }
    }
}
