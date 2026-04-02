using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a peer log entry.
    /// </summary>
    public record PeerLog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PeerLog" /> class.
        /// </summary>
        [JsonConstructor]
        public PeerLog(
            int id,
            string iPAddress,
            long timestamp,
            bool blocked,
            string reason)
        {
            Id = id;
            IPAddress = iPAddress;
            Timestamp = timestamp;
            Blocked = blocked;
            Reason = reason;
        }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; }

        /// <summary>
        /// Gets the ip address.
        /// </summary>
        [JsonPropertyName("ip")]
        public string IPAddress { get; }

        /// <summary>
        /// Gets the event time as a Unix timestamp in seconds.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; }

        /// <summary>
        /// Gets a value indicating whether the peer is blocked.
        /// </summary>
        [JsonPropertyName("blocked")]
        public bool Blocked { get; }

        /// <summary>
        /// Gets the reason.
        /// </summary>
        [JsonPropertyName("reason")]
        public string Reason { get; }
    }
}
