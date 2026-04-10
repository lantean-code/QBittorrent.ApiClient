using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents the response payload returned when qBittorrent accepts a torrent-creation task.
    /// </summary>
    public sealed record TorrentCreationTaskIdentifier
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TorrentCreationTaskIdentifier" /> class.
        /// </summary>
        /// <param name="taskID">The torrent-creation task identifier.</param>
        [JsonConstructor]
        public TorrentCreationTaskIdentifier(string? taskID)
        {
            TaskId = taskID;
        }

        /// <summary>
        /// Gets the torrent-creation task identifier.
        /// </summary>
        [JsonPropertyName("taskID")]
        public string? TaskId { get; }
    }
}
