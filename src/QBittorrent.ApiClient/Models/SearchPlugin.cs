using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents an installed search plugin.
    /// </summary>
    public record SearchPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchPlugin" /> class.
        /// </summary>
        [JsonConstructor]
        public SearchPlugin(
            bool enabled,
            string fullName,
            string name,
            IReadOnlyList<SearchCategory> supportedCategories,
            string url,
            string version)
        {
            Enabled = enabled;
            FullName = fullName;
            Name = name;
            SupportedCategories = supportedCategories;
            Url = url;
            Version = version;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is enabled.
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the torrent name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the supported categories.
        /// </summary>
        [JsonPropertyName("supportedCategories")]
        public IReadOnlyList<SearchCategory> SupportedCategories { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }
    }
}