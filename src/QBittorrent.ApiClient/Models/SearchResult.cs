using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a single search result.
    /// </summary>
    public record SearchResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResult" /> class.
        /// </summary>
        [JsonConstructor]
        public SearchResult(
            string descriptionLink,
            string fileName,
            long fileSize,
            string fileUrl,
            int leechers,
            int seeders,
            string siteUrl,
            string engineName,
            long? publishedOn)
        {
            DescriptionLink = descriptionLink;
            FileName = fileName;
            FileSize = fileSize;
            FileUrl = fileUrl;
            Leechers = leechers;
            Seeders = seeders;
            SiteUrl = siteUrl;
            EngineName = engineName;
            PublishedOn = publishedOn;
        }

        /// <summary>
        /// Gets or sets the description link.
        /// </summary>
        [JsonPropertyName("descrLink")]
        public string DescriptionLink { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        [JsonPropertyName("fileName")]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file size.
        /// </summary>
        [JsonPropertyName("fileSize")]
        public long FileSize { get; set; }

        /// <summary>
        /// Gets or sets the file URL.
        /// </summary>
        [JsonPropertyName("fileUrl")]
        public string FileUrl { get; set; }

        /// <summary>
        /// Gets or sets the leechers.
        /// </summary>
        [JsonPropertyName("nbLeechers")]
        public int Leechers { get; set; }

        /// <summary>
        /// Gets or sets the seeders.
        /// </summary>
        [JsonPropertyName("nbSeeders")]
        public int Seeders { get; set; }

        /// <summary>
        /// Gets or sets the site URL.
        /// </summary>
        [JsonPropertyName("siteUrl")]
        public string SiteUrl { get; set; }

        /// <summary>
        /// Gets or sets the engine name.
        /// </summary>
        [JsonPropertyName("engineName")]
        public string EngineName { get; set; }

        /// <summary>
        /// Gets or sets the published on.
        /// </summary>
        [JsonPropertyName("pubDate")]
        public long? PublishedOn { get; set; }
    }
}