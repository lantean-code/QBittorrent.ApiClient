using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents the outcome snapshot reported by qBittorrent after an add-torrent request.
    /// </summary>
    public record AddTorrentResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddTorrentResult" /> class.
        /// </summary>
        [JsonConstructor]
        public AddTorrentResult(int successCount, int failureCount, int pendingCount, IReadOnlyList<string>? addedTorrentIds)
        {
            SuccessCount = successCount;
            FailureCount = failureCount;
            PendingCount = pendingCount;
            AddedTorrentIds = addedTorrentIds ?? [];
            SupportsAsync = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddTorrentResult" /> class.
        /// </summary>
        public AddTorrentResult(int successCount, int failureCount)
        {
            SuccessCount = successCount;
            FailureCount = failureCount;
            AddedTorrentIds = [];
            SupportsAsync = false;
        }

        /// <summary>
        /// Gets the success count.
        /// </summary>
        [JsonPropertyName("success_count")]
        public int SuccessCount { get; }

        /// <summary>
        /// Gets the number of torrents that qBittorrent reported as failed during the initial add request.
        /// </summary>
        [JsonPropertyName("failure_count")]
        public int FailureCount { get; }

        /// <summary>
        /// Gets the number of torrents that qBittorrent accepted for asynchronous processing during the initial add request.
        /// </summary>
        [JsonPropertyName("pending_count")]
        public int PendingCount { get; }

        /// <summary>
        /// Gets the IDs of torrents that qBittorrent reported as added during the initial add request.
        /// </summary>
        [JsonPropertyName("added_torrent_ids")]
        public IReadOnlyList<string> AddedTorrentIds { get; }

        /// <summary>
        /// Gets or sets a value indicating whether async is supported.
        /// </summary>
        [JsonIgnore]
        public bool SupportsAsync { get; internal set; }
    }
}
