namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies qBittorrent file-priority values.
    /// </summary>
    public enum Priority
    {
        /// <summary>
        /// Skips the file instead of downloading it.
        /// </summary>
        DoNotDownload = 0,

        /// <summary>
        /// Downloads the file with normal priority.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// Downloads the file with high priority.
        /// </summary>
        High = 6,

        /// <summary>
        /// Downloads the file with the highest priority.
        /// </summary>
        Maximum = 7
    }
}
