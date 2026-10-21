using System.Text.Json.Serialization;
using QBittorrent.ApiClient.Converters;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a qBittorrent torrent category.
    /// </summary>
    public record Category
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Category" /> class.
        /// </summary>
        [JsonConstructor]
        public Category(
            string name,
            string? savePath,
            DownloadPathOption? downloadPath)
        {
            Name = name;
            SavePath = savePath;
            DownloadPath = downloadPath;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        /// Gets the save path.
        /// </summary>
        [JsonPropertyName("savePath")]
        public string? SavePath { get; }

        /// <summary>
        /// Gets the download path.
        /// </summary>
        [JsonPropertyName("download_path")]
        [JsonConverter(typeof(DownloadPathOptionJsonConverter))]
        public DownloadPathOption? DownloadPath { get; }

        /// <summary>
        /// Gets the category share-ratio limit.
        /// </summary>
        [JsonPropertyName("ratio_limit")]
        public double? RatioLimit { get; init; }

        /// <summary>
        /// Gets the category seeding-time limit in minutes.
        /// </summary>
        [JsonPropertyName("seeding_time_limit")]
        public int? SeedingTimeLimit { get; init; }

        /// <summary>
        /// Gets the category inactive-seeding-time limit in minutes.
        /// </summary>
        [JsonPropertyName("inactive_seeding_time_limit")]
        public int? InactiveSeedingTimeLimit { get; init; }

        /// <summary>
        /// Gets how enabled category share limits are combined.
        /// </summary>
        [JsonPropertyName("share_limits_mode")]
        [JsonConverter(typeof(JsonStringEnumConverter<ShareLimitsMode>))]
        public ShareLimitsMode? ShareLimitsMode { get; init; }

        /// <summary>
        /// Gets the action applied when the category share limits are reached.
        /// </summary>
        [JsonPropertyName("share_limit_action")]
        [JsonConverter(typeof(JsonStringEnumConverter<ShareLimitAction>))]
        public ShareLimitAction? ShareLimitAction { get; init; }
    }
}
