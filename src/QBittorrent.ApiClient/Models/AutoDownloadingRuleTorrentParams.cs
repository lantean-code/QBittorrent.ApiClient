using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents torrent parameters used by RSS auto-downloading rules.
    /// </summary>
    public record AutoDownloadingRuleTorrentParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoDownloadingRuleTorrentParams" /> class.
        /// </summary>
        public AutoDownloadingRuleTorrentParams()
        {
            Category = "";
            DownloadPath = "";
            SavePath = "";
            Tags = [];
        }

        /// <summary>
        /// Gets or sets the torrent category.
        /// </summary>
        [JsonPropertyName("category")]
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the download rate limit in bytes per second.
        /// </summary>
        [JsonPropertyName("download_limit")]
        public int? DownloadLimit { get; set; }

        /// <summary>
        /// Gets or sets the download path.
        /// </summary>
        [JsonPropertyName("download_path")]
        public string DownloadPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the download path is used.
        /// </summary>
        [JsonPropertyName("use_download_path")]
        public bool? UseDownloadPath { get; set; }

        /// <summary>
        /// Gets or sets the inactive seeding time limit in minutes.
        /// </summary>
        [JsonPropertyName("inactive_seeding_time_limit")]
        public int? InactiveSeedingTimeLimit { get; set; }

        /// <summary>
        /// Gets or sets the operating mode.
        /// </summary>
        [JsonPropertyName("operating_mode")]
        public TorrentOperatingMode OperatingMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the torrent is added to the top of the queue.
        /// </summary>
        [JsonPropertyName("add_to_top_of_queue")]
        public bool? AddToTopOfQueue { get; set; }

        /// <summary>
        /// Gets or sets the share ratio limit as a unitless ratio value.
        /// </summary>
        [JsonPropertyName("ratio_limit")]
        public double? RatioLimit { get; set; }

        /// <summary>
        /// Gets or sets the action to take when the share limits are reached.
        /// </summary>
        [JsonPropertyName("share_limit_action")]
        [JsonConverter(typeof(JsonStringEnumConverter<ShareLimitAction>))]
        public ShareLimitAction? ShareLimitAction { get; set; }

        /// <summary>
        /// Gets or sets the save path.
        /// </summary>
        [JsonPropertyName("save_path")]
        public string SavePath { get; set; }

        /// <summary>
        /// Gets or sets the seeding time limit in minutes.
        /// </summary>
        [JsonPropertyName("seeding_time_limit")]
        public int? SeedingTimeLimit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether existing data is not rechecked.
        /// </summary>
        [JsonPropertyName("skip_checking")]
        public bool? SkipChecking { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the torrent is added in a stopped state.
        /// </summary>
        [JsonPropertyName("stopped")]
        public bool? Stopped { get; set; }

        /// <summary>
        /// Gets or sets the condition that stops the torrent after it is added.
        /// </summary>
        [JsonPropertyName("stop_condition")]
        public StopCondition? StopCondition { get; set; }

        /// <summary>
        /// Gets or sets the torrent tags.
        /// </summary>
        [JsonPropertyName("tags")]
        public IReadOnlyList<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the upload rate limit in bytes per second.
        /// </summary>
        [JsonPropertyName("upload_limit")]
        public int? UploadLimit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether automatic torrent management is enabled.
        /// </summary>
        [JsonPropertyName("use_auto_tmm")]
        public bool? UseAutoTmm { get; set; }

        /// <summary>
        /// Gets or sets the content layout.
        /// </summary>
        [JsonPropertyName("content_layout")]
        public TorrentContentLayout? ContentLayout { get; set; }

        /// <summary>
        /// Gets or sets the SSL certificate.
        /// </summary>
        [JsonPropertyName("ssl_certificate")]
        public string? SslCertificate { get; set; }

        /// <summary>
        /// Gets or sets the SSL private key.
        /// </summary>
        [JsonPropertyName("ssl_private_key")]
        public string? SslPrivateKey { get; set; }

        /// <summary>
        /// Gets or sets the SSL Diffie-Hellman parameters.
        /// </summary>
        [JsonPropertyName("ssl_dh_params")]
        public string? SslDhParams { get; set; }
    }
}
