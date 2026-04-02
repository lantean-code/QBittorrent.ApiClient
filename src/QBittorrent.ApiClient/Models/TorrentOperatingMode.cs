using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies how qBittorrent manages a torrent after it is added.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<TorrentOperatingMode>))]
    public enum TorrentOperatingMode
    {
        /// <summary>
        /// Lets qBittorrent manage the torrent automatically.
        /// </summary>
        AutoManaged,

        /// <summary>
        /// Forces the torrent to run outside automatic management.
        /// </summary>
        Forced
    }
}
