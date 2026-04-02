namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies which peer connection protocols qBittorrent should use.
    /// </summary>
    public enum BittorrentProtocol
    {
        /// <summary>
        /// Allows both TCP and uTP peer connections.
        /// </summary>
        TcpAndUtp = 0,

        /// <summary>
        /// Allows only TCP peer connections.
        /// </summary>
        TcpOnly = 1,

        /// <summary>
        /// Allows only uTP peer connections.
        /// </summary>
        UtpOnly = 2
    }
}
