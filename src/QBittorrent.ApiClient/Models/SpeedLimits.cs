using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents the normal and alternative global transfer speed limits.
    /// </summary>
    public record SpeedLimits
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpeedLimits" /> class.
        /// </summary>
        /// <param name="uploadLimit">The normal upload limit in bytes per second.</param>
        /// <param name="downloadLimit">The normal download limit in bytes per second.</param>
        /// <param name="alternativeUploadLimit">The alternative upload limit in bytes per second.</param>
        /// <param name="alternativeDownloadLimit">The alternative download limit in bytes per second.</param>
        [JsonConstructor]
        public SpeedLimits(int uploadLimit, int downloadLimit, int alternativeUploadLimit, int alternativeDownloadLimit)
        {
            UploadLimit = uploadLimit;
            DownloadLimit = downloadLimit;
            AlternativeUploadLimit = alternativeUploadLimit;
            AlternativeDownloadLimit = alternativeDownloadLimit;
        }

        /// <summary>
        /// Gets the normal upload limit in bytes per second.
        /// </summary>
        [JsonPropertyName("up_limit")]
        public int UploadLimit { get; }

        /// <summary>
        /// Gets the normal download limit in bytes per second.
        /// </summary>
        [JsonPropertyName("dl_limit")]
        public int DownloadLimit { get; }

        /// <summary>
        /// Gets the alternative upload limit in bytes per second.
        /// </summary>
        [JsonPropertyName("alt_up_limit")]
        public int AlternativeUploadLimit { get; }

        /// <summary>
        /// Gets the alternative download limit in bytes per second.
        /// </summary>
        [JsonPropertyName("alt_dl_limit")]
        public int AlternativeDownloadLimit { get; }
    }
}
