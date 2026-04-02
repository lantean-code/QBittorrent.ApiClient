using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a directory entry returned by qBittorrent with metadata.
    /// </summary>
    public record DirectoryContentEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryContentEntry" /> class.
        /// </summary>
        /// <param name="name">The entry name.</param>
        /// <param name="type">The entry type.</param>
        /// <param name="size">The entry size in bytes when the entry is a file.</param>
        /// <param name="creationDate">The entry creation time as a Unix timestamp in seconds.</param>
        /// <param name="lastAccessDate">The entry last access time as a Unix timestamp in seconds.</param>
        /// <param name="lastModificationDate">The entry last modification time as a Unix timestamp in seconds.</param>
        [JsonConstructor]
        public DirectoryContentEntry(
            string name,
            DirectoryContentEntryType type,
            long? size,
            long creationDate,
            long lastAccessDate,
            long lastModificationDate)
        {
            Name = name;
            Type = type;
            Size = size;
            CreationDate = creationDate;
            LastAccessDate = lastAccessDate;
            LastModificationDate = lastModificationDate;
        }

        /// <summary>
        /// Gets the entry name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        /// Gets the entry type.
        /// </summary>
        [JsonPropertyName("type")]
        public DirectoryContentEntryType Type { get; }

        /// <summary>
        /// Gets the entry size in bytes when available.
        /// </summary>
        [JsonPropertyName("size")]
        public long? Size { get; }

        /// <summary>
        /// Gets the creation time as a Unix timestamp in seconds.
        /// </summary>
        [JsonPropertyName("creation_date")]
        public long CreationDate { get; }

        /// <summary>
        /// Gets the last access time as a Unix timestamp in seconds.
        /// </summary>
        [JsonPropertyName("last_access_date")]
        public long LastAccessDate { get; }

        /// <summary>
        /// Gets the last modification time as a Unix timestamp in seconds.
        /// </summary>
        [JsonPropertyName("last_modification_date")]
        public long LastModificationDate { get; }
    }
}
