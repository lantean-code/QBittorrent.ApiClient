namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies how qBittorrent should use the operating system disk cache for writes.
    /// </summary>
    public enum DiskIoWriteMode
    {
        /// <summary>
        /// Disables use of the operating system disk cache for writes.
        /// </summary>
        DisableOsCache = 0,

        /// <summary>
        /// Enables use of the operating system disk cache for writes.
        /// </summary>
        EnableOsCache = 1,

        /// <summary>
        /// Uses write-through mode when supported by the upstream build.
        /// </summary>
        WriteThrough = 2
    }
}
