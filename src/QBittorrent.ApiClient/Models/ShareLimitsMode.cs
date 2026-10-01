using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies how enabled share limits are combined.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<ShareLimitsMode>))]
    public enum ShareLimitsMode
    {
        /// <summary>
        /// Inherits the applicable category or global mode.
        /// </summary>
        Default = -1,

        /// <summary>
        /// Applies the share-limit action when any enabled limit is reached.
        /// </summary>
        MatchAny = 0,

        /// <summary>
        /// Applies the share-limit action only when all enabled limits are reached.
        /// </summary>
        MatchAll = 1
    }
}
