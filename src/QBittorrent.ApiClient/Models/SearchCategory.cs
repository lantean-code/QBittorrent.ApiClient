using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a search category supported by a search plugin.
    /// </summary>
    public record SearchCategory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchCategory" /> class.
        /// </summary>
        [JsonConstructor]
        public SearchCategory(string id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the torrent name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}