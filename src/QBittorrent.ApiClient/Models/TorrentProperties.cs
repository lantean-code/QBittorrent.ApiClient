using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents detailed properties of a torrent.
    /// </summary>
    public record TorrentProperties
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TorrentProperties" /> class.
        /// </summary>
        [JsonConstructor]
        public TorrentProperties(
            long additionDate,
            string comment,
            long completionDate,
            string createdBy,
            long creationDate,
            int downloadLimit,
            int downloadSpeed,
            long downloadSpeedAverage,
            long estimatedTimeOfArrival,
            long lastSeen,
            int connections,
            int connectionsLimit,
            int peers,
            int peersTotal,
            long pieceSize,
            int piecesHave,
            int piecesNum,
            long reannounce,
            string savePath,
            long seedingTime,
            int seeds,
            int seedsTotal,
            double shareRatio,
            long timeElapsed,
            long totalDownloaded,
            long totalDownloadedSession,
            long totalSize,
            long totalUploaded,
            long totalUploadedSession,
            long totalWasted,
            int uploadLimit,
            int uploadSpeed,
            long uploadSpeedAverage,
            string infoHashV1,
            string infoHashV2,
            string? hash = null,
            string? name = null,
            string? downloadPath = null,
            double? popularity = null,
            double? progress = null,
            double? availability = null,
            bool? isPrivate = null,
            bool? @private = null,
            bool? hasMetadata = null)
        {
            AdditionDate = additionDate;
            Comment = comment;
            CompletionDate = completionDate;
            CreatedBy = createdBy;
            CreationDate = creationDate;
            DownloadLimit = downloadLimit;
            DownloadSpeed = downloadSpeed;
            DownloadSpeedAverage = downloadSpeedAverage;
            EstimatedTimeOfArrival = estimatedTimeOfArrival;
            LastSeen = lastSeen;
            Connections = connections;
            ConnectionsLimit = connectionsLimit;
            Peers = peers;
            PeersTotal = peersTotal;
            PieceSize = pieceSize;
            PiecesHave = piecesHave;
            PiecesNum = piecesNum;
            Reannounce = reannounce;
            SavePath = savePath;
            SeedingTime = seedingTime;
            Seeds = seeds;
            SeedsTotal = seedsTotal;
            ShareRatio = shareRatio;
            TimeElapsed = timeElapsed;
            TotalDownloaded = totalDownloaded;
            TotalDownloadedSession = totalDownloadedSession;
            TotalSize = totalSize;
            TotalUploaded = totalUploaded;
            TotalUploadedSession = totalUploadedSession;
            TotalWasted = totalWasted;
            UploadLimit = uploadLimit;
            UploadSpeed = uploadSpeed;
            UploadSpeedAverage = uploadSpeedAverage;
            InfoHashV1 = infoHashV1;
            InfoHashV2 = infoHashV2;
            Hash = hash;
            Name = name;
            DownloadPath = downloadPath;
            Popularity = popularity;
            Progress = progress;
            Availability = availability;
            IsPrivate = isPrivate;
            Private = @private;
            HasMetadata = hasMetadata;
        }

        /// <summary>
        /// Gets the addition time as a Unix timestamp in seconds.
        /// </summary>
        [JsonPropertyName("addition_date")]
        public long AdditionDate { get; }

        /// <summary>
        /// Gets the comment.
        /// </summary>
        [JsonPropertyName("comment")]
        public string Comment { get; }

        /// <summary>
        /// Gets the completion time as a Unix timestamp in seconds.
        /// </summary>
        [JsonPropertyName("completion_date")]
        public long CompletionDate { get; }

        /// <summary>
        /// Gets the created by.
        /// </summary>
        [JsonPropertyName("created_by")]
        public string CreatedBy { get; }

        /// <summary>
        /// Gets the creation time as a Unix timestamp in seconds.
        /// </summary>
        [JsonPropertyName("creation_date")]
        public long CreationDate { get; }

        /// <summary>
        /// Gets the download rate limit in bytes per second.
        /// </summary>
        [JsonPropertyName("dl_limit")]
        public int DownloadLimit { get; }

        /// <summary>
        /// Gets the current download speed in bytes per second.
        /// </summary>
        [JsonPropertyName("dl_speed")]
        public int DownloadSpeed { get; }

        /// <summary>
        /// Gets the average download speed in bytes per second.
        /// </summary>
        [JsonPropertyName("dl_speed_avg")]
        public long DownloadSpeedAverage { get; }

        /// <summary>
        /// Gets the estimated time remaining in seconds.
        /// </summary>
        [JsonPropertyName("eta")]
        public long EstimatedTimeOfArrival { get; }

        /// <summary>
        /// Gets the last-seen time as a Unix timestamp in seconds.
        /// </summary>
        [JsonPropertyName("last_seen")]
        public long LastSeen { get; }

        /// <summary>
        /// Gets the connections.
        /// </summary>
        [JsonPropertyName("nb_connections")]
        public int Connections { get; }

        /// <summary>
        /// Gets the connections limit.
        /// </summary>
        [JsonPropertyName("nb_connections_limit")]
        public int ConnectionsLimit { get; }

        /// <summary>
        /// Gets the number of peers.
        /// </summary>
        [JsonPropertyName("peers")]
        public int Peers { get; }

        /// <summary>
        /// Gets the peers total.
        /// </summary>
        [JsonPropertyName("peers_total")]
        public int PeersTotal { get; }

        /// <summary>
        /// Gets the piece size in bytes.
        /// </summary>
        [JsonPropertyName("piece_size")]
        public long PieceSize { get; }

        /// <summary>
        /// Gets the pieces have.
        /// </summary>
        [JsonPropertyName("pieces_have")]
        public int PiecesHave { get; }

        /// <summary>
        /// Gets the pieces num.
        /// </summary>
        [JsonPropertyName("pieces_num")]
        public int PiecesNum { get; }

        /// <summary>
        /// Gets the time until the next tracker reannounce in seconds.
        /// </summary>
        [JsonPropertyName("reannounce")]
        public long Reannounce { get; }

        /// <summary>
        /// Gets the save path.
        /// </summary>
        [JsonPropertyName("save_path")]
        public string SavePath { get; }

        /// <summary>
        /// Gets the download path.
        /// </summary>
        [JsonPropertyName("download_path")]
        public string? DownloadPath { get; }

        /// <summary>
        /// Gets the seeding time in seconds.
        /// </summary>
        [JsonPropertyName("seeding_time")]
        public long SeedingTime { get; }

        /// <summary>
        /// Gets the number of seeds.
        /// </summary>
        [JsonPropertyName("seeds")]
        public int Seeds { get; }

        /// <summary>
        /// Gets the seeds total.
        /// </summary>
        [JsonPropertyName("seeds_total")]
        public int SeedsTotal { get; }

        /// <summary>
        /// Gets the share ratio as a unitless ratio value.
        /// </summary>
        [JsonPropertyName("share_ratio")]
        public double ShareRatio { get; }

        /// <summary>
        /// Gets the popularity value reported by qBittorrent in distributed copies.
        /// </summary>
        [JsonPropertyName("popularity")]
        public double? Popularity { get; }

        /// <summary>
        /// Gets the torrent completion fraction from 0.0 to 1.0.
        /// </summary>
        [JsonPropertyName("progress")]
        public double? Progress { get; }

        /// <summary>
        /// Gets the availability reported by qBittorrent in distributed copies.
        /// </summary>
        [JsonPropertyName("availability")]
        public double? Availability { get; }

        /// <summary>
        /// Gets the elapsed time in seconds.
        /// </summary>
        [JsonPropertyName("time_elapsed")]
        public long TimeElapsed { get; }

        /// <summary>
        /// Gets the total downloaded amount in bytes.
        /// </summary>
        [JsonPropertyName("total_downloaded")]
        public long TotalDownloaded { get; }

        /// <summary>
        /// Gets the total amount downloaded in the current session in bytes.
        /// </summary>
        [JsonPropertyName("total_downloaded_session")]
        public long TotalDownloadedSession { get; }

        /// <summary>
        /// Gets the total size in bytes.
        /// </summary>
        [JsonPropertyName("total_size")]
        public long TotalSize { get; }

        /// <summary>
        /// Gets the total uploaded amount in bytes.
        /// </summary>
        [JsonPropertyName("total_uploaded")]
        public long TotalUploaded { get; }

        /// <summary>
        /// Gets the total amount uploaded in the current session in bytes.
        /// </summary>
        [JsonPropertyName("total_uploaded_session")]
        public long TotalUploadedSession { get; }

        /// <summary>
        /// Gets the total wasted amount in bytes.
        /// </summary>
        [JsonPropertyName("total_wasted")]
        public long TotalWasted { get; }

        /// <summary>
        /// Gets the upload rate limit in bytes per second.
        /// </summary>
        [JsonPropertyName("up_limit")]
        public int UploadLimit { get; }

        /// <summary>
        /// Gets the current upload speed in bytes per second.
        /// </summary>
        [JsonPropertyName("up_speed")]
        public int UploadSpeed { get; }

        /// <summary>
        /// Gets the average upload speed in bytes per second.
        /// </summary>
        [JsonPropertyName("up_speed_avg")]
        public long UploadSpeedAverage { get; }

        /// <summary>
        /// Gets the v1 info hash.
        /// </summary>
        [JsonPropertyName("infohash_v1")]
        public string InfoHashV1 { get; }

        /// <summary>
        /// Gets the v2 info hash.
        /// </summary>
        [JsonPropertyName("infohash_v2")]
        public string InfoHashV2 { get; }

        /// <summary>
        /// Gets the torrent hash.
        /// </summary>
        [JsonPropertyName("hash")]
        public string? Hash { get; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; }

        /// <summary>
        /// Gets a value indicating whether the torrent is private.
        /// </summary>
        [JsonPropertyName("is_private")]
        public bool? IsPrivate { get; }

        /// <summary>
        /// Gets a value indicating whether the torrent is private.
        /// </summary>
        [JsonPropertyName("private")]
        public bool? Private { get; }

        /// <summary>
        /// Gets a value indicating whether torrent metadata is available.
        /// </summary>
        [JsonPropertyName("has_metadata")]
        public bool? HasMetadata { get; }
    }
}
