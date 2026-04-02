namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies how qBittorrent should use the operating system disk cache for reads.
    /// </summary>
    public enum DiskIoReadMode
    {
        /// <summary>
        /// Disables use of the operating system disk cache for reads.
        /// </summary>
        DisableOsCache = 0,

        /// <summary>
        /// Enables use of the operating system disk cache for reads.
        /// </summary>
        EnableOsCache = 1
    }
}
