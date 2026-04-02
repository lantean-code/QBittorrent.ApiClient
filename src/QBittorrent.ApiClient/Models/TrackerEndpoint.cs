using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents statistics for a tracker endpoint.
    /// </summary>
    public record TrackerEndpoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackerEndpoint" /> class.
        /// </summary>
        /// <param name="name">The tracker endpoint name.</param>
        /// <param name="updating">Whether qBittorrent is currently updating the endpoint.</param>
        /// <param name="status">The tracker endpoint status.</param>
        /// <param name="message">The tracker status message.</param>
        /// <param name="bitTorrentVersion">The tracker protocol version reported by qBittorrent.</param>
        /// <param name="peers">The number of peers reported by the tracker.</param>
        /// <param name="seeds">The number of seeds reported by the tracker.</param>
        /// <param name="leeches">The number of leeches reported by the tracker.</param>
        /// <param name="downloads">The number of completed downloads reported by the tracker.</param>
        /// <param name="nextAnnounce">The time until the next announce in seconds when available.</param>
        /// <param name="minAnnounce">The minimum announce interval in seconds when available.</param>
        public TrackerEndpoint(
            string? name,
            bool? updating,
            TrackerStatus status,
            string? message,
            int? bitTorrentVersion,
            int? peers,
            int? seeds,
            int? leeches,
            int? downloads,
            long? nextAnnounce,
            long? minAnnounce)
        {
            Name = name;
            Updating = updating;
            Status = status;
            Message = message;
            BitTorrentVersion = bitTorrentVersion;
            Peers = peers;
            Seeds = seeds;
            Leeches = leeches;
            Downloads = downloads;
            NextAnnounce = nextAnnounce;
            MinAnnounce = minAnnounce;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; }

        /// <summary>
        /// Gets a value indicating whether the endpoint is being updated.
        /// </summary>
        [JsonPropertyName("updating")]
        public bool? Updating { get; }

        /// <summary>
        /// Gets the tracker status.
        /// </summary>
        [JsonPropertyName("status")]
        public TrackerStatus Status { get; }

        /// <summary>
        /// Gets the tracker status message.
        /// </summary>
        [JsonPropertyName("msg")]
        public string? Message { get; }

        /// <summary>
        /// Gets the BitTorrent protocol version.
        /// </summary>
        [JsonPropertyName("bt_version")]
        public int? BitTorrentVersion { get; }

        /// <summary>
        /// Gets the number of peers.
        /// </summary>
        [JsonPropertyName("num_peers")]
        public int? Peers { get; }

        /// <summary>
        /// Gets the number of seeds.
        /// </summary>
        [JsonPropertyName("num_seeds")]
        public int? Seeds { get; }

        /// <summary>
        /// Gets the number of leeches.
        /// </summary>
        [JsonPropertyName("num_leeches")]
        public int? Leeches { get; }

        /// <summary>
        /// Gets the number of completed downloads.
        /// </summary>
        [JsonPropertyName("num_downloaded")]
        public int? Downloads { get; }

        /// <summary>
        /// Gets the time until the next announce in seconds when available.
        /// </summary>
        [JsonPropertyName("next_announce")]
        public long? NextAnnounce { get; }

        /// <summary>
        /// Gets the minimum announce interval in seconds when available.
        /// </summary>
        [JsonPropertyName("min_announce")]
        public long? MinAnnounce { get; }
    }
}
