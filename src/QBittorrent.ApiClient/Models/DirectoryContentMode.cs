namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies which directory entries qBittorrent should return.
    /// </summary>
    public enum DirectoryContentMode
    {
        /// <summary>
        /// Returns both files and directories.
        /// </summary>
        All,

        /// <summary>
        /// Returns only directories.
        /// </summary>
        Directories,

        /// <summary>
        /// Returns only files.
        /// </summary>
        Files
    }
}
