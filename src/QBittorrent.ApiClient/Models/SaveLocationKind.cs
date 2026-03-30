namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies the type of qBittorrent save-location selection.
    /// </summary>
    public enum SaveLocationKind
    {
        /// <summary>
        /// Uses the watched folder.
        /// </summary>
        WatchedFolder,

        /// <summary>
        /// Uses the default save folder.
        /// </summary>
        DefaultFolder,

        /// <summary>
        /// Uses an explicit custom save path.
        /// </summary>
        CustomPath
    }
}
