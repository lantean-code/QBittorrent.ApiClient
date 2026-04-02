using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a qBittorrent Web API key payload.
    /// </summary>
    public record ApiKey
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKey" /> class.
        /// </summary>
        /// <param name="key">The Web API key.</param>
        [JsonConstructor]
        public ApiKey(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Gets the Web API key.
        /// </summary>
        [JsonPropertyName("apiKey")]
        public string Key { get; }
    }
}
