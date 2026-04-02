using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies how qBittorrent removes torrent content from disk.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<TorrentContentRemoveOption>))]
    public enum TorrentContentRemoveOption
    {
        /// <summary>
        /// Deletes the torrent content permanently.
        /// </summary>
        Delete,

        /// <summary>
        /// Moves the torrent content to the recycle bin or trash when possible.
        /// </summary>
        MoveToTrash
    }
}
