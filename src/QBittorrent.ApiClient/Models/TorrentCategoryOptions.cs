namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents category options accepted by qBittorrent 5.3 and later.
    /// </summary>
    public record TorrentCategoryOptions
    {
        /// <summary>
        /// Gets or sets the category save path.
        /// </summary>
        public string? SavePath { get; set; }

        /// <summary>
        /// Gets or sets the category download-path behavior.
        /// </summary>
        public DownloadPathOption? DownloadPath { get; set; }

        /// <summary>
        /// Gets or sets the category share-ratio limit.
        /// </summary>
        public double? RatioLimit { get; set; }

        /// <summary>
        /// Gets or sets the category seeding-time limit in minutes.
        /// </summary>
        public int? SeedingTimeLimit { get; set; }

        /// <summary>
        /// Gets or sets the category inactive-seeding-time limit in minutes.
        /// </summary>
        public int? InactiveSeedingTimeLimit { get; set; }

        /// <summary>
        /// Gets or sets how enabled category share limits are combined.
        /// </summary>
        public ShareLimitsMode? ShareLimitsMode { get; set; }

        /// <summary>
        /// Gets or sets the action applied when the category share limits are reached.
        /// </summary>
        public ShareLimitAction? ShareLimitAction { get; set; }
    }
}
