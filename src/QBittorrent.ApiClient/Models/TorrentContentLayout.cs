using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies how qBittorrent should lay out torrent content on disk.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<TorrentContentLayout>))]
    public enum TorrentContentLayout
    {
        /// <summary>
        /// Preserves the layout defined by the torrent metadata.
        /// </summary>
        Original,

        /// <summary>
        /// Stores the torrent content inside a dedicated subfolder.
        /// </summary>
        Subfolder,

        /// <summary>
        /// Stores the torrent content without creating a subfolder.
        /// </summary>
        NoSubfolder
    }
}
