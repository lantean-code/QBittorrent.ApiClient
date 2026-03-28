using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents shared torrent parameters used by qBittorrent features.
    /// </summary>
    public record TorrentParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TorrentParams" /> class.
        /// </summary>
        public TorrentParams()
        {
            Category = "";
            DownloadPath = "";
            OperatingMode = "";
            SavePath = "";
            Tags = [];
        }

        /// <summary>
        /// Gets or sets the torrent category.
        /// </summary>
        [JsonPropertyName("category")]
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the download limit.
        /// </summary>
        [JsonPropertyName("download_limit")]
        public int? DownloadLimit { get; set; }

        /// <summary>
        /// Gets or sets the download path.
        /// </summary>
        [JsonPropertyName("download_path")]
        public string DownloadPath { get; set; }

        /// <summary>
        /// Gets or sets the inactive seeding time limit.
        /// </summary>
        [JsonPropertyName("inactive_seeding_time_limit")]
        public int? InactiveSeedingTimeLimit { get; set; }

        /// <summary>
        /// Gets or sets the operating mode.
        /// </summary>
        [JsonPropertyName("operating_mode")]
        public string OperatingMode { get; set; }

        /// <summary>
        /// Gets or sets the ratio limit.
        /// </summary>
        [JsonPropertyName("ratio_limit")]
        public int? RatioLimit { get; set; }

        /// <summary>
        /// Gets or sets the save path.
        /// </summary>
        [JsonPropertyName("save_path")]
        public string SavePath { get; set; }

        /// <summary>
        /// Gets or sets the seeding time limit.
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
        /// Gets or sets the torrent tags.
        /// </summary>
        [JsonPropertyName("tags")]
        public IReadOnlyList<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the upload limit.
        /// </summary>
        [JsonPropertyName("upload_limit")]
        public int? UploadLimit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether automatic torrent management is enabled.
        /// </summary>
        [JsonPropertyName("use_auto_tmm")]
        public bool UseAutoTmm { get; set; }

        /// <summary>
        /// Gets or sets the content layout.
        /// </summary>
        [JsonPropertyName("content_layout")]
        public string? ContentLayout { get; set; }
    }
}