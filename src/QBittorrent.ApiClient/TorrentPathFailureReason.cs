namespace QBittorrent.ApiClient
{
    /// <summary>
    /// Identifies the action-specific reason a torrent path update failed.
    /// </summary>
    public enum TorrentPathFailureReason
    {
        /// <summary>
        /// qBittorrent could not write to the target directory.
        /// </summary>
        DirectoryNotWritable,

        /// <summary>
        /// qBittorrent could not create the target directory.
        /// </summary>
        DirectoryCreationFailed
    }
}
