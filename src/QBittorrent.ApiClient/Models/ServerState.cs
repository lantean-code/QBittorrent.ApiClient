using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents server-state information from the sync API.
    /// </summary>
    public record ServerState : GlobalTransferInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerState" /> class.
        /// </summary>
        [JsonConstructor]
        public ServerState(
            long? allTimeDownloaded,
            long? allTimeUploaded,
            int? averageTimeQueue,
            string? connectionStatus,
            int? dHTNodes,
            long? downloadInfoData,
            long? downloadInfoSpeed,
            long? downloadRateLimit,
            long? freeSpaceOnDisk,
            float? globalRatio,
            int? queuedIOJobs,
            bool? queuing,
            float? readCacheHits,
            float? readCacheOverload,
            int? refreshInterval,
            int? totalBuffersSize,
            int? totalPeerConnections,
            int? totalQueuedSize,
            long? totalWastedSession,
            long? uploadInfoData,
            long? uploadInfoSpeed,
            long? uploadRateLimit,
            bool? useAltSpeedLimits,
            bool? useSubcategories,
            float? writeCacheOverload,
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
        /// Gets the all time downloaded.
        /// </summary>
        [JsonPropertyName("alltime_dl")]
        public long? AllTimeDownloaded { get; }

        /// <summary>
        /// Gets the all time uploaded.
        /// </summary>
        [JsonPropertyName("alltime_ul")]
        public long? AllTimeUploaded { get; }

        /// <summary>
        /// Gets the average time queue.
        /// </summary>
        [JsonPropertyName("average_time_queue")]
        public int? AverageTimeQueue { get; }

        /// <summary>
        /// Gets the free space on disk.
        /// </summary>
        [JsonPropertyName("free_space_on_disk")]
        public long? FreeSpaceOnDisk { get; }

        /// <summary>
        /// Gets the global ratio.
        /// </summary>
        [JsonPropertyName("global_ratio")]
        public float? GlobalRatio { get; }

        /// <summary>
        /// Gets the queued io jobs.
        /// </summary>
        [JsonPropertyName("queued_io_jobs")]
        public int? QueuedIOJobs { get; }

        /// <summary>
        /// Gets a value indicating whether torrent queueing is enabled.
        /// </summary>
        [JsonPropertyName("queueing")]
        public bool? Queuing { get; }

        /// <summary>
        /// Gets the read cache hits.
        /// </summary>
        [JsonPropertyName("read_cache_hits")]
        public float? ReadCacheHits { get; }

        /// <summary>
        /// Gets the read cache overload.
        /// </summary>
        [JsonPropertyName("read_cache_overload")]
        public float? ReadCacheOverload { get; }

        /// <summary>
        /// Gets the refresh interval.
        /// </summary>
        [JsonPropertyName("refresh_interval")]
        public int? RefreshInterval { get; }

        /// <summary>
        /// Gets the total buffers size.
        /// </summary>
        [JsonPropertyName("total_buffers_size")]
        public int? TotalBuffersSize { get; }

        /// <summary>
        /// Gets the total peer connections.
        /// </summary>
        [JsonPropertyName("total_peer_connections")]
        public int? TotalPeerConnections { get; }

        /// <summary>
        /// Gets the total queued size.
        /// </summary>
        [JsonPropertyName("total_queued_size")]
        public int? TotalQueuedSize { get; }

        /// <summary>
        /// Gets the total wasted session.
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
        /// Gets the write cache overload.
        /// </summary>
        [JsonPropertyName("write_cache_overload")]
        public float? WriteCacheOverload { get; }
    }
}
