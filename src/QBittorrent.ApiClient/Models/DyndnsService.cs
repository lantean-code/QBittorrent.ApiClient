namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies which dynamic DNS service qBittorrent should update.
    /// </summary>
    public enum DyndnsService
    {
        /// <summary>
        /// Disables dynamic DNS service updates.
        /// </summary>
        None = -1,

        /// <summary>
        /// Uses the DynDNS service.
        /// </summary>
        DynDns = 0,

        /// <summary>
        /// Uses the No-IP service.
        /// </summary>
        NoIp = 1
    }
}
