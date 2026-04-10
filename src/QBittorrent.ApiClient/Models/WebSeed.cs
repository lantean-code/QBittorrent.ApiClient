using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a web seed URL.
    /// </summary>
    public record WebSeed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebSeed" /> class.
        /// </summary>
        [JsonConstructor]
        public WebSeed(string url)
        {
            Url = url;
        }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; }
    }
}
