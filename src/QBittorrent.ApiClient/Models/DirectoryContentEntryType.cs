namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies the type of a directory entry returned by qBittorrent.
    /// </summary>
    public enum DirectoryContentEntryType
    {
        /// <summary>
        /// Indicates that the entry is a directory.
        /// </summary>
        Directory,

        /// <summary>
        /// Indicates that the entry is a file.
        /// </summary>
        File
    }
}
