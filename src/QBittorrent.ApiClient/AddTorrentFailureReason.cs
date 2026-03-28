namespace QBittorrent.ApiClient
{
    /// <summary>
    /// Identifies the action-specific reason an add-torrent request failed.
    /// </summary>
    public enum AddTorrentFailureReason
    {
        /// <summary>
        /// qBittorrent rejected the supplied request payload.
        /// </summary>
        ValidationFailed,

        /// <summary>
        /// The supplied torrent data or metadata format is unsupported.
        /// </summary>
        InvalidTorrentData,

        /// <summary>
        /// qBittorrent could not add any of the requested torrents.
        /// </summary>
        AllTorrentsFailed
    }
}
