using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a network interface exposed by qBittorrent.
    /// </summary>
    public record NetworkInterface
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkInterface" /> class.
        /// </summary>
        [JsonConstructor]
        public NetworkInterface(
            string name,
            string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; }
    }
}
