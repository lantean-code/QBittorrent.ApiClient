using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a qBittorrent log entry.
    /// </summary>
    public record Log
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Log" /> class.
        /// </summary>
        [JsonConstructor]
        public Log(
            int id,
            string message,
            long timestamp,
            LogType type)
        {
            Id = id;
            Message = message;
            Timestamp = timestamp;
            Type = type;
        }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; }

        /// <summary>
        /// Gets the tracker status message.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        [JsonPropertyName("type")]
        public LogType Type { get; }
    }
}