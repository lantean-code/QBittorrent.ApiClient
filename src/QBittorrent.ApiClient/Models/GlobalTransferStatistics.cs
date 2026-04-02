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
            ConnectionStatus? connectionStatus,
            long? dHTNodes,
            long? downloadInfoData,
            long? downloadInfoSpeed,
            int? downloadRateLimit,
            long? uploadInfoData,
            long? uploadInfoSpeed,
            int? uploadRateLimit,
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
        public ConnectionStatus? ConnectionStatus { get; }

        /// <summary>
        /// Gets the number of DHT nodes.
        /// </summary>
        [JsonPropertyName("dht_nodes")]
        public long? DHTNodes { get; }

        /// <summary>
        /// Gets the downloaded data total in bytes.
        /// </summary>
        [JsonPropertyName("dl_info_data")]
        public long? DownloadInfoData { get; }

        /// <summary>
        /// Gets the current download speed in bytes per second.
        /// </summary>
        [JsonPropertyName("dl_info_speed")]
        public long? DownloadInfoSpeed { get; }

        /// <summary>
        /// Gets the download rate limit in bytes per second.
        /// </summary>
        [JsonPropertyName("dl_rate_limit")]
        public int? DownloadRateLimit { get; }

        /// <summary>
        /// Gets the uploaded data total in bytes.
        /// </summary>
        [JsonPropertyName("up_info_data")]
        public long? UploadInfoData { get; }

        /// <summary>
        /// Gets the current upload speed in bytes per second.
        /// </summary>
        [JsonPropertyName("up_info_speed")]
        public long? UploadInfoSpeed { get; }

        /// <summary>
        /// Gets the upload rate limit in bytes per second.
        /// </summary>
        [JsonPropertyName("up_rate_limit")]
        public int? UploadRateLimit { get; }

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
