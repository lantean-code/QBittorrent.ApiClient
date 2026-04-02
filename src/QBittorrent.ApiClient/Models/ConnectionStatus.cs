using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies qBittorrent's current external connectivity status.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<ConnectionStatus>))]
    public enum ConnectionStatus
    {
        /// <summary>
        /// qBittorrent is accepting incoming connections.
        /// </summary>
        [JsonStringEnumMemberName("connected")]
        Connected,

        /// <summary>
        /// qBittorrent is listening but no incoming connections are detected.
        /// </summary>
        [JsonStringEnumMemberName("firewalled")]
        Firewalled,

        /// <summary>
        /// qBittorrent is not listening for incoming connections.
        /// </summary>
        [JsonStringEnumMemberName("disconnected")]
        Disconnected
    }
}
