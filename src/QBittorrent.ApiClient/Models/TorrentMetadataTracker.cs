using System.Text.Json;
using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a tracker entry within torrent metadata.
    /// </summary>
    public record TorrentMetadataTracker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TorrentMetadataTracker" /> class.
        /// </summary>
        /// <param name="url">The tracker URL.</param>
        /// <param name="tier">The tracker tier.</param>
        [JsonConstructor]
        public TorrentMetadataTracker(string? url, int tier)
        {
            Url = url ?? throw new JsonException("The torrent metadata tracker payload did not include a valid url.");
            Tier = tier;
        }

        /// <summary>
        /// Gets the tracker URL.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; }

        /// <summary>
        /// Gets the tracker tier.
        /// </summary>
        [JsonPropertyName("tier")]
        public int Tier { get; }
    }
}
