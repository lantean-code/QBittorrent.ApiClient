using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies how qBittorrent stores resume data.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<ResumeDataStorageType>))]
    public enum ResumeDataStorageType
    {
        /// <summary>
        /// Stores resume data using the legacy format.
        /// </summary>
        Legacy,

        /// <summary>
        /// Stores resume data in SQLite.
        /// </summary>
        [JsonStringEnumMemberName("SQLite")]
        Sqlite
    }
}
