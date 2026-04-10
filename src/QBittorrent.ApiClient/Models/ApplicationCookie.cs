using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a cookie stored in qBittorrent application settings.
    /// </summary>
    public record ApplicationCookie
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationCookie" /> class.
        /// </summary>
        [JsonConstructor]
        public ApplicationCookie(string name, string? domain, string? path, string? value, long? expirationDate)
        {
            Name = name;
            Domain = domain;
            Path = path;
            Value = value;
            ExpirationDate = expirationDate;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        /// Gets the domain.
        /// </summary>
        [JsonPropertyName("domain")]
        public string? Domain { get; }

        /// <summary>
        /// Gets the path.
        /// </summary>
        [JsonPropertyName("path")]
        public string? Path { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        [JsonPropertyName("value")]
        public string? Value { get; }

        /// <summary>
        /// Gets the expiration date.
        /// </summary>
        [JsonPropertyName("expirationDate")]
        public long? ExpirationDate { get; }
    }
}
