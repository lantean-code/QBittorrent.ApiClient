namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies how qBittorrent should delete added torrent files after they are processed.
    /// </summary>
    public enum AutoDeleteMode
    {
        /// <summary>
        /// Never deletes the added torrent file automatically.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Deletes the added torrent file only if it is successfully added.
        /// </summary>
        IfAdded = 1,

        /// <summary>
        /// Always deletes the added torrent file.
        /// </summary>
        Always = 2
    }
}
