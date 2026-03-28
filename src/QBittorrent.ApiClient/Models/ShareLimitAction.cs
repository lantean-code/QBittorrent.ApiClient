namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies the action qBittorrent should take when a share limit is reached.
    /// </summary>
    public enum ShareLimitAction
    {
        /// <summary>
        /// Uses qBittorrent's configured default action for reached share limits.
        /// </summary>
        Default = -1, // special value

        /// <summary>
        /// Stops the torrent when its share limits are reached.
        /// </summary>
        Stop = 0,

        /// <summary>
        /// Removes the torrent when its share limits are reached.
        /// </summary>
        Remove = 1,

        /// <summary>
        /// Removes the torrent and its downloaded content when its share limits are reached.
        /// </summary>
        RemoveWithContent = 3,

        /// <summary>
        /// Enables super seeding when the torrent reaches its share limits.
        /// </summary>
        EnableSuperSeeding = 2
    }
}
