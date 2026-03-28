using QBittorrent.ApiClient.Converters;
using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a torrent in qBittorrent.
    /// </summary>
    public record Torrent
    {
        /// <summary>
        /// Gets or sets the torrent hash.
        /// </summary>
        [JsonPropertyName("hash")]
        public string Hash { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the v1 info hash.
        /// </summary>
        [JsonPropertyName("infohash_v1")]
        public string? InfoHashV1 { get; init; }

        /// <summary>
        /// Gets or sets the v2 info hash.
        /// </summary>
        [JsonPropertyName("infohash_v2")]
        public string? InfoHashV2 { get; init; }

        /// <summary>
        /// Gets or sets the torrent name.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; }

        /// <summary>
        /// Gets or sets the magnet URI.
        /// </summary>
        [JsonPropertyName("magnet_uri")]
        public string? MagnetUri { get; init; }

        /// <summary>
        /// Gets or sets the torrent size in bytes.
        /// </summary>
        [JsonPropertyName("size")]
        public long? Size { get; init; }

        /// <summary>
        /// Gets or sets the torrent progress.
        /// </summary>
        [JsonPropertyName("progress")]
        public float? Progress { get; init; }

        /// <summary>
        /// Gets or sets the download speed.
        /// </summary>
        [JsonPropertyName("dlspeed")]
        public long? DownloadSpeed { get; init; }

        /// <summary>
        /// Gets or sets the upload speed.
        /// </summary>
        [JsonPropertyName("upspeed")]
        public long? UploadSpeed { get; init; }

        /// <summary>
        /// Gets or sets the torrent priority.
        /// </summary>
        [JsonPropertyName("priority")]
        public int? Priority { get; init; }

        /// <summary>
        /// Gets or sets the number of seeds.
        /// </summary>
        [JsonPropertyName("num_seeds")]
        public int? NumberSeeds { get; init; }

        /// <summary>
        /// Gets or sets the number of complete copies.
        /// </summary>
        [JsonPropertyName("num_complete")]
        public int? NumberComplete { get; init; }

        /// <summary>
        /// Gets or sets the number of leeches.
        /// </summary>
        [JsonPropertyName("num_leechs")]
        public int? NumberLeeches { get; init; }

        /// <summary>
        /// Gets or sets the number of incomplete peers.
        /// </summary>
        [JsonPropertyName("num_incomplete")]
        public int? NumberIncomplete { get; init; }

        /// <summary>
        /// Gets or sets the share ratio.
        /// </summary>
        [JsonPropertyName("ratio")]
        public float? Ratio { get; init; }

        /// <summary>
        /// Gets or sets the swarm popularity.
        /// </summary>
        [JsonPropertyName("popularity")]
        public float? Popularity { get; init; }

        /// <summary>
        /// Gets or sets the estimated time remaining.
        /// </summary>
        [JsonPropertyName("eta")]
        public long? EstimatedTimeOfArrival { get; init; }

        /// <summary>
        /// Gets or sets the torrent state.
        /// </summary>
        [JsonPropertyName("state")]
        public string? State { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether sequential download is enabled.
        /// </summary>
        [JsonPropertyName("seq_dl")]
        public bool? SequentialDownload { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether first and last piece priority is enabled.
        /// </summary>
        [JsonPropertyName("f_l_piece_prio")]
        public bool? FirstLastPiecePriority { get; init; }

        /// <summary>
        /// Gets or sets the torrent category.
        /// </summary>
        [JsonPropertyName("category")]
        public string? Category { get; init; }

        /// <summary>
        /// Gets or sets the torrent tags.
        /// </summary>
        [JsonPropertyName("tags")]
        [JsonConverter(typeof(CommaSeparatedJsonConverter))]
        public IReadOnlyList<string>? Tags { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether super seeding is enabled.
        /// </summary>
        [JsonPropertyName("super_seeding")]
        public bool? SuperSeeding { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether force start is enabled.
        /// </summary>
        [JsonPropertyName("force_start")]
        public bool? ForceStart { get; init; }

        /// <summary>
        /// Gets or sets the save path.
        /// </summary>
        [JsonPropertyName("save_path")]
        public string? SavePath { get; init; }

        /// <summary>
        /// Gets or sets the download path.
        /// </summary>
        [JsonPropertyName("download_path")]
        public string? DownloadPath { get; init; }

        /// <summary>
        /// Gets or sets the content path.
        /// </summary>
        [JsonPropertyName("content_path")]
        public string? ContentPath { get; init; }

        /// <summary>
        /// Gets or sets the root path.
        /// </summary>
        [JsonPropertyName("root_path")]
        public string? RootPath { get; init; }

        /// <summary>
        /// Gets or sets the added-on timestamp.
        /// </summary>
        [JsonPropertyName("added_on")]
        public long? AddedOn { get; init; }

        /// <summary>
        /// Gets or sets the completion timestamp.
        /// </summary>
        [JsonPropertyName("completion_on")]
        public long? CompletionOn { get; init; }

        /// <summary>
        /// Gets or sets the current tracker URL.
        /// </summary>
        [JsonPropertyName("tracker")]
        public string? Tracker { get; init; }

        /// <summary>
        /// Gets or sets the trackers count.
        /// </summary>
        [JsonPropertyName("trackers_count")]
        public int? TrackersCount { get; init; }

        /// <summary>
        /// Gets or sets the download limit.
        /// </summary>
        [JsonPropertyName("dl_limit")]
        public long? DownloadLimit { get; init; }

        /// <summary>
        /// Gets or sets the upload limit.
        /// </summary>
        [JsonPropertyName("up_limit")]
        public long? UploadLimit { get; init; }

        /// <summary>
        /// Gets or sets the downloaded byte count.
        /// </summary>
        [JsonPropertyName("downloaded")]
        public long? Downloaded { get; init; }

        /// <summary>
        /// Gets or sets the uploaded byte count.
        /// </summary>
        [JsonPropertyName("uploaded")]
        public long? Uploaded { get; init; }

        /// <summary>
        /// Gets or sets the amount downloaded in the current session.
        /// </summary>
        [JsonPropertyName("downloaded_session")]
        public long? DownloadedSession { get; init; }

        /// <summary>
        /// Gets or sets the amount uploaded in the current session.
        /// </summary>
        [JsonPropertyName("uploaded_session")]
        public long? UploadedSession { get; init; }

        /// <summary>
        /// Gets or sets the remaining byte count.
        /// </summary>
        [JsonPropertyName("amount_left")]
        public long? AmountLeft { get; init; }

        /// <summary>
        /// Gets or sets the completed byte count.
        /// </summary>
        [JsonPropertyName("completed")]
        public long? Completed { get; init; }

        /// <summary>
        /// Gets or sets the connections count.
        /// </summary>
        [JsonPropertyName("connections_count")]
        public int? ConnectionsCount { get; init; }

        /// <summary>
        /// Gets or sets the connections limit.
        /// </summary>
        [JsonPropertyName("connections_limit")]
        public int? ConnectionsLimit { get; init; }

        /// <summary>
        /// Gets or sets the max ratio.
        /// </summary>
        [JsonPropertyName("max_ratio")]
        public float? MaxRatio { get; init; }

        /// <summary>
        /// Gets or sets the max seeding time.
        /// </summary>
        [JsonPropertyName("max_seeding_time")]
        public int? MaxSeedingTime { get; init; }

        /// <summary>
        /// Gets or sets the max inactive seeding time.
        /// </summary>
        [JsonPropertyName("max_inactive_seeding_time")]
        public float? MaxInactiveSeedingTime { get; init; }

        /// <summary>
        /// Gets or sets the ratio limit.
        /// </summary>
        [JsonPropertyName("ratio_limit")]
        public float? RatioLimit { get; init; }

        /// <summary>
        /// Gets or sets the seeding time limit.
        /// </summary>
        [JsonPropertyName("seeding_time_limit")]
        public int? SeedingTimeLimit { get; init; }

        /// <summary>
        /// Gets or sets the inactive seeding time limit.
        /// </summary>
        [JsonPropertyName("inactive_seeding_time_limit")]
        public float? InactiveSeedingTimeLimit { get; init; }

        /// <summary>
        /// Gets or sets the share limit action.
        /// </summary>
        [JsonPropertyName("share_limit_action")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ShareLimitAction? ShareLimitAction { get; init; }

        /// <summary>
        /// Gets or sets the seen complete.
        /// </summary>
        [JsonPropertyName("seen_complete")]
        public long? SeenComplete { get; init; }

        /// <summary>
        /// Gets or sets the last activity.
        /// </summary>
        [JsonPropertyName("last_activity")]
        public long? LastActivity { get; init; }

        /// <summary>
        /// Gets or sets the total size in bytes.
        /// </summary>
        [JsonPropertyName("total_size")]
        public long? TotalSize { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether automatic torrent management is enabled.
        /// </summary>
        [JsonPropertyName("auto_tmm")]
        public bool? AutomaticTorrentManagement { get; init; }

        /// <summary>
        /// Gets or sets the time active.
        /// </summary>
        [JsonPropertyName("time_active")]
        public int? TimeActive { get; init; }

        /// <summary>
        /// Gets or sets the seeding time.
        /// </summary>
        [JsonPropertyName("seeding_time")]
        public long? SeedingTime { get; init; }

        /// <summary>
        /// Gets or sets the availability reported by qBittorrent.
        /// </summary>
        [JsonPropertyName("availability")]
        public float? Availability { get; init; }

        /// <summary>
        /// Gets or sets the time until the next tracker reannounce.
        /// </summary>
        [JsonPropertyName("reannounce")]
        public long? Reannounce { get; init; }

        /// <summary>
        /// Gets or sets the torrent comment.
        /// </summary>
        [JsonPropertyName("comment")]
        public string? Comment { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether torrent metadata is available.
        /// </summary>
        [JsonPropertyName("has_metadata")]
        public bool? HasMetadata { get; init; }

        /// <summary>
        /// Gets or sets the torrent creator.
        /// </summary>
        [JsonPropertyName("created_by")]
        public string? CreatedBy { get; init; }

        /// <summary>
        /// Gets or sets the torrent creation date.
        /// </summary>
        [JsonPropertyName("creation_date")]
        public long? CreationDate { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether the torrent is private.
        /// </summary>
        [JsonPropertyName("private")]
        public bool? IsPrivate { get; init; }

        /// <summary>
        /// Gets or sets the total wasted bytes.
        /// </summary>
        [JsonPropertyName("total_wasted")]
        public long? TotalWasted { get; init; }

        /// <summary>
        /// Gets or sets the number of pieces.
        /// </summary>
        [JsonPropertyName("pieces_num")]
        public int? PiecesCount { get; init; }

        /// <summary>
        /// Gets or sets the piece size in bytes.
        /// </summary>
        [JsonPropertyName("piece_size")]
        public long? PieceSize { get; init; }

        /// <summary>
        /// Gets or sets the number of pieces currently available.
        /// </summary>
        [JsonPropertyName("pieces_have")]
        public int? PiecesHave { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether the torrent has a tracker warning.
        /// </summary>
        [JsonPropertyName("has_tracker_warning")]
        public bool? HasTrackerWarning { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether the torrent has a tracker error.
        /// </summary>
        [JsonPropertyName("has_tracker_error")]
        public bool? HasTrackerError { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether the torrent has a non-tracker announce error.
        /// </summary>
        [JsonPropertyName("has_other_announce_error")]
        public bool? HasOtherAnnounceError { get; init; }

        /// <summary>
        /// Gets or sets the trackers for the torrent.
        /// </summary>
        [JsonPropertyName("trackers")]
        public IReadOnlyList<TorrentTracker>? Trackers { get; init; }
    }
}
