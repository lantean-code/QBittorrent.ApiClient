using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies the platform reported by the qBittorrent build info endpoint.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<BuildPlatform>))]
    public enum BuildPlatform
    {
        /// <summary>
        /// The platform is not one of the explicitly recognized values.
        /// </summary>
        [JsonStringEnumMemberName("unknown")]
        Unknown,

        /// <summary>
        /// qBittorrent is running on Linux.
        /// </summary>
        [JsonStringEnumMemberName("linux")]
        Linux,

        /// <summary>
        /// qBittorrent is running on macOS.
        /// </summary>
        [JsonStringEnumMemberName("macos")]
        MacOS,

        /// <summary>
        /// qBittorrent is running on Windows.
        /// </summary>
        [JsonStringEnumMemberName("windows")]
        Windows
    }
}
