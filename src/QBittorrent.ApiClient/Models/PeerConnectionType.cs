using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies the connection type reported for a torrent peer.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<PeerConnectionType>))]
    public enum PeerConnectionType
    {
        /// <summary>
        /// The peer is connected over uTP.
        /// </summary>
        [JsonStringEnumMemberName("\u03BCTP")]
        Utp,

        /// <summary>
        /// The peer is using the BitTorrent peer protocol directly.
        /// </summary>
        [JsonStringEnumMemberName("BT")]
        Bittorrent,

        /// <summary>
        /// The peer is a web seed connection.
        /// </summary>
        [JsonStringEnumMemberName("Web")]
        Web
    }
}
