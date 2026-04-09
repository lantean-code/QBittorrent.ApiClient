using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a tracker attached to a torrent.
    /// </summary>
    public record TorrentTracker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TorrentTracker" /> class.
        /// </summary>
        [JsonConstructor]
        public TorrentTracker(
            string url,
            bool? updating,
            TrackerStatus status,
            int tier,
            int peers,
            int seeds,
            int leeches,
            int downloads,
            string message,
            long? nextAnnounce,
            long? minAnnounce,
            IReadOnlyList<TrackerEndpoint>? endpoints)
        {
            Url = url;
            Updating = updating;
            Status = status;
            Tier = tier;
            Peers = peers;
            Seeds = seeds;
            Leeches = leeches;
            Downloads = downloads;
            Message = message;
            NextAnnounce = nextAnnounce;
            MinAnnounce = minAnnounce;
            Endpoints = endpoints ?? Array.Empty<TrackerEndpoint>();
        }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; }

        /// <summary>
        /// Gets a value indicating whether qBittorrent is currently updating the tracker.
        /// </summary>
        [JsonPropertyName("updating")]
        public bool? Updating { get; }

        /// <summary>
        /// Gets the tracker status.
        /// </summary>
        [JsonPropertyName("status")]
        public TrackerStatus Status { get; }

        /// <summary>
        /// Gets the tier.
        /// </summary>
        [JsonPropertyName("tier")]
        public int Tier { get; }

        /// <summary>
        /// Gets the number of peers.
        /// </summary>
        [JsonPropertyName("num_peers")]
        public int Peers { get; }

        /// <summary>
        /// Gets the number of seeds.
        /// </summary>
        [JsonPropertyName("num_seeds")]
        public int Seeds { get; }

        /// <summary>
        /// Gets the number of leeches.
        /// </summary>
        [JsonPropertyName("num_leeches")]
        public int Leeches { get; }

        /// <summary>
        /// Gets the number of completed downloads.
        /// </summary>
        [JsonPropertyName("num_downloaded")]
        public int Downloads { get; }

        /// <summary>
        /// Gets the tracker status message.
        /// </summary>
        [JsonPropertyName("msg")]
        public string Message { get; }

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

        /// <summary>
        /// Gets the endpoints.
        /// </summary>
        [JsonPropertyName("endpoints")]
        public IReadOnlyList<TrackerEndpoint> Endpoints { get; }
    }
}
