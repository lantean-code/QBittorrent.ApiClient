namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies when qBittorrent should stop a torrent after it is added.
    /// </summary>
    public enum StopCondition
    {
        /// <summary>
        /// Does not stop the torrent automatically after it is added.
        /// </summary>
        None = 0,

        /// <summary>
        /// Stops the torrent after its metadata has been downloaded.
        /// </summary>
        MetadataReceived = 1,

        /// <summary>
        /// Stops the torrent after qBittorrent has checked its files.
        /// </summary>
        FilesChecked = 2
    }
}
