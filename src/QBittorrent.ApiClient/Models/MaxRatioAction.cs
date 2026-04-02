namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies which action qBittorrent should take when the configured share limits are reached.
    /// </summary>
    public enum MaxRatioAction
    {
        /// <summary>
        /// Stops the torrent.
        /// </summary>
        StopTorrent = 0,

        /// <summary>
        /// Removes the torrent.
        /// </summary>
        RemoveTorrent = 1,

        /// <summary>
        /// Enables super seeding for the torrent.
        /// </summary>
        EnableSuperSeeding = 2,

        /// <summary>
        /// Removes the torrent and its files.
        /// </summary>
        RemoveTorrentAndFiles = 3
    }
}
