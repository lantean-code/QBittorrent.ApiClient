using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents global transfer statistics reported by qBittorrent.
    /// </summary>
    public record GlobalTransferStatistics
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalTransferStatistics" /> class.
        /// </summary>
        [JsonConstructor]
        public GlobalTransferStatistics(
            string? connectionStatus,
            int? dHTNodes,
            long? downloadInfoData,
            long? downloadInfoSpeed,
            long? downloadRateLimit,
            long? uploadInfoData,
            long? uploadInfoSpeed,
            long? uploadRateLimit,
            string? lastExternalAddressV4 = null,
            string? lastExternalAddressV6 = null)
        {
            ConnectionStatus = connectionStatus;
            DHTNodes = dHTNodes;
            DownloadInfoData = downloadInfoData;
            DownloadInfoSpeed = downloadInfoSpeed;
            DownloadRateLimit = downloadRateLimit;
            UploadInfoData = uploadInfoData;
            UploadInfoSpeed = uploadInfoSpeed;
            UploadRateLimit = uploadRateLimit;
            LastExternalAddressV4 = lastExternalAddressV4;
            LastExternalAddressV6 = lastExternalAddressV6;
        }

        /// <summary>
        /// Gets the connection status.
        /// </summary>
        [JsonPropertyName("connection_status")]
        public string? ConnectionStatus { get; }

        /// <summary>
        /// Gets the dht nodes.
        /// </summary>
        [JsonPropertyName("dht_nodes")]
        public int? DHTNodes { get; }

        /// <summary>
        /// Gets the download info data.
        /// </summary>
        [JsonPropertyName("dl_info_data")]
        public long? DownloadInfoData { get; }

        /// <summary>
        /// Gets the download info speed.
        /// </summary>
        [JsonPropertyName("dl_info_speed")]
        public long? DownloadInfoSpeed { get; }

        /// <summary>
        /// Gets the download rate limit.
        /// </summary>
        [JsonPropertyName("dl_rate_limit")]
        public long? DownloadRateLimit { get; }

        /// <summary>
        /// Gets the upload info data.
        /// </summary>
        [JsonPropertyName("up_info_data")]
        public long? UploadInfoData { get; }

        /// <summary>
        /// Gets the upload info speed.
        /// </summary>
        [JsonPropertyName("up_info_speed")]
        public long? UploadInfoSpeed { get; }

        /// <summary>
        /// Gets the upload rate limit.
        /// </summary>
        [JsonPropertyName("up_rate_limit")]
        public long? UploadRateLimit { get; }

        /// <summary>
        /// Gets the last external address v 4.
        /// </summary>
        [JsonPropertyName("last_external_address_v4")]
        public string? LastExternalAddressV4 { get; }

        /// <summary>
        /// Gets the last external address v 6.
        /// </summary>
        [JsonPropertyName("last_external_address_v6")]
        public string? LastExternalAddressV6 { get; }
    }
}
