using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies the torrent metadata format qBittorrent should create or report.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<TorrentFormat>))]
    public enum TorrentFormat
    {
        /// <summary>
        /// Creates or reports a v1 torrent.
        /// </summary>
        [JsonStringEnumMemberName("v1")]
        V1,

        /// <summary>
        /// Creates or reports a v2 torrent.
        /// </summary>
        [JsonStringEnumMemberName("v2")]
        V2,

        /// <summary>
        /// Creates or reports a hybrid torrent.
        /// </summary>
        [JsonStringEnumMemberName("hybrid")]
        Hybrid
    }
}
