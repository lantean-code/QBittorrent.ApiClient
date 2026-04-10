using System.Text.Json.Serialization;
using QBittorrent.ApiClient.Converters;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies the type of a directory entry returned by qBittorrent.
    /// </summary>
    [JsonConverter(typeof(DirectoryContentEntryTypeJsonConverter))]
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
