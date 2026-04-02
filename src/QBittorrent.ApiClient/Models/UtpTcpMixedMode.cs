namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies how qBittorrent should balance uTP and TCP traffic.
    /// </summary>
    public enum UtpTcpMixedMode
    {
        /// <summary>
        /// Prefers TCP connections.
        /// </summary>
        PreferTcp = 0,

        /// <summary>
        /// Uses peer-proportional balancing.
        /// </summary>
        PeerProportional = 1
    }
}
