using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents qBittorrent process information.
    /// </summary>
    public record ProcessInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessInfo" /> class.
        /// </summary>
        /// <param name="launchTime">The qBittorrent process launch time as a Unix timestamp in seconds.</param>
        [JsonConstructor]
        public ProcessInfo(long launchTime)
        {
            LaunchTime = launchTime;
        }

        /// <summary>
        /// Gets the qBittorrent process launch time as a Unix timestamp in seconds.
        /// </summary>
        [JsonPropertyName("launch_time")]
        public long LaunchTime { get; }
    }
}
