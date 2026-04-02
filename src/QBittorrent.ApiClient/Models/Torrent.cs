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
        /// Initializes a new instance of the <see cref="Torrent" /> class.
        /// </summary>
        [JsonConstructor]
        public Torrent(
            string? hash = null,
            string? infoHashV1 = null,
            string? infoHashV2 = null,
            string? name = null,
            string? magnetUri = null,
            long? size = null,
            float? progress = null,
            long? downloadSpeed = null,
            long? uploadSpeed = null,
            int? priority = null,
            int? numberSeeds = null,
            int? numberComplete = null,
            int? numberLeeches = null,
            int? numberIncomplete = null,
            float? ratio = null,
            float? popularity = null,
            long? estimatedTimeOfArrival = null,
            string? state = null,
            bool? sequentialDownload = null,
            bool? firstLastPiecePriority = null,
            string? category = null,
            IReadOnlyList<string>? tags = null,
            bool? superSeeding = null,
            bool? forceStart = null,
            string? savePath = null,
            string? downloadPath = null,
            string? contentPath = null,
            string? rootPath = null,
            long? addedOn = null,
            long? completionOn = null,
            string? tracker = null,
            int? trackersCount = null,
            long? downloadLimit = null,
            long? uploadLimit = null,
            long? downloaded = null,
            long? uploaded = null,
            long? downloadedSession = null,
            long? uploadedSession = null,
            long? amountLeft = null,
            long? completed = null,
            int? connectionsCount = null,
            int? connectionsLimit = null,
            float? maxRatio = null,
            int? maxSeedingTime = null,
            float? maxInactiveSeedingTime = null,
            float? ratioLimit = null,
            int? seedingTimeLimit = null,
            float? inactiveSeedingTimeLimit = null,
            ShareLimitAction? shareLimitAction = null,
            long? seenComplete = null,
            long? lastActivity = null,
            long? totalSize = null,
            bool? automaticTorrentManagement = null,
            int? timeActive = null,
            long? seedingTime = null,
            float? availability = null,
            long? reannounce = null,
            string? comment = null,
            bool? hasMetadata = null,
            string? createdBy = null,
            long? creationDate = null,
            bool? isPrivate = null,
            long? totalWasted = null,
            int? piecesCount = null,
            long? pieceSize = null,
            int? piecesHave = null,
            bool? hasTrackerWarning = null,
            bool? hasTrackerError = null,
            bool? hasOtherAnnounceError = null,
            IReadOnlyList<TorrentTracker>? trackers = null)
        {
            Hash = hash ?? string.Empty;
            InfoHashV1 = infoHashV1;
            InfoHashV2 = infoHashV2;
            Name = name;
            MagnetUri = magnetUri;
            Size = size;
            Progress = progress;
            DownloadSpeed = downloadSpeed;
            UploadSpeed = uploadSpeed;
            Priority = priority;
            NumberSeeds = numberSeeds;
            NumberComplete = numberComplete;
            NumberLeeches = numberLeeches;
            NumberIncomplete = numberIncomplete;
            Ratio = ratio;
            Popularity = popularity;
            EstimatedTimeOfArrival = estimatedTimeOfArrival;
            State = state;
            SequentialDownload = sequentialDownload;
            FirstLastPiecePriority = firstLastPiecePriority;
            Category = category;
            Tags = tags;
            SuperSeeding = superSeeding;
            ForceStart = forceStart;
            SavePath = savePath;
            DownloadPath = downloadPath;
            ContentPath = contentPath;
            RootPath = rootPath;
            AddedOn = addedOn;
            CompletionOn = completionOn;
            Tracker = tracker;
            TrackersCount = trackersCount;
            DownloadLimit = downloadLimit;
            UploadLimit = uploadLimit;
            Downloaded = downloaded;
            Uploaded = uploaded;
            DownloadedSession = downloadedSession;
            UploadedSession = uploadedSession;
            AmountLeft = amountLeft;
            Completed = completed;
            ConnectionsCount = connectionsCount;
            ConnectionsLimit = connectionsLimit;
            MaxRatio = maxRatio;
            MaxSeedingTime = maxSeedingTime;
            MaxInactiveSeedingTime = maxInactiveSeedingTime;
            RatioLimit = ratioLimit;
            SeedingTimeLimit = seedingTimeLimit;
            InactiveSeedingTimeLimit = inactiveSeedingTimeLimit;
            ShareLimitAction = shareLimitAction;
            SeenComplete = seenComplete;
            LastActivity = lastActivity;
            TotalSize = totalSize;
            AutomaticTorrentManagement = automaticTorrentManagement;
            TimeActive = timeActive;
            SeedingTime = seedingTime;
            Availability = availability;
            Reannounce = reannounce;
            Comment = comment;
            HasMetadata = hasMetadata;
            CreatedBy = createdBy;
            CreationDate = creationDate;
            IsPrivate = isPrivate;
            TotalWasted = totalWasted;
            PiecesCount = piecesCount;
            PieceSize = pieceSize;
            PiecesHave = piecesHave;
            HasTrackerWarning = hasTrackerWarning;
            HasTrackerError = hasTrackerError;
            HasOtherAnnounceError = hasOtherAnnounceError;
            Trackers = trackers;
        }

        /// <summary>
        /// Gets the torrent hash.
        /// </summary>
        [JsonPropertyName("hash")]
        public string Hash { get; }

        /// <summary>
        /// Gets the v1 info hash.
        /// </summary>
        [JsonPropertyName("infohash_v1")]
        public string? InfoHashV1 { get; }

        /// <summary>
        /// Gets the v2 info hash.
        /// </summary>
        [JsonPropertyName("infohash_v2")]
        public string? InfoHashV2 { get; }

        /// <summary>
        /// Gets the torrent name.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; }

        /// <summary>
        /// Gets the magnet URI.
        /// </summary>
        [JsonPropertyName("magnet_uri")]
        public string? MagnetUri { get; }

        /// <summary>
        /// Gets the torrent size in bytes.
        /// </summary>
        [JsonPropertyName("size")]
        public long? Size { get; }

        /// <summary>
        /// Gets the torrent progress.
        /// </summary>
        [JsonPropertyName("progress")]
        public float? Progress { get; }

        /// <summary>
        /// Gets the download speed.
        /// </summary>
        [JsonPropertyName("dlspeed")]
        public long? DownloadSpeed { get; }

        /// <summary>
        /// Gets the upload speed.
        /// </summary>
        [JsonPropertyName("upspeed")]
        public long? UploadSpeed { get; }

        /// <summary>
        /// Gets the torrent priority.
        /// </summary>
        [JsonPropertyName("priority")]
        public int? Priority { get; }

        /// <summary>
        /// Gets the number of seeds.
        /// </summary>
        [JsonPropertyName("num_seeds")]
        public int? NumberSeeds { get; }

        /// <summary>
        /// Gets the number of complete copies.
        /// </summary>
        [JsonPropertyName("num_complete")]
        public int? NumberComplete { get; }

        /// <summary>
        /// Gets the number of leeches.
        /// </summary>
        [JsonPropertyName("num_leechs")]
        public int? NumberLeeches { get; }

        /// <summary>
        /// Gets the number of incomplete peers.
        /// </summary>
        [JsonPropertyName("num_incomplete")]
        public int? NumberIncomplete { get; }

        /// <summary>
        /// Gets the share ratio.
        /// </summary>
        [JsonPropertyName("ratio")]
        public float? Ratio { get; }

        /// <summary>
        /// Gets the swarm popularity.
        /// </summary>
        [JsonPropertyName("popularity")]
        public float? Popularity { get; }

        /// <summary>
        /// Gets the estimated time remaining.
        /// </summary>
        [JsonPropertyName("eta")]
        public long? EstimatedTimeOfArrival { get; }

        /// <summary>
        /// Gets the torrent state.
        /// </summary>
        [JsonPropertyName("state")]
        public string? State { get; }

        /// <summary>
        /// Gets a value indicating whether sequential download is enabled.
        /// </summary>
        [JsonPropertyName("seq_dl")]
        public bool? SequentialDownload { get; }

        /// <summary>
        /// Gets a value indicating whether first and last piece priority is enabled.
        /// </summary>
        [JsonPropertyName("f_l_piece_prio")]
        public bool? FirstLastPiecePriority { get; }

        /// <summary>
        /// Gets the torrent category.
        /// </summary>
        [JsonPropertyName("category")]
        public string? Category { get; }

        /// <summary>
        /// Gets the torrent tags.
        /// </summary>
        [JsonPropertyName("tags")]
        [JsonConverter(typeof(CommaSeparatedJsonConverter))]
        public IReadOnlyList<string>? Tags { get; }

        /// <summary>
        /// Gets a value indicating whether super seeding is enabled.
        /// </summary>
        [JsonPropertyName("super_seeding")]
        public bool? SuperSeeding { get; }

        /// <summary>
        /// Gets a value indicating whether force start is enabled.
        /// </summary>
        [JsonPropertyName("force_start")]
        public bool? ForceStart { get; }

        /// <summary>
        /// Gets the save path.
        /// </summary>
        [JsonPropertyName("save_path")]
        public string? SavePath { get; }

        /// <summary>
        /// Gets the download path.
        /// </summary>
        [JsonPropertyName("download_path")]
        public string? DownloadPath { get; }

        /// <summary>
        /// Gets the content path.
        /// </summary>
        [JsonPropertyName("content_path")]
        public string? ContentPath { get; }

        /// <summary>
        /// Gets the root path.
        /// </summary>
        [JsonPropertyName("root_path")]
        public string? RootPath { get; }

        /// <summary>
        /// Gets the added-on timestamp.
        /// </summary>
        [JsonPropertyName("added_on")]
        public long? AddedOn { get; }

        /// <summary>
        /// Gets the completion timestamp.
        /// </summary>
        [JsonPropertyName("completion_on")]
        public long? CompletionOn { get; }

        /// <summary>
        /// Gets the current tracker URL.
        /// </summary>
        [JsonPropertyName("tracker")]
        public string? Tracker { get; }

        /// <summary>
        /// Gets the trackers count.
        /// </summary>
        [JsonPropertyName("trackers_count")]
        public int? TrackersCount { get; }

        /// <summary>
        /// Gets the download limit.
        /// </summary>
        [JsonPropertyName("dl_limit")]
        public long? DownloadLimit { get; }

        /// <summary>
        /// Gets the upload limit.
        /// </summary>
        [JsonPropertyName("up_limit")]
        public long? UploadLimit { get; }

        /// <summary>
        /// Gets the downloaded byte count.
        /// </summary>
        [JsonPropertyName("downloaded")]
        public long? Downloaded { get; }

        /// <summary>
        /// Gets the uploaded byte count.
        /// </summary>
        [JsonPropertyName("uploaded")]
        public long? Uploaded { get; }

        /// <summary>
        /// Gets the amount downloaded in the current session.
        /// </summary>
        [JsonPropertyName("downloaded_session")]
        public long? DownloadedSession { get; }

        /// <summary>
        /// Gets the amount uploaded in the current session.
        /// </summary>
        [JsonPropertyName("uploaded_session")]
        public long? UploadedSession { get; }

        /// <summary>
        /// Gets the remaining byte count.
        /// </summary>
        [JsonPropertyName("amount_left")]
        public long? AmountLeft { get; }

        /// <summary>
        /// Gets the completed byte count.
        /// </summary>
        [JsonPropertyName("completed")]
        public long? Completed { get; }

        /// <summary>
        /// Gets the connections count.
        /// </summary>
        [JsonPropertyName("connections_count")]
        public int? ConnectionsCount { get; }

        /// <summary>
        /// Gets the connections limit.
        /// </summary>
        [JsonPropertyName("connections_limit")]
        public int? ConnectionsLimit { get; }

        /// <summary>
        /// Gets the max ratio.
        /// </summary>
        [JsonPropertyName("max_ratio")]
        public float? MaxRatio { get; }

        /// <summary>
        /// Gets the max seeding time.
        /// </summary>
        [JsonPropertyName("max_seeding_time")]
        public int? MaxSeedingTime { get; }

        /// <summary>
        /// Gets the max inactive seeding time.
        /// </summary>
        [JsonPropertyName("max_inactive_seeding_time")]
        public float? MaxInactiveSeedingTime { get; }

        /// <summary>
        /// Gets the ratio limit.
        /// </summary>
        [JsonPropertyName("ratio_limit")]
        public float? RatioLimit { get; }

        /// <summary>
        /// Gets the seeding time limit.
        /// </summary>
        [JsonPropertyName("seeding_time_limit")]
        public int? SeedingTimeLimit { get; }

        /// <summary>
        /// Gets the inactive seeding time limit.
        /// </summary>
        [JsonPropertyName("inactive_seeding_time_limit")]
        public float? InactiveSeedingTimeLimit { get; }

        /// <summary>
        /// Gets the share limit action.
        /// </summary>
        [JsonPropertyName("share_limit_action")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ShareLimitAction? ShareLimitAction { get; }

        /// <summary>
        /// Gets the seen complete.
        /// </summary>
        [JsonPropertyName("seen_complete")]
        public long? SeenComplete { get; }

        /// <summary>
        /// Gets the last activity.
        /// </summary>
        [JsonPropertyName("last_activity")]
        public long? LastActivity { get; }

        /// <summary>
        /// Gets the total size in bytes.
        /// </summary>
        [JsonPropertyName("total_size")]
        public long? TotalSize { get; }

        /// <summary>
        /// Gets a value indicating whether automatic torrent management is enabled.
        /// </summary>
        [JsonPropertyName("auto_tmm")]
        public bool? AutomaticTorrentManagement { get; }

        /// <summary>
        /// Gets the time active.
        /// </summary>
        [JsonPropertyName("time_active")]
        public int? TimeActive { get; }

        /// <summary>
        /// Gets the seeding time.
        /// </summary>
        [JsonPropertyName("seeding_time")]
        public long? SeedingTime { get; }

        /// <summary>
        /// Gets the availability reported by qBittorrent.
        /// </summary>
        [JsonPropertyName("availability")]
        public float? Availability { get; }

        /// <summary>
        /// Gets the time until the next tracker reannounce.
        /// </summary>
        [JsonPropertyName("reannounce")]
        public long? Reannounce { get; }

        /// <summary>
        /// Gets the torrent comment.
        /// </summary>
        [JsonPropertyName("comment")]
        public string? Comment { get; }

        /// <summary>
        /// Gets a value indicating whether torrent metadata is available.
        /// </summary>
        [JsonPropertyName("has_metadata")]
        public bool? HasMetadata { get; }

        /// <summary>
        /// Gets the torrent creator.
        /// </summary>
        [JsonPropertyName("created_by")]
        public string? CreatedBy { get; }

        /// <summary>
        /// Gets the torrent creation date.
        /// </summary>
        [JsonPropertyName("creation_date")]
        public long? CreationDate { get; }

        /// <summary>
        /// Gets a value indicating whether the torrent is private.
        /// </summary>
        [JsonPropertyName("private")]
        public bool? IsPrivate { get; }

        /// <summary>
        /// Gets the total wasted bytes.
        /// </summary>
        [JsonPropertyName("total_wasted")]
        public long? TotalWasted { get; }

        /// <summary>
        /// Gets the number of pieces.
        /// </summary>
        [JsonPropertyName("pieces_num")]
        public int? PiecesCount { get; }

        /// <summary>
        /// Gets the piece size in bytes.
        /// </summary>
        [JsonPropertyName("piece_size")]
        public long? PieceSize { get; }

        /// <summary>
        /// Gets the number of pieces currently available.
        /// </summary>
        [JsonPropertyName("pieces_have")]
        public int? PiecesHave { get; }

        /// <summary>
        /// Gets a value indicating whether the torrent has a tracker warning.
        /// </summary>
        [JsonPropertyName("has_tracker_warning")]
        public bool? HasTrackerWarning { get; }

        /// <summary>
        /// Gets a value indicating whether the torrent has a tracker error.
        /// </summary>
        [JsonPropertyName("has_tracker_error")]
        public bool? HasTrackerError { get; }

        /// <summary>
        /// Gets a value indicating whether the torrent has a non-tracker announce error.
        /// </summary>
        [JsonPropertyName("has_other_announce_error")]
        public bool? HasOtherAnnounceError { get; }

        /// <summary>
        /// Gets the trackers for the torrent.
        /// </summary>
        [JsonPropertyName("trackers")]
        public IReadOnlyList<TorrentTracker>? Trackers { get; }
    }
}
