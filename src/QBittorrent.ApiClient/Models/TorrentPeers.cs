using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents peer-sync data for a torrent.
    /// </summary>
    public record TorrentPeers
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TorrentPeers" /> class.
        /// </summary>
        [JsonConstructor]
        public TorrentPeers(
            bool fullUpdate,
            IReadOnlyDictionary<string, Peer>? peers,
            IReadOnlyList<string>? peersRemoved,
            int requestId,
            bool? showFlags)
        {
            FullUpdate = fullUpdate;
            Peers = peers;
            PeersRemoved = peersRemoved;
            RequestId = requestId;
            ShowFlags = showFlags;
        }

        /// <summary>
        /// Gets a value indicating whether the payload contains a full update.
        /// </summary>
        [JsonPropertyName("full_update")]
        public bool FullUpdate { get; }

        /// <summary>
        /// Gets the number of peers.
        /// </summary>
        [JsonPropertyName("peers")]
        public IReadOnlyDictionary<string, Peer>? Peers { get; }

        /// <summary>
        /// Gets the peers removed.
        /// </summary>
        [JsonPropertyName("peers_removed")]
        public IReadOnlyList<string>? PeersRemoved { get; }

        /// <summary>
        /// Gets the request ID.
        /// </summary>
        [JsonPropertyName("rid")]
        public int RequestId { get; }

        /// <summary>
        /// Gets a value indicating whether peer flags are included.
        /// </summary>
        [JsonPropertyName("show_flags")]
        public bool? ShowFlags { get; }
    }
}