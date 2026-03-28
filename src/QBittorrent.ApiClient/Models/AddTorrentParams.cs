namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents the options used when adding torrents to qBittorrent.
    /// </summary>
    public record AddTorrentParams
    {
        /// <summary>
        /// Gets or sets the torrent source URLs.
        /// </summary>
        public IEnumerable<string>? Urls { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether existing data is not rechecked.
        /// </summary>
        public bool? SkipChecking { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether sequential download is enabled.
        /// </summary>
        public bool? SequentialDownload { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether first and last piece priority is enabled.
        /// </summary>
        public bool? FirstLastPiecePriority { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether new torrents are added to the top of the queue.
        /// </summary>
        public bool? AddToTopOfQueue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether force start is enabled.
        /// </summary>
        public bool? Forced { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the torrent is added in a stopped state.
        /// </summary>
        public bool? Stopped { get; set; }

        /// <summary>
        /// Gets or sets the save path.
        /// </summary>
        public string? SavePath { get; set; }

        /// <summary>
        /// Gets or sets the download path.
        /// </summary>
        public string? DownloadPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the download path setting is used.
        /// </summary>
        public bool? UseDownloadPath { get; set; }

        /// <summary>
        /// Gets or sets the torrent category.
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets the torrent tags.
        /// </summary>
        public IEnumerable<string>? Tags { get; set; }

        /// <summary>
        /// Gets or sets the rename torrent.
        /// </summary>
        public string? RenameTorrent { get; set; }

        /// <summary>
        /// Gets or sets the upload limit.
        /// </summary>
        public long? UploadLimit { get; set; }

        /// <summary>
        /// Gets or sets the download limit.
        /// </summary>
        public long? DownloadLimit { get; set; }

        /// <summary>
        /// Gets or sets the ratio limit.
        /// </summary>
        public float? RatioLimit { get; set; }

        /// <summary>
        /// Gets or sets the seeding time limit.
        /// </summary>
        public int? SeedingTimeLimit { get; set; }

        /// <summary>
        /// Gets or sets the inactive seeding time limit.
        /// </summary>
        public int? InactiveSeedingTimeLimit { get; set; }

        /// <summary>
        /// Gets or sets the share limit action.
        /// </summary>
        public ShareLimitAction? ShareLimitAction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether automatic torrent management is enabled.
        /// </summary>
        public bool? AutoTorrentManagement { get; set; }

        /// <summary>
        /// Gets or sets the stop condition.
        /// </summary>
        public StopCondition? StopCondition { get; set; }

        /// <summary>
        /// Gets or sets the content layout.
        /// </summary>
        public TorrentContentLayout? ContentLayout { get; set; }

        /// <summary>
        /// Gets or sets the file priorities.
        /// </summary>
        public IEnumerable<Priority>? FilePriorities { get; set; }

        /// <summary>
        /// Gets or sets the downloader.
        /// </summary>
        public string? Downloader { get; set; }

        /// <summary>
        /// Gets or sets the SSL certificate.
        /// </summary>
        public string? SslCertificate { get; set; }

        /// <summary>
        /// Gets or sets the SSL private key.
        /// </summary>
        public string? SslPrivateKey { get; set; }

        /// <summary>
        /// Gets or sets the SSL DH params.
        /// </summary>
        public string? SslDhParams { get; set; }

        /// <summary>
        /// Gets or sets the cookie.
        /// </summary>
        public string? Cookie { get; set; }

        /// <summary>
        /// Gets or sets the torrent files keyed by file name.
        /// </summary>
        public Dictionary<string, Stream>? Torrents { get; set; }
    }
}
