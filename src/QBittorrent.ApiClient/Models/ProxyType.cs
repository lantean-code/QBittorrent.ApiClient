using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies the proxy type qBittorrent should use.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<ProxyType>))]
    public enum ProxyType
    {
        /// <summary>
        /// Disables proxy usage.
        /// </summary>
        None = 0,

        /// <summary>
        /// Uses an HTTP proxy.
        /// </summary>
        [JsonStringEnumMemberName("HTTP")]
        Http = 1,

        /// <summary>
        /// Uses a SOCKS5 proxy.
        /// </summary>
        [JsonStringEnumMemberName("SOCKS5")]
        Socks5 = 2,

        /// <summary>
        /// Uses a SOCKS4 proxy.
        /// </summary>
        [JsonStringEnumMemberName("SOCKS4")]
        Socks4 = 5
    }
}
