using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies the status of a torrent-creation task.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<TorrentCreationTaskStatusKind>))]
    public enum TorrentCreationTaskStatusKind
    {
        /// <summary>
        /// The task is waiting to start.
        /// </summary>
        Queued,

        /// <summary>
        /// The task is currently running.
        /// </summary>
        Running,

        /// <summary>
        /// The task finished successfully.
        /// </summary>
        Finished,

        /// <summary>
        /// The task finished with an error.
        /// </summary>
        Failed
    }
}
