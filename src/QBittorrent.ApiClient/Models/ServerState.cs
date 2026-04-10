using System.Text.Json.Serialization;
using QBittorrent.ApiClient.Converters;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents server-state information from the sync API.
    /// </summary>
    public record ServerState : GlobalTransferStatistics
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerState" /> class.
        /// </summary>
        [JsonConstructor]
        public ServerState(
            long? allTimeDownloaded,
            long? allTimeUploaded,
            long? averageTimeQueue,
            ConnectionStatus? connectionStatus,
            long? dHTNodes,
            long? downloadInfoData,
            long? downloadInfoSpeed,
            int? downloadRateLimit,
            long? freeSpaceOnDisk,
            double? globalRatio,
            long? queuedIOJobs,
            bool? queuing,
            double? readCacheHits,
            double? readCacheOverload,
            int? refreshInterval,
            long? totalBuffersSize,
            long? totalPeerConnections,
            long? totalQueuedSize,
            long? totalWastedSession,
            long? uploadInfoData,
            long? uploadInfoSpeed,
            int? uploadRateLimit,
            bool? useAltSpeedLimits,
            bool? useSubcategories,
            double? writeCacheOverload,
            string? lastExternalAddressV4 = null,
            string? lastExternalAddressV6 = null) : base(connectionStatus, dHTNodes, downloadInfoData, downloadInfoSpeed, downloadRateLimit, uploadInfoData, uploadInfoSpeed, uploadRateLimit, lastExternalAddressV4, lastExternalAddressV6)
        {
            AllTimeDownloaded = allTimeDownloaded;
            AllTimeUploaded = allTimeUploaded;
            AverageTimeQueue = averageTimeQueue;
            FreeSpaceOnDisk = freeSpaceOnDisk;
            GlobalRatio = globalRatio;
            QueuedIOJobs = queuedIOJobs;
            Queuing = queuing;
            ReadCacheHits = readCacheHits;
            ReadCacheOverload = readCacheOverload;
            RefreshInterval = refreshInterval;
            TotalBuffersSize = totalBuffersSize;
            TotalPeerConnections = totalPeerConnections;
            TotalQueuedSize = totalQueuedSize;
            TotalWastedSession = totalWastedSession;
            UseAltSpeedLimits = useAltSpeedLimits;
            UseSubcategories = useSubcategories;
            WriteCacheOverload = writeCacheOverload;
        }

        /// <summary>
        /// Gets the all-time downloaded total in bytes.
        /// </summary>
        [JsonPropertyName("alltime_dl")]
        public long? AllTimeDownloaded { get; }

        /// <summary>
        /// Gets the all-time uploaded total in bytes.
        /// </summary>
        [JsonPropertyName("alltime_ul")]
        public long? AllTimeUploaded { get; }

        /// <summary>
        /// Gets the average queued I/O job time in milliseconds.
        /// </summary>
        [JsonPropertyName("average_time_queue")]
        public long? AverageTimeQueue { get; }

        /// <summary>
        /// Gets the free space on disk in bytes.
        /// </summary>
        [JsonPropertyName("free_space_on_disk")]
        public long? FreeSpaceOnDisk { get; }

        /// <summary>
        /// Gets the global share ratio as a unitless ratio value.
        /// </summary>
        [JsonConverter(typeof(NullableStringDoubleJsonConverter))]
        [JsonPropertyName("global_ratio")]
        public double? GlobalRatio { get; }

        /// <summary>
        /// Gets the number of queued I/O jobs.
        /// </summary>
        [JsonPropertyName("queued_io_jobs")]
        public long? QueuedIOJobs { get; }

        /// <summary>
        /// Gets a value indicating whether torrent queueing is enabled.
        /// </summary>
        [JsonPropertyName("queueing")]
        public bool? Queuing { get; }

        /// <summary>
        /// Gets the read cache hit rate as a percentage.
        /// </summary>
        [JsonConverter(typeof(NullableStringDoubleJsonConverter))]
        [JsonPropertyName("read_cache_hits")]
        public double? ReadCacheHits { get; }

        /// <summary>
        /// Gets the read cache overload as a percentage.
        /// </summary>
        [JsonConverter(typeof(NullableStringDoubleJsonConverter))]
        [JsonPropertyName("read_cache_overload")]
        public double? ReadCacheOverload { get; }

        /// <summary>
        /// Gets the sync refresh interval in milliseconds.
        /// </summary>
        [JsonPropertyName("refresh_interval")]
        public int? RefreshInterval { get; }

        /// <summary>
        /// Gets the total buffer size in bytes.
        /// </summary>
        [JsonPropertyName("total_buffers_size")]
        public long? TotalBuffersSize { get; }

        /// <summary>
        /// Gets the total number of peer connections.
        /// </summary>
        [JsonPropertyName("total_peer_connections")]
        public long? TotalPeerConnections { get; }

        /// <summary>
        /// Gets the total queued size in bytes.
        /// </summary>
        [JsonPropertyName("total_queued_size")]
        public long? TotalQueuedSize { get; }

        /// <summary>
        /// Gets the total wasted data for the current session in bytes.
        /// </summary>
        [JsonPropertyName("total_wasted_session")]
        public long? TotalWastedSession { get; }

        /// <summary>
        /// Gets a value indicating whether alt speed limits is used.
        /// </summary>
        [JsonPropertyName("use_alt_speed_limits")]
        public bool? UseAltSpeedLimits { get; }

        /// <summary>
        /// Gets a value indicating whether subcategories are used.
        /// </summary>
        [JsonPropertyName("use_subcategories")]
        public bool? UseSubcategories { get; }

        /// <summary>
        /// Gets the write cache overload as a percentage.
        /// </summary>
        [JsonConverter(typeof(NullableStringDoubleJsonConverter))]
        [JsonPropertyName("write_cache_overload")]
        public double? WriteCacheOverload { get; }
    }
}
