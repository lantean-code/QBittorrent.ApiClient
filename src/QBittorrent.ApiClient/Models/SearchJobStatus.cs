using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies the runtime status of a search job.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<SearchJobStatus>))]
    public enum SearchJobStatus
    {
        /// <summary>
        /// The search job is currently running.
        /// </summary>
        Running,

        /// <summary>
        /// The search job is no longer running.
        /// </summary>
        Stopped
    }
}
