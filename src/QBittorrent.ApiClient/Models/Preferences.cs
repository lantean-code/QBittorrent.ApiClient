using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents the current qBittorrent application preferences.
    /// </summary>
    public record Preferences
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Preferences" /> class.
        /// </summary>
        [JsonConstructor]
        public Preferences(
            bool addToTopOfQueue,
            bool addStoppedEnabled,
            string addTrackers,
            bool addTrackersEnabled,
            bool addTrackersFromUrlEnabled,
            string addTrackersUrl,
            string addTrackersUrlList,
            int altDlLimit,
            int altUpLimit,
            bool alternativeWebuiEnabled,
            string alternativeWebuiPath,
            string announceIp,
            int announcePort,
            bool announceToAllTiers,
            bool announceToAllTrackers,
            bool anonymousMode,
            string appInstanceName,
            int asyncIoThreads,
            AutoDeleteMode autoDeleteMode,
            bool autoTmmEnabled,
            bool autorunEnabled,
            bool autorunOnTorrentAddedEnabled,
            string autorunOnTorrentAddedProgram,
            string autorunProgram,
            bool deleteTorrentContentFiles,
            string bannedIPs,
            int bdecodeDepthLimit,
            int bdecodeTokenLimit,
            BittorrentProtocol bittorrentProtocol,
            bool blockPeersOnPrivilegedPorts,
            string bypassAuthSubnetWhitelist,
            bool bypassAuthSubnetWhitelistEnabled,
            bool bypassLocalAuth,
            bool categoryChangedTmmEnabled,
            int checkingMemoryUse,
            int connectionSpeed,
            string currentInterfaceAddress,
            string currentInterfaceName,
            string currentNetworkInterface,
            bool dht,
            string dhtBootstrapNodes,
            int diskCache,
            int diskCacheTtl,
            DiskIoReadMode diskIoReadMode,
            DiskIoType diskIoType,
            DiskIoWriteMode diskIoWriteMode,
            int diskQueueSize,
            int dlLimit,
            bool dontCountSlowTorrents,
            string dyndnsDomain,
            bool dyndnsEnabled,
            string dyndnsPassword,
            DyndnsService dyndnsService,
            string dyndnsUsername,
            int embeddedTrackerPort,
            bool embeddedTrackerPortForwarding,
            bool enableCoalesceReadWrite,
            bool enableEmbeddedTracker,
            bool enableMultiConnectionsFromSameIp,
            bool enablePieceExtentAffinity,
            bool enableUploadSuggestions,
            EncryptionMode encryption,
            string excludedFileNames,
            bool excludedFileNamesEnabled,
            string exportDir,
            string exportDirFin,
            int fileLogAge,
            int fileLogAgeType,
            bool fileLogBackupEnabled,
            bool fileLogDeleteOld,
            bool fileLogEnabled,
            int fileLogMaxSize,
            string fileLogPath,
            int filePoolSize,
            int hashingThreads,
            string i2pAddress,
            bool i2pEnabled,
            int i2pInboundLength,
            int i2pInboundQuantity,
            bool i2pMixedMode,
            int i2pOutboundLength,
            int i2pOutboundQuantity,
            int i2pPort,
            bool idnSupportEnabled,
            bool incompleteFilesExt,
            bool useUnwantedFolder,
            bool ipFilterEnabled,
            string ipFilterPath,
            bool ipFilterTrackers,
            bool limitLanPeers,
            bool limitTcpOverhead,
            bool limitUtpRate,
            int listenPort,
            bool sslEnabled,
            int sslListenPort,
            string locale,
            bool lsd,
            bool mailNotificationAuthEnabled,
            string mailNotificationEmail,
            bool mailNotificationEnabled,
            string mailNotificationPassword,
            string mailNotificationSender,
            string mailNotificationSmtp,
            bool mailNotificationSslEnabled,
            string mailNotificationUsername,
            bool markOfTheWeb,
            int maxActiveCheckingTorrents,
            int maxActiveDownloads,
            int maxActiveTorrents,
            int maxActiveUploads,
            int maxConcurrentHttpAnnounces,
            int maxConnec,
            int maxConnecPerTorrent,
            int maxInactiveSeedingTime,
            bool maxInactiveSeedingTimeEnabled,
            double maxRatio,
            MaxRatioAction maxRatioAct,
            bool maxRatioEnabled,
            int maxSeedingTime,
            bool maxSeedingTimeEnabled,
            int maxUploads,
            int maxUploadsPerTorrent,
            int memoryWorkingSetLimit,
            bool mergeTrackers,
            int outgoingPortsMax,
            int outgoingPortsMin,
            int peerTos,
            int peerTurnover,
            int peerTurnoverCutoff,
            int peerTurnoverInterval,
            bool performanceWarning,
            bool pex,
            bool preallocateAll,
            bool proxyAuthEnabled,
            bool proxyBittorrent,
            bool proxyHostnameLookup,
            string proxyIp,
            bool proxyMisc,
            string proxyPassword,
            bool proxyPeerConnections,
            int proxyPort,
            bool proxyRss,
            ProxyType proxyType,
            string proxyUsername,
            string pythonExecutablePath,
            bool queueingEnabled,
            bool randomPort,
            bool reannounceWhenAddressChanged,
            bool recheckCompletedTorrents,
            int refreshInterval,
            int requestQueueSize,
            bool? resolvePeerHostNames,
            bool resolvePeerCountries,
            ResumeDataStorageType resumeDataStorageType,
            bool rssAutoDownloadingEnabled,
            long rssFetchDelay,
            bool rssDownloadRepackProperEpisodes,
            int rssMaxArticlesPerFeed,
            bool rssProcessingEnabled,
            int rssRefreshInterval,
            string rssSmartEpisodeFilters,
            string savePath,
            bool savePathChangedTmmEnabled,
            int saveResumeDataInterval,
            int saveStatisticsInterval,
            Dictionary<string, SaveLocation> scanDirs,
            int scheduleFromHour,
            int scheduleFromMin,
            int scheduleToHour,
            int scheduleToMin,
            SchedulerDays schedulerDays,
            bool schedulerEnabled,
            int sendBufferLowWatermark,
            int sendBufferWatermark,
            int sendBufferWatermarkFactor,
            int slowTorrentDlRateThreshold,
            int slowTorrentInactiveTimer,
            int slowTorrentUlRateThreshold,
            int socketBacklogSize,
            int socketReceiveBufferSize,
            int socketSendBufferSize,
            bool ssrfMitigation,
            int stopTrackerTimeout,
            string tempPath,
            bool tempPathEnabled,
            bool torrentChangedTmmEnabled,
            TorrentContentLayout torrentContentLayout,
            TorrentContentRemoveOption torrentContentRemoveOption,
            int torrentFileSizeLimit,
            StopCondition torrentStopCondition,
            int upLimit,
            UploadChokingAlgorithm uploadChokingAlgorithm,
            UploadSlotsBehavior uploadSlotsBehavior,
            bool upnp,
            int upnpLeaseDuration,
            bool useCategoryPathsInManualMode,
            bool useHttps,
            bool ignoreSslErrors,
            bool? useSubcategories,
            UtpTcpMixedMode utpTcpMixedMode,
            bool validateHttpsTrackerCertificate,
            int? hostnameCacheTtl,
            string webUiAddress,
            string webUiApiKey,
            int webUiBanDuration,
            bool webUiClickjackingProtectionEnabled,
            bool webUiCsrfProtectionEnabled,
            string webUiCustomHttpHeaders,
            string webUiDomainList,
            bool webUiHostHeaderValidationEnabled,
            string webUiHttpsCertPath,
            string webUiHttpsKeyPath,
            int webUiMaxAuthFailCount,
            int webUiPort,
            string webUiReverseProxiesList,
            bool webUiReverseProxyEnabled,
            bool webUiSecureCookieEnabled,
            int webUiSessionTimeout,
            bool webUiUpnp,
            bool webUiUseCustomHttpHeadersEnabled,
            string webUiUsername,
            bool confirmTorrentDeletion,
            bool confirmTorrentRecheck,
            bool statusBarExternalIp
        )
        {
            AddToTopOfQueue = addToTopOfQueue;
            AddStoppedEnabled = addStoppedEnabled;
            AddTrackers = addTrackers;
            AddTrackersEnabled = addTrackersEnabled;
            AddTrackersFromUrlEnabled = addTrackersFromUrlEnabled;
            AddTrackersUrl = addTrackersUrl;
            AddTrackersUrlList = addTrackersUrlList;
            AltDlLimit = altDlLimit;
            AltUpLimit = altUpLimit;
            AlternativeWebuiEnabled = alternativeWebuiEnabled;
            AlternativeWebuiPath = alternativeWebuiPath;
            AnnounceIp = announceIp;
            AnnouncePort = announcePort;
            AnnounceToAllTiers = announceToAllTiers;
            AnnounceToAllTrackers = announceToAllTrackers;
            AnonymousMode = anonymousMode;
            AppInstanceName = appInstanceName;
            AsyncIoThreads = asyncIoThreads;
            AutoDeleteMode = autoDeleteMode;
            AutoTmmEnabled = autoTmmEnabled;
            AutorunEnabled = autorunEnabled;
            AutorunOnTorrentAddedEnabled = autorunOnTorrentAddedEnabled;
            AutorunOnTorrentAddedProgram = autorunOnTorrentAddedProgram;
            AutorunProgram = autorunProgram;
            DeleteTorrentContentFiles = deleteTorrentContentFiles;
            BannedIPs = bannedIPs;
            BdecodeDepthLimit = bdecodeDepthLimit;
            BdecodeTokenLimit = bdecodeTokenLimit;
            BittorrentProtocol = bittorrentProtocol;
            BlockPeersOnPrivilegedPorts = blockPeersOnPrivilegedPorts;
            BypassAuthSubnetWhitelist = bypassAuthSubnetWhitelist;
            BypassAuthSubnetWhitelistEnabled = bypassAuthSubnetWhitelistEnabled;
            BypassLocalAuth = bypassLocalAuth;
            CategoryChangedTmmEnabled = categoryChangedTmmEnabled;
            CheckingMemoryUse = checkingMemoryUse;
            ConnectionSpeed = connectionSpeed;
            CurrentInterfaceAddress = currentInterfaceAddress;
            CurrentInterfaceName = currentInterfaceName;
            CurrentNetworkInterface = currentNetworkInterface;
            Dht = dht;
            DhtBootstrapNodes = dhtBootstrapNodes;
            DiskCache = diskCache;
            DiskCacheTtl = diskCacheTtl;
            DiskIoReadMode = diskIoReadMode;
            DiskIoType = diskIoType;
            DiskIoWriteMode = diskIoWriteMode;
            DiskQueueSize = diskQueueSize;
            DlLimit = dlLimit;
            DontCountSlowTorrents = dontCountSlowTorrents;
            DyndnsDomain = dyndnsDomain;
            DyndnsEnabled = dyndnsEnabled;
            DyndnsPassword = dyndnsPassword;
            DyndnsService = dyndnsService;
            DyndnsUsername = dyndnsUsername;
            EmbeddedTrackerPort = embeddedTrackerPort;
            EmbeddedTrackerPortForwarding = embeddedTrackerPortForwarding;
            EnableCoalesceReadWrite = enableCoalesceReadWrite;
            EnableEmbeddedTracker = enableEmbeddedTracker;
            EnableMultiConnectionsFromSameIp = enableMultiConnectionsFromSameIp;
            EnablePieceExtentAffinity = enablePieceExtentAffinity;
            EnableUploadSuggestions = enableUploadSuggestions;
            Encryption = encryption;
            ExcludedFileNames = excludedFileNames;
            ExcludedFileNamesEnabled = excludedFileNamesEnabled;
            ExportDir = exportDir;
            ExportDirFin = exportDirFin;
            FileLogAge = fileLogAge;
            FileLogAgeType = fileLogAgeType;
            FileLogBackupEnabled = fileLogBackupEnabled;
            FileLogDeleteOld = fileLogDeleteOld;
            FileLogEnabled = fileLogEnabled;
            FileLogMaxSize = fileLogMaxSize;
            FileLogPath = fileLogPath;
            FilePoolSize = filePoolSize;
            HashingThreads = hashingThreads;
            I2pAddress = i2pAddress;
            I2pEnabled = i2pEnabled;
            I2pInboundLength = i2pInboundLength;
            I2pInboundQuantity = i2pInboundQuantity;
            I2pMixedMode = i2pMixedMode;
            I2pOutboundLength = i2pOutboundLength;
            I2pOutboundQuantity = i2pOutboundQuantity;
            I2pPort = i2pPort;
            IdnSupportEnabled = idnSupportEnabled;
            IncompleteFilesExt = incompleteFilesExt;
            UseUnwantedFolder = useUnwantedFolder;
            IpFilterEnabled = ipFilterEnabled;
            IpFilterPath = ipFilterPath;
            IpFilterTrackers = ipFilterTrackers;
            LimitLanPeers = limitLanPeers;
            LimitTcpOverhead = limitTcpOverhead;
            LimitUtpRate = limitUtpRate;
            ListenPort = listenPort;
            SslEnabled = sslEnabled;
            SslListenPort = sslListenPort;
            Locale = locale;
            Lsd = lsd;
            MailNotificationAuthEnabled = mailNotificationAuthEnabled;
            MailNotificationEmail = mailNotificationEmail;
            MailNotificationEnabled = mailNotificationEnabled;
            MailNotificationPassword = mailNotificationPassword;
            MailNotificationSender = mailNotificationSender;
            MailNotificationSmtp = mailNotificationSmtp;
            MailNotificationSslEnabled = mailNotificationSslEnabled;
            MailNotificationUsername = mailNotificationUsername;
            MarkOfTheWeb = markOfTheWeb;
            MaxActiveCheckingTorrents = maxActiveCheckingTorrents;
            MaxActiveDownloads = maxActiveDownloads;
            MaxActiveTorrents = maxActiveTorrents;
            MaxActiveUploads = maxActiveUploads;
            MaxConcurrentHttpAnnounces = maxConcurrentHttpAnnounces;
            MaxConnec = maxConnec;
            MaxConnecPerTorrent = maxConnecPerTorrent;
            MaxInactiveSeedingTime = maxInactiveSeedingTime;
            MaxInactiveSeedingTimeEnabled = maxInactiveSeedingTimeEnabled;
            MaxRatio = maxRatio;
            MaxRatioAct = maxRatioAct;
            MaxRatioEnabled = maxRatioEnabled;
            MaxSeedingTime = maxSeedingTime;
            MaxSeedingTimeEnabled = maxSeedingTimeEnabled;
            MaxUploads = maxUploads;
            MaxUploadsPerTorrent = maxUploadsPerTorrent;
            MemoryWorkingSetLimit = memoryWorkingSetLimit;
            MergeTrackers = mergeTrackers;
            OutgoingPortsMax = outgoingPortsMax;
            OutgoingPortsMin = outgoingPortsMin;
            PeerTos = peerTos;
            PeerTurnover = peerTurnover;
            PeerTurnoverCutoff = peerTurnoverCutoff;
            PeerTurnoverInterval = peerTurnoverInterval;
            PerformanceWarning = performanceWarning;
            Pex = pex;
            PreallocateAll = preallocateAll;
            ProxyAuthEnabled = proxyAuthEnabled;
            ProxyBittorrent = proxyBittorrent;
            ProxyHostnameLookup = proxyHostnameLookup;
            ProxyIp = proxyIp;
            ProxyMisc = proxyMisc;
            ProxyPassword = proxyPassword;
            ProxyPeerConnections = proxyPeerConnections;
            ProxyPort = proxyPort;
            ProxyRss = proxyRss;
            ProxyType = proxyType;
            ProxyUsername = proxyUsername;
            PythonExecutablePath = pythonExecutablePath;
            QueueingEnabled = queueingEnabled;
            RandomPort = randomPort;
            ReannounceWhenAddressChanged = reannounceWhenAddressChanged;
            RecheckCompletedTorrents = recheckCompletedTorrents;
            RefreshInterval = refreshInterval;
            RequestQueueSize = requestQueueSize;
            ResolvePeerHostNames = resolvePeerHostNames;
            ResolvePeerCountries = resolvePeerCountries;
            ResumeDataStorageType = resumeDataStorageType;
            RssAutoDownloadingEnabled = rssAutoDownloadingEnabled;
            RssDownloadRepackProperEpisodes = rssDownloadRepackProperEpisodes;
            RssFetchDelay = rssFetchDelay;
            RssMaxArticlesPerFeed = rssMaxArticlesPerFeed;
            RssProcessingEnabled = rssProcessingEnabled;
            RssRefreshInterval = rssRefreshInterval;
            RssSmartEpisodeFilters = rssSmartEpisodeFilters;
            SavePath = savePath;
            SavePathChangedTmmEnabled = savePathChangedTmmEnabled;
            SaveResumeDataInterval = saveResumeDataInterval;
            SaveStatisticsInterval = saveStatisticsInterval;
            ScanDirs = scanDirs;
            ScheduleFromHour = scheduleFromHour;
            ScheduleFromMin = scheduleFromMin;
            ScheduleToHour = scheduleToHour;
            ScheduleToMin = scheduleToMin;
            SchedulerDays = schedulerDays;
            SchedulerEnabled = schedulerEnabled;
            SendBufferLowWatermark = sendBufferLowWatermark;
            SendBufferWatermark = sendBufferWatermark;
            SendBufferWatermarkFactor = sendBufferWatermarkFactor;
            SlowTorrentDlRateThreshold = slowTorrentDlRateThreshold;
            SlowTorrentInactiveTimer = slowTorrentInactiveTimer;
            SlowTorrentUlRateThreshold = slowTorrentUlRateThreshold;
            SocketBacklogSize = socketBacklogSize;
            SocketReceiveBufferSize = socketReceiveBufferSize;
            SocketSendBufferSize = socketSendBufferSize;
            SsrfMitigation = ssrfMitigation;
            StopTrackerTimeout = stopTrackerTimeout;
            TempPath = tempPath;
            TempPathEnabled = tempPathEnabled;
            TorrentChangedTmmEnabled = torrentChangedTmmEnabled;
            TorrentContentLayout = torrentContentLayout;
            TorrentContentRemoveOption = torrentContentRemoveOption;
            TorrentFileSizeLimit = torrentFileSizeLimit;
            TorrentStopCondition = torrentStopCondition;
            UpLimit = upLimit;
            UploadChokingAlgorithm = uploadChokingAlgorithm;
            UploadSlotsBehavior = uploadSlotsBehavior;
            Upnp = upnp;
            UpnpLeaseDuration = upnpLeaseDuration;
            UseCategoryPathsInManualMode = useCategoryPathsInManualMode;
            UseHttps = useHttps;
            IgnoreSslErrors = ignoreSslErrors;
            UseSubcategories = useSubcategories;
            UtpTcpMixedMode = utpTcpMixedMode;
            ValidateHttpsTrackerCertificate = validateHttpsTrackerCertificate;
            HostnameCacheTtl = hostnameCacheTtl;
            WebUiAddress = webUiAddress;
            WebUiApiKey = webUiApiKey;
            WebUiBanDuration = webUiBanDuration;
            WebUiClickjackingProtectionEnabled = webUiClickjackingProtectionEnabled;
            WebUiCsrfProtectionEnabled = webUiCsrfProtectionEnabled;
            WebUiCustomHttpHeaders = webUiCustomHttpHeaders;
            WebUiDomainList = webUiDomainList;
            WebUiHostHeaderValidationEnabled = webUiHostHeaderValidationEnabled;
            WebUiHttpsCertPath = webUiHttpsCertPath;
            WebUiHttpsKeyPath = webUiHttpsKeyPath;
            WebUiMaxAuthFailCount = webUiMaxAuthFailCount;
            WebUiPort = webUiPort;
            WebUiReverseProxiesList = webUiReverseProxiesList;
            WebUiReverseProxyEnabled = webUiReverseProxyEnabled;
            WebUiSecureCookieEnabled = webUiSecureCookieEnabled;
            WebUiSessionTimeout = webUiSessionTimeout;
            WebUiUpnp = webUiUpnp;
            WebUiUseCustomHttpHeadersEnabled = webUiUseCustomHttpHeadersEnabled;
            WebUiUsername = webUiUsername;
            ConfirmTorrentDeletion = confirmTorrentDeletion;
            ConfirmTorrentRecheck = confirmTorrentRecheck;
            StatusBarExternalIp = statusBarExternalIp;
        }

        /// <summary>
        /// Gets a value indicating whether new torrents are added to the top of the queue.
        /// </summary>
        [JsonPropertyName("add_to_top_of_queue")]
        public bool AddToTopOfQueue { get; }

        /// <summary>
        /// Gets a value indicating whether new torrents are added in a stopped state.
        /// </summary>
        [JsonPropertyName("add_stopped_enabled")]
        public bool AddStoppedEnabled { get; }

        /// <summary>
        /// Gets the additional trackers.
        /// </summary>
        [JsonPropertyName("add_trackers")]
        public string AddTrackers { get; }

        /// <summary>
        /// Gets a value indicating whether additional trackers are appended automatically.
        /// </summary>
        [JsonPropertyName("add_trackers_enabled")]
        public bool AddTrackersEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether trackers are loaded from the configured URL.
        /// </summary>
        [JsonPropertyName("add_trackers_from_url_enabled")]
        public bool AddTrackersFromUrlEnabled { get; }

        /// <summary>
        /// Gets the additional tracker URL.
        /// </summary>
        [JsonPropertyName("add_trackers_url")]
        public string AddTrackersUrl { get; }

        /// <summary>
        /// Gets the additional tracker URL list.
        /// </summary>
        [JsonPropertyName("add_trackers_url_list")]
        public string AddTrackersUrlList { get; }

        /// <summary>
        /// Gets the alternative download limit in bytes per second.
        /// </summary>
        [JsonPropertyName("alt_dl_limit")]
        public int AltDlLimit { get; }

        /// <summary>
        /// Gets the alternative upload limit in bytes per second.
        /// </summary>
        [JsonPropertyName("alt_up_limit")]
        public int AltUpLimit { get; }

        /// <summary>
        /// Gets a value indicating whether the alternative Web UI is enabled.
        /// </summary>
        [JsonPropertyName("alternative_webui_enabled")]
        public bool AlternativeWebuiEnabled { get; }

        /// <summary>
        /// Gets the alternative Web UI path.
        /// </summary>
        [JsonPropertyName("alternative_webui_path")]
        public string AlternativeWebuiPath { get; }

        /// <summary>
        /// Gets the announce IP address.
        /// </summary>
        [JsonPropertyName("announce_ip")]
        public string AnnounceIp { get; }

        /// <summary>
        /// Gets the announce port number.
        /// </summary>
        [JsonPropertyName("announce_port")]
        public int AnnouncePort { get; }

        /// <summary>
        /// Gets a value indicating whether announces are sent to all tracker tiers.
        /// </summary>
        [JsonPropertyName("announce_to_all_tiers")]
        public bool AnnounceToAllTiers { get; }

        /// <summary>
        /// Gets a value indicating whether announces are sent to all trackers.
        /// </summary>
        [JsonPropertyName("announce_to_all_trackers")]
        public bool AnnounceToAllTrackers { get; }

        /// <summary>
        /// Gets a value indicating whether anonymous mode is enabled.
        /// </summary>
        [JsonPropertyName("anonymous_mode")]
        public bool AnonymousMode { get; }

        /// <summary>
        /// Gets the application instance name.
        /// </summary>
        [JsonPropertyName("app_instance_name")]
        public string AppInstanceName { get; }

        /// <summary>
        /// Gets the async I/O thread count.
        /// </summary>
        [JsonPropertyName("async_io_threads")]
        public int AsyncIoThreads { get; }

        /// <summary>
        /// Gets the torrent file auto-delete mode: <c>0</c> for never, <c>1</c> for if added, or <c>2</c> for always.
        /// </summary>
        [JsonPropertyName("auto_delete_mode")]
        public AutoDeleteMode AutoDeleteMode { get; }

        /// <summary>
        /// Gets a value indicating whether automatic torrent management is enabled.
        /// </summary>
        [JsonPropertyName("auto_tmm_enabled")]
        public bool AutoTmmEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether the autorun command is enabled.
        /// </summary>
        [JsonPropertyName("autorun_enabled")]
        public bool AutorunEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether the torrent-added autorun command is enabled.
        /// </summary>
        [JsonPropertyName("autorun_on_torrent_added_enabled")]
        public bool AutorunOnTorrentAddedEnabled { get; }

        /// <summary>
        /// Gets the autorun on torrent added program.
        /// </summary>
        [JsonPropertyName("autorun_on_torrent_added_program")]
        public string AutorunOnTorrentAddedProgram { get; }

        /// <summary>
        /// Gets the autorun program.
        /// </summary>
        [JsonPropertyName("autorun_program")]
        public string AutorunProgram { get; }

        /// <summary>
        /// Gets a value indicating whether torrent content files are deleted with the torrent.
        /// </summary>
        [JsonPropertyName("delete_torrent_content_files")]
        public bool DeleteTorrentContentFiles { get; }

        /// <summary>
        /// Gets the banned IP addresses.
        /// </summary>
        [JsonPropertyName("banned_IPs")]
        public string BannedIPs { get; }

        /// <summary>
        /// Gets the bdecode depth limit.
        /// </summary>
        [JsonPropertyName("bdecode_depth_limit")]
        public int BdecodeDepthLimit { get; }

        /// <summary>
        /// Gets the bdecode token limit.
        /// </summary>
        [JsonPropertyName("bdecode_token_limit")]
        public int BdecodeTokenLimit { get; }

        /// <summary>
        /// Gets the peer connection protocol mode: <c>0</c> for TCP and uTP, <c>1</c> for TCP only, or <c>2</c> for uTP only.
        /// </summary>
        [JsonPropertyName("bittorrent_protocol")]
        public BittorrentProtocol BittorrentProtocol { get; }

        /// <summary>
        /// Gets a value indicating whether peers on privileged ports are blocked.
        /// </summary>
        [JsonPropertyName("block_peers_on_privileged_ports")]
        public bool BlockPeersOnPrivilegedPorts { get; }

        /// <summary>
        /// Gets the bypass auth subnet whitelist.
        /// </summary>
        [JsonPropertyName("bypass_auth_subnet_whitelist")]
        public string BypassAuthSubnetWhitelist { get; }

        /// <summary>
        /// Gets a value indicating whether the authentication-bypass subnet whitelist is enabled.
        /// </summary>
        [JsonPropertyName("bypass_auth_subnet_whitelist_enabled")]
        public bool BypassAuthSubnetWhitelistEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether local clients bypass authentication.
        /// </summary>
        [JsonPropertyName("bypass_local_auth")]
        public bool BypassLocalAuth { get; }

        /// <summary>
        /// Gets a value indicating whether automatic torrent management reacts to category changes.
        /// </summary>
        [JsonPropertyName("category_changed_tmm_enabled")]
        public bool CategoryChangedTmmEnabled { get; }

        /// <summary>
        /// Gets the outstanding memory when checking torrents in MiB.
        /// </summary>
        [JsonPropertyName("checking_memory_use")]
        public int CheckingMemoryUse { get; }

        /// <summary>
        /// Gets the outgoing connection rate limit in connections per second.
        /// </summary>
        [JsonPropertyName("connection_speed")]
        public int ConnectionSpeed { get; }

        /// <summary>
        /// Gets the current interface address.
        /// </summary>
        [JsonPropertyName("current_interface_address")]
        public string CurrentInterfaceAddress { get; }

        /// <summary>
        /// Gets the current interface name.
        /// </summary>
        [JsonPropertyName("current_interface_name")]
        public string CurrentInterfaceName { get; }

        /// <summary>
        /// Gets the current network interface.
        /// </summary>
        [JsonPropertyName("current_network_interface")]
        public string CurrentNetworkInterface { get; }

        /// <summary>
        /// Gets a value indicating whether DHT is enabled.
        /// </summary>
        [JsonPropertyName("dht")]
        public bool Dht { get; }

        /// <summary>
        /// Gets the DHT bootstrap nodes.
        /// </summary>
        [JsonPropertyName("dht_bootstrap_nodes")]
        public string DhtBootstrapNodes { get; }

        /// <summary>
        /// Gets the disk cache size in MiB.
        /// </summary>
        [JsonPropertyName("disk_cache")]
        public int DiskCache { get; }

        /// <summary>
        /// Gets the disk cache expiry interval in seconds.
        /// </summary>
        [JsonPropertyName("disk_cache_ttl")]
        public int DiskCacheTtl { get; }

        /// <summary>
        /// Gets the disk I/O read mode: <c>0</c> for disable OS cache or <c>1</c> for enable OS cache.
        /// </summary>
        [JsonPropertyName("disk_io_read_mode")]
        public DiskIoReadMode DiskIoReadMode { get; }

        /// <summary>
        /// Gets the disk I/O type: <c>0</c> for default, <c>1</c> for memory mapped files, <c>2</c> for POSIX-compliant, or <c>3</c> for simple pread/pwrite.
        /// </summary>
        [JsonPropertyName("disk_io_type")]
        public DiskIoType DiskIoType { get; }

        /// <summary>
        /// Gets the disk I/O write mode: <c>0</c> for disable OS cache, <c>1</c> for enable OS cache, or <c>2</c> for write-through when supported by the upstream build.
        /// </summary>
        [JsonPropertyName("disk_io_write_mode")]
        public DiskIoWriteMode DiskIoWriteMode { get; }

        /// <summary>
        /// Gets the disk queue size in bytes.
        /// </summary>
        [JsonPropertyName("disk_queue_size")]
        public int DiskQueueSize { get; }

        /// <summary>
        /// Gets the download limit in bytes per second.
        /// </summary>
        [JsonPropertyName("dl_limit")]
        public int DlLimit { get; }

        /// <summary>
        /// Gets a value indicating whether slow torrents are excluded from queueing limits.
        /// </summary>
        [JsonPropertyName("dont_count_slow_torrents")]
        public bool DontCountSlowTorrents { get; }

        /// <summary>
        /// Gets the dyndns domain.
        /// </summary>
        [JsonPropertyName("dyndns_domain")]
        public string DyndnsDomain { get; }

        /// <summary>
        /// Gets a value indicating whether dynamic DNS updates are enabled.
        /// </summary>
        [JsonPropertyName("dyndns_enabled")]
        public bool DyndnsEnabled { get; }

        /// <summary>
        /// Gets the dyndns password.
        /// </summary>
        [JsonPropertyName("dyndns_password")]
        public string DyndnsPassword { get; }

        /// <summary>
        /// Gets the dynamic DNS service selector: <c>0</c> for DynDNS or <c>1</c> for No-IP.
        /// </summary>
        [JsonPropertyName("dyndns_service")]
        public DyndnsService DyndnsService { get; }

        /// <summary>
        /// Gets the dyndns username.
        /// </summary>
        [JsonPropertyName("dyndns_username")]
        public string DyndnsUsername { get; }

        /// <summary>
        /// Gets the embedded tracker port number.
        /// </summary>
        [JsonPropertyName("embedded_tracker_port")]
        public int EmbeddedTrackerPort { get; }

        /// <summary>
        /// Gets a value indicating whether the embedded tracker port is forwarded.
        /// </summary>
        [JsonPropertyName("embedded_tracker_port_forwarding")]
        public bool EmbeddedTrackerPortForwarding { get; }

        /// <summary>
        /// Gets a value indicating whether coalesced read and write operations are enabled.
        /// </summary>
        [JsonPropertyName("enable_coalesce_read_write")]
        public bool EnableCoalesceReadWrite { get; }

        /// <summary>
        /// Gets a value indicating whether the embedded tracker is enabled.
        /// </summary>
        [JsonPropertyName("enable_embedded_tracker")]
        public bool EnableEmbeddedTracker { get; }

        /// <summary>
        /// Gets a value indicating whether multiple connections from the same IP are allowed.
        /// </summary>
        [JsonPropertyName("enable_multi_connections_from_same_ip")]
        public bool EnableMultiConnectionsFromSameIp { get; }

        /// <summary>
        /// Gets a value indicating whether piece-extent affinity is enabled.
        /// </summary>
        [JsonPropertyName("enable_piece_extent_affinity")]
        public bool EnablePieceExtentAffinity { get; }

        /// <summary>
        /// Gets a value indicating whether upload suggestions are enabled.
        /// </summary>
        [JsonPropertyName("enable_upload_suggestions")]
        public bool EnableUploadSuggestions { get; }

        /// <summary>
        /// Gets the encryption mode: <c>0</c> for allow encryption, <c>1</c> for require encryption, or <c>2</c> for disable encryption.
        /// </summary>
        [JsonPropertyName("encryption")]
        public EncryptionMode Encryption { get; }

        /// <summary>
        /// Gets the excluded file names.
        /// </summary>
        [JsonPropertyName("excluded_file_names")]
        public string ExcludedFileNames { get; }

        /// <summary>
        /// Gets a value indicating whether excluded file-name filtering is enabled.
        /// </summary>
        [JsonPropertyName("excluded_file_names_enabled")]
        public bool ExcludedFileNamesEnabled { get; }

        /// <summary>
        /// Gets the export dir.
        /// </summary>
        [JsonPropertyName("export_dir")]
        public string ExportDir { get; }

        /// <summary>
        /// Gets the export dir fin.
        /// </summary>
        [JsonPropertyName("export_dir_fin")]
        public string ExportDirFin { get; }

        /// <summary>
        /// Gets the backup log retention age in the units selected by <see cref="FileLogAgeType" />.
        /// </summary>
        [JsonPropertyName("file_log_age")]
        public int FileLogAge { get; }

        /// <summary>
        /// Gets the backup log retention unit selector: <c>0</c> for days, <c>1</c> for months, or <c>2</c> for years.
        /// </summary>
        [JsonPropertyName("file_log_age_type")]
        public int FileLogAgeType { get; }

        /// <summary>
        /// Gets a value indicating whether file-log backups are enabled.
        /// </summary>
        [JsonPropertyName("file_log_backup_enabled")]
        public bool FileLogBackupEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether old log files are deleted.
        /// </summary>
        [JsonPropertyName("file_log_delete_old")]
        public bool FileLogDeleteOld { get; }

        /// <summary>
        /// Gets a value indicating whether file logging is enabled.
        /// </summary>
        [JsonPropertyName("file_log_enabled")]
        public bool FileLogEnabled { get; }

        /// <summary>
        /// Gets the log file backup threshold in KiB.
        /// </summary>
        [JsonPropertyName("file_log_max_size")]
        public int FileLogMaxSize { get; }

        /// <summary>
        /// Gets the file log path.
        /// </summary>
        [JsonPropertyName("file_log_path")]
        public string FileLogPath { get; }

        /// <summary>
        /// Gets the file pool size.
        /// </summary>
        [JsonPropertyName("file_pool_size")]
        public int FilePoolSize { get; }

        /// <summary>
        /// Gets the hashing threads.
        /// </summary>
        [JsonPropertyName("hashing_threads")]
        public int HashingThreads { get; }

        /// <summary>
        /// Gets the I2P address.
        /// </summary>
        [JsonPropertyName("i2p_address")]
        public string I2pAddress { get; }

        /// <summary>
        /// Gets a value indicating whether I2P is enabled.
        /// </summary>
        [JsonPropertyName("i2p_enabled")]
        public bool I2pEnabled { get; }

        /// <summary>
        /// Gets the I2P inbound length.
        /// </summary>
        [JsonPropertyName("i2p_inbound_length")]
        public int I2pInboundLength { get; }

        /// <summary>
        /// Gets the I2P inbound quantity.
        /// </summary>
        [JsonPropertyName("i2p_inbound_quantity")]
        public int I2pInboundQuantity { get; }

        /// <summary>
        /// Gets a value indicating whether I2P mixed mode is enabled.
        /// </summary>
        [JsonPropertyName("i2p_mixed_mode")]
        public bool I2pMixedMode { get; }

        /// <summary>
        /// Gets the I2P outbound length.
        /// </summary>
        [JsonPropertyName("i2p_outbound_length")]
        public int I2pOutboundLength { get; }

        /// <summary>
        /// Gets the I2P outbound quantity.
        /// </summary>
        [JsonPropertyName("i2p_outbound_quantity")]
        public int I2pOutboundQuantity { get; }

        /// <summary>
        /// Gets the I2P port number.
        /// </summary>
        [JsonPropertyName("i2p_port")]
        public int I2pPort { get; }

        /// <summary>
        /// Gets a value indicating whether idn support is enabled.
        /// </summary>
        [JsonPropertyName("idn_support_enabled")]
        public bool IdnSupportEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether the incomplete-files extension is used.
        /// </summary>
        [JsonPropertyName("incomplete_files_ext")]
        public bool IncompleteFilesExt { get; }

        /// <summary>
        /// Gets a value indicating whether unwanted folder is used.
        /// </summary>
        [JsonPropertyName("use_unwanted_folder")]
        public bool UseUnwantedFolder { get; }

        /// <summary>
        /// Gets a value indicating whether IP filtering is enabled.
        /// </summary>
        [JsonPropertyName("ip_filter_enabled")]
        public bool IpFilterEnabled { get; }

        /// <summary>
        /// Gets the IP filter path.
        /// </summary>
        [JsonPropertyName("ip_filter_path")]
        public string IpFilterPath { get; }

        /// <summary>
        /// Gets a value indicating whether tracker traffic is filtered through the IP filter.
        /// </summary>
        [JsonPropertyName("ip_filter_trackers")]
        public bool IpFilterTrackers { get; }

        /// <summary>
        /// Gets a value indicating whether LAN peers are excluded from transfer limits.
        /// </summary>
        [JsonPropertyName("limit_lan_peers")]
        public bool LimitLanPeers { get; }

        /// <summary>
        /// Gets a value indicating whether TCP overhead counts toward transfer limits.
        /// </summary>
        [JsonPropertyName("limit_tcp_overhead")]
        public bool LimitTcpOverhead { get; }

        /// <summary>
        /// Gets a value indicating whether uTP traffic counts toward transfer limits.
        /// </summary>
        [JsonPropertyName("limit_utp_rate")]
        public bool LimitUtpRate { get; }

        /// <summary>
        /// Gets the listen port number.
        /// </summary>
        [JsonPropertyName("listen_port")]
        public int ListenPort { get; }

        /// <summary>
        /// Gets a value indicating whether SSL is enabled.
        /// </summary>
        [JsonPropertyName("ssl_enabled")]
        public bool SslEnabled { get; }

        /// <summary>
        /// Gets the SSL listen port number.
        /// </summary>
        [JsonPropertyName("ssl_listen_port")]
        public int SslListenPort { get; }

        /// <summary>
        /// Gets the locale.
        /// </summary>
        [JsonPropertyName("locale")]
        public string Locale { get; }

        /// <summary>
        /// Gets a value indicating whether LSD is enabled.
        /// </summary>
        [JsonPropertyName("lsd")]
        public bool Lsd { get; }

        /// <summary>
        /// Gets a value indicating whether SMTP authentication is enabled.
        /// </summary>
        [JsonPropertyName("mail_notification_auth_enabled")]
        public bool MailNotificationAuthEnabled { get; }

        /// <summary>
        /// Gets the mail notification email.
        /// </summary>
        [JsonPropertyName("mail_notification_email")]
        public string MailNotificationEmail { get; }

        /// <summary>
        /// Gets a value indicating whether mail notifications are enabled.
        /// </summary>
        [JsonPropertyName("mail_notification_enabled")]
        public bool MailNotificationEnabled { get; }

        /// <summary>
        /// Gets the mail notification password.
        /// </summary>
        [JsonPropertyName("mail_notification_password")]
        public string MailNotificationPassword { get; }

        /// <summary>
        /// Gets the mail notification sender.
        /// </summary>
        [JsonPropertyName("mail_notification_sender")]
        public string MailNotificationSender { get; }

        /// <summary>
        /// Gets the mail notification SMTP server.
        /// </summary>
        [JsonPropertyName("mail_notification_smtp")]
        public string MailNotificationSmtp { get; }

        /// <summary>
        /// Gets a value indicating whether mail notification SSL is enabled.
        /// </summary>
        [JsonPropertyName("mail_notification_ssl_enabled")]
        public bool MailNotificationSslEnabled { get; }

        /// <summary>
        /// Gets the mail notification username.
        /// </summary>
        [JsonPropertyName("mail_notification_username")]
        public string MailNotificationUsername { get; }

        /// <summary>
        /// Gets a value indicating whether Mark of the Web is applied to downloaded files.
        /// </summary>
        [JsonPropertyName("mark_of_the_web")]
        public bool MarkOfTheWeb { get; }

        /// <summary>
        /// Gets the maximum active checking torrent count.
        /// </summary>
        [JsonPropertyName("max_active_checking_torrents")]
        public int MaxActiveCheckingTorrents { get; }

        /// <summary>
        /// Gets the maximum active download count.
        /// </summary>
        [JsonPropertyName("max_active_downloads")]
        public int MaxActiveDownloads { get; }

        /// <summary>
        /// Gets the maximum active torrent count.
        /// </summary>
        [JsonPropertyName("max_active_torrents")]
        public int MaxActiveTorrents { get; }

        /// <summary>
        /// Gets the maximum active upload count.
        /// </summary>
        [JsonPropertyName("max_active_uploads")]
        public int MaxActiveUploads { get; }

        /// <summary>
        /// Gets the maximum concurrent HTTP announce count.
        /// </summary>
        [JsonPropertyName("max_concurrent_http_announces")]
        public int MaxConcurrentHttpAnnounces { get; }

        /// <summary>
        /// Gets the maximum connection count.
        /// </summary>
        [JsonPropertyName("max_connec")]
        public int MaxConnec { get; }

        /// <summary>
        /// Gets the maximum connection count per torrent.
        /// </summary>
        [JsonPropertyName("max_connec_per_torrent")]
        public int MaxConnecPerTorrent { get; }

        /// <summary>
        /// Gets the max inactive seeding time in minutes.
        /// </summary>
        [JsonPropertyName("max_inactive_seeding_time")]
        public int MaxInactiveSeedingTime { get; }

        /// <summary>
        /// Gets a value indicating whether the maximum inactive seeding time is enabled.
        /// </summary>
        [JsonPropertyName("max_inactive_seeding_time_enabled")]
        public bool MaxInactiveSeedingTimeEnabled { get; }

        /// <summary>
        /// Gets the max ratio as a unitless ratio value.
        /// </summary>
        [JsonPropertyName("max_ratio")]
        public double MaxRatio { get; }

        /// <summary>
        /// Gets the share-limit action: <c>0</c> for stop torrent, <c>1</c> for remove torrent, <c>2</c> for enable super seeding, or <c>3</c> for remove torrent and its files.
        /// </summary>
        [JsonPropertyName("max_ratio_act")]
        public MaxRatioAction MaxRatioAct { get; }

        /// <summary>
        /// Gets a value indicating whether the maximum ratio is enabled.
        /// </summary>
        [JsonPropertyName("max_ratio_enabled")]
        public bool MaxRatioEnabled { get; }

        /// <summary>
        /// Gets the max seeding time in minutes.
        /// </summary>
        [JsonPropertyName("max_seeding_time")]
        public int MaxSeedingTime { get; }

        /// <summary>
        /// Gets a value indicating whether the maximum seeding time is enabled.
        /// </summary>
        [JsonPropertyName("max_seeding_time_enabled")]
        public bool MaxSeedingTimeEnabled { get; }

        /// <summary>
        /// Gets the maximum upload slot count.
        /// </summary>
        [JsonPropertyName("max_uploads")]
        public int MaxUploads { get; }

        /// <summary>
        /// Gets the maximum upload slot count per torrent.
        /// </summary>
        [JsonPropertyName("max_uploads_per_torrent")]
        public int MaxUploadsPerTorrent { get; }

        /// <summary>
        /// Gets the memory working set limit in MiB.
        /// </summary>
        [JsonPropertyName("memory_working_set_limit")]
        public int MemoryWorkingSetLimit { get; }

        /// <summary>
        /// Gets a value indicating whether trackers from multiple sources are merged.
        /// </summary>
        [JsonPropertyName("merge_trackers")]
        public bool MergeTrackers { get; }

        /// <summary>
        /// Gets the maximum outgoing port number.
        /// </summary>
        [JsonPropertyName("outgoing_ports_max")]
        public int OutgoingPortsMax { get; }

        /// <summary>
        /// Gets the minimum outgoing port number.
        /// </summary>
        [JsonPropertyName("outgoing_ports_min")]
        public int OutgoingPortsMin { get; }

        /// <summary>
        /// Gets the peer Type of Service value.
        /// </summary>
        [JsonPropertyName("peer_tos")]
        public int PeerTos { get; }

        /// <summary>
        /// Gets the peer turnover disconnect percentage.
        /// </summary>
        [JsonPropertyName("peer_turnover")]
        public int PeerTurnover { get; }

        /// <summary>
        /// Gets the peer turnover threshold percentage.
        /// </summary>
        [JsonPropertyName("peer_turnover_cutoff")]
        public int PeerTurnoverCutoff { get; }

        /// <summary>
        /// Gets the peer turnover disconnect interval in seconds.
        /// </summary>
        [JsonPropertyName("peer_turnover_interval")]
        public int PeerTurnoverInterval { get; }

        /// <summary>
        /// Gets a value indicating whether performance warnings are enabled.
        /// </summary>
        [JsonPropertyName("performance_warning")]
        public bool PerformanceWarning { get; }

        /// <summary>
        /// Gets a value indicating whether PEX is enabled.
        /// </summary>
        [JsonPropertyName("pex")]
        public bool Pex { get; }

        /// <summary>
        /// Gets a value indicating whether all files are preallocated.
        /// </summary>
        [JsonPropertyName("preallocate_all")]
        public bool PreallocateAll { get; }

        /// <summary>
        /// Gets a value indicating whether proxy auth is enabled.
        /// </summary>
        [JsonPropertyName("proxy_auth_enabled")]
        public bool ProxyAuthEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether BitTorrent traffic uses the proxy.
        /// </summary>
        [JsonPropertyName("proxy_bittorrent")]
        public bool ProxyBittorrent { get; }

        /// <summary>
        /// Gets a value indicating whether hostname lookups use the proxy.
        /// </summary>
        [JsonPropertyName("proxy_hostname_lookup")]
        public bool ProxyHostnameLookup { get; }

        /// <summary>
        /// Gets the proxy IP address.
        /// </summary>
        [JsonPropertyName("proxy_ip")]
        public string ProxyIp { get; }

        /// <summary>
        /// Gets a value indicating whether miscellaneous traffic uses the proxy.
        /// </summary>
        [JsonPropertyName("proxy_misc")]
        public bool ProxyMisc { get; }

        /// <summary>
        /// Gets the proxy password.
        /// </summary>
        [JsonPropertyName("proxy_password")]
        public string ProxyPassword { get; }

        /// <summary>
        /// Gets a value indicating whether peer connections use the proxy.
        /// </summary>
        [JsonPropertyName("proxy_peer_connections")]
        public bool ProxyPeerConnections { get; }

        /// <summary>
        /// Gets the proxy port number.
        /// </summary>
        [JsonPropertyName("proxy_port")]
        public int ProxyPort { get; }

        /// <summary>
        /// Gets a value indicating whether RSS traffic uses the proxy.
        /// </summary>
        [JsonPropertyName("proxy_rss")]
        public bool ProxyRss { get; }

        /// <summary>
        /// Gets the proxy type.
        /// </summary>
        [JsonPropertyName("proxy_type")]
        public ProxyType ProxyType { get; }

        /// <summary>
        /// Gets the proxy username.
        /// </summary>
        [JsonPropertyName("proxy_username")]
        public string ProxyUsername { get; }

        /// <summary>
        /// Gets the python executable path.
        /// </summary>
        [JsonPropertyName("python_executable_path")]
        public string PythonExecutablePath { get; }

        /// <summary>
        /// Gets a value indicating whether queueing is enabled.
        /// </summary>
        [JsonPropertyName("queueing_enabled")]
        public bool QueueingEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether a random listening port is used.
        /// </summary>
        [JsonPropertyName("random_port")]
        public bool RandomPort { get; }

        /// <summary>
        /// Gets a value indicating whether torrents are reannounced when the address changes.
        /// </summary>
        [JsonPropertyName("reannounce_when_address_changed")]
        public bool ReannounceWhenAddressChanged { get; }

        /// <summary>
        /// Gets a value indicating whether completed torrents are rechecked.
        /// </summary>
        [JsonPropertyName("recheck_completed_torrents")]
        public bool RecheckCompletedTorrents { get; }

        /// <summary>
        /// Gets the refresh interval in milliseconds.
        /// </summary>
        [JsonPropertyName("refresh_interval")]
        public int RefreshInterval { get; }

        /// <summary>
        /// Gets the maximum outstanding request count to a single peer.
        /// </summary>
        [JsonPropertyName("request_queue_size")]
        public int RequestQueueSize { get; }

        /// <summary>
        /// Gets a value indicating whether reverse DNS lookup of peer host names is enabled when returned by the server.
        /// </summary>
        [JsonPropertyName("resolve_peer_host_names")]
        public bool? ResolvePeerHostNames { get; }

        /// <summary>
        /// Gets a value indicating whether peer countries are resolved.
        /// </summary>
        [JsonPropertyName("resolve_peer_countries")]
        public bool ResolvePeerCountries { get; }

        /// <summary>
        /// Gets the resume data storage type.
        /// </summary>
        [JsonPropertyName("resume_data_storage_type")]
        public ResumeDataStorageType ResumeDataStorageType { get; }

        /// <summary>
        /// Gets a value indicating whether RSS auto downloading is enabled.
        /// </summary>
        [JsonPropertyName("rss_auto_downloading_enabled")]
        public bool RssAutoDownloadingEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether RSS repack and proper episodes are downloaded.
        /// </summary>
        [JsonPropertyName("rss_download_repack_proper_episodes")]
        public bool RssDownloadRepackProperEpisodes { get; }

        /// <summary>
        /// Gets the same-host RSS request delay in seconds.
        /// </summary>
        [JsonPropertyName("rss_fetch_delay")]
        public long RssFetchDelay { get; }

        /// <summary>
        /// Gets the RSS max articles per feed.
        /// </summary>
        [JsonPropertyName("rss_max_articles_per_feed")]
        public int RssMaxArticlesPerFeed { get; }

        /// <summary>
        /// Gets a value indicating whether RSS processing is enabled.
        /// </summary>
        [JsonPropertyName("rss_processing_enabled")]
        public bool RssProcessingEnabled { get; }

        /// <summary>
        /// Gets the RSS refresh interval in minutes.
        /// </summary>
        [JsonPropertyName("rss_refresh_interval")]
        public int RssRefreshInterval { get; }

        /// <summary>
        /// Gets the RSS smart episode filters.
        /// </summary>
        [JsonPropertyName("rss_smart_episode_filters")]
        public string RssSmartEpisodeFilters { get; }

        /// <summary>
        /// Gets the save path.
        /// </summary>
        [JsonPropertyName("save_path")]
        public string SavePath { get; }

        /// <summary>
        /// Gets a value indicating whether save path changed TMM is enabled.
        /// </summary>
        [JsonPropertyName("save_path_changed_tmm_enabled")]
        public bool SavePathChangedTmmEnabled { get; }

        /// <summary>
        /// Gets the save resume data interval in minutes.
        /// </summary>
        [JsonPropertyName("save_resume_data_interval")]
        public int SaveResumeDataInterval { get; }

        /// <summary>
        /// Gets the save statistics interval in minutes.
        /// </summary>
        [JsonPropertyName("save_statistics_interval")]
        public int SaveStatisticsInterval { get; }

        /// <summary>
        /// Gets the monitored scan directories.
        /// </summary>
        [JsonPropertyName("scan_dirs")]
        public Dictionary<string, SaveLocation> ScanDirs { get; }

        /// <summary>
        /// Gets the scheduled start hour in 24-hour time.
        /// </summary>
        [JsonPropertyName("schedule_from_hour")]
        public int ScheduleFromHour { get; }

        /// <summary>
        /// Gets the scheduled start minute.
        /// </summary>
        [JsonPropertyName("schedule_from_min")]
        public int ScheduleFromMin { get; }

        /// <summary>
        /// Gets the scheduled end hour in 24-hour time.
        /// </summary>
        [JsonPropertyName("schedule_to_hour")]
        public int ScheduleToHour { get; }

        /// <summary>
        /// Gets the scheduled end minute.
        /// </summary>
        [JsonPropertyName("schedule_to_min")]
        public int ScheduleToMin { get; }

        /// <summary>
        /// Gets the scheduler day selection: <c>0</c> for every day, <c>1</c> for weekdays, <c>2</c> for weekends, <c>3</c> for Monday, <c>4</c> for Tuesday, <c>5</c> for Wednesday, <c>6</c> for Thursday, <c>7</c> for Friday, <c>8</c> for Saturday, or <c>9</c> for Sunday.
        /// </summary>
        [JsonPropertyName("scheduler_days")]
        public SchedulerDays SchedulerDays { get; }

        /// <summary>
        /// Gets a value indicating whether scheduled alternative speed limits are enabled.
        /// </summary>
        [JsonPropertyName("scheduler_enabled")]
        public bool SchedulerEnabled { get; }

        /// <summary>
        /// Gets the send buffer low watermark in KiB.
        /// </summary>
        [JsonPropertyName("send_buffer_low_watermark")]
        public int SendBufferLowWatermark { get; }

        /// <summary>
        /// Gets the send buffer watermark in KiB.
        /// </summary>
        [JsonPropertyName("send_buffer_watermark")]
        public int SendBufferWatermark { get; }

        /// <summary>
        /// Gets the send buffer watermark factor as a percentage.
        /// </summary>
        [JsonPropertyName("send_buffer_watermark_factor")]
        public int SendBufferWatermarkFactor { get; }

        /// <summary>
        /// Gets the slow torrent download rate threshold in KiB/s.
        /// </summary>
        [JsonPropertyName("slow_torrent_dl_rate_threshold")]
        public int SlowTorrentDlRateThreshold { get; }

        /// <summary>
        /// Gets the slow torrent inactivity timer in seconds.
        /// </summary>
        [JsonPropertyName("slow_torrent_inactive_timer")]
        public int SlowTorrentInactiveTimer { get; }

        /// <summary>
        /// Gets the slow torrent upload rate threshold in KiB/s.
        /// </summary>
        [JsonPropertyName("slow_torrent_ul_rate_threshold")]
        public int SlowTorrentUlRateThreshold { get; }

        /// <summary>
        /// Gets the socket backlog size as a connection count.
        /// </summary>
        [JsonPropertyName("socket_backlog_size")]
        public int SocketBacklogSize { get; }

        /// <summary>
        /// Gets the socket receive buffer size in bytes. A value of <c>0</c> uses the system default.
        /// </summary>
        [JsonPropertyName("socket_receive_buffer_size")]
        public int SocketReceiveBufferSize { get; }

        /// <summary>
        /// Gets the socket send buffer size in bytes. A value of <c>0</c> uses the system default.
        /// </summary>
        [JsonPropertyName("socket_send_buffer_size")]
        public int SocketSendBufferSize { get; }

        /// <summary>
        /// Gets a value indicating whether SSRF mitigation is enabled.
        /// </summary>
        [JsonPropertyName("ssrf_mitigation")]
        public bool SsrfMitigation { get; }

        /// <summary>
        /// Gets the stop tracker timeout in seconds.
        /// </summary>
        [JsonPropertyName("stop_tracker_timeout")]
        public int StopTrackerTimeout { get; }

        /// <summary>
        /// Gets the temp path.
        /// </summary>
        [JsonPropertyName("temp_path")]
        public string TempPath { get; }

        /// <summary>
        /// Gets a value indicating whether the temporary path is enabled.
        /// </summary>
        [JsonPropertyName("temp_path_enabled")]
        public bool TempPathEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether automatic torrent management reacts to torrent changes.
        /// </summary>
        [JsonPropertyName("torrent_changed_tmm_enabled")]
        public bool TorrentChangedTmmEnabled { get; }

        /// <summary>
        /// Gets the torrent content layout.
        /// </summary>
        [JsonPropertyName("torrent_content_layout")]
        public TorrentContentLayout TorrentContentLayout { get; }

        /// <summary>
        /// Gets the torrent content remove option.
        /// </summary>
        [JsonPropertyName("torrent_content_remove_option")]
        public TorrentContentRemoveOption TorrentContentRemoveOption { get; }

        /// <summary>
        /// Gets the torrent file size limit in bytes.
        /// </summary>
        [JsonPropertyName("torrent_file_size_limit")]
        public int TorrentFileSizeLimit { get; }

        /// <summary>
        /// Gets the torrent stop condition.
        /// </summary>
        [JsonPropertyName("torrent_stop_condition")]
        public StopCondition TorrentStopCondition { get; }

        /// <summary>
        /// Gets the upload limit in bytes per second.
        /// </summary>
        [JsonPropertyName("up_limit")]
        public int UpLimit { get; }

        /// <summary>
        /// Gets the upload choking algorithm: <c>0</c> for round-robin, <c>1</c> for fastest upload, or <c>2</c> for anti-leech.
        /// </summary>
        [JsonPropertyName("upload_choking_algorithm")]
        public UploadChokingAlgorithm UploadChokingAlgorithm { get; }

        /// <summary>
        /// Gets the upload slots behavior: <c>0</c> for fixed slots or <c>1</c> for upload-rate-based slots.
        /// </summary>
        [JsonPropertyName("upload_slots_behavior")]
        public UploadSlotsBehavior UploadSlotsBehavior { get; }

        /// <summary>
        /// Gets a value indicating whether UPnP is enabled.
        /// </summary>
        [JsonPropertyName("upnp")]
        public bool Upnp { get; }

        /// <summary>
        /// Gets the UPnP lease duration in seconds. A value of <c>0</c> requests a permanent lease.
        /// </summary>
        [JsonPropertyName("upnp_lease_duration")]
        public int UpnpLeaseDuration { get; }

        /// <summary>
        /// Gets a value indicating whether category paths are used in manual mode.
        /// </summary>
        [JsonPropertyName("use_category_paths_in_manual_mode")]
        public bool UseCategoryPathsInManualMode { get; }

        /// <summary>
        /// Gets a value indicating whether HTTPS is enabled.
        /// </summary>
        [JsonPropertyName("use_https")]
        public bool UseHttps { get; }

        /// <summary>
        /// Gets a value indicating whether SSL certificate errors are ignored.
        /// </summary>
        [JsonPropertyName("ignore_ssl_errors")]
        public bool IgnoreSslErrors { get; }

        /// <summary>
        /// Gets a value indicating whether subcategories are used.
        /// </summary>
        [JsonPropertyName("use_subcategories")]
        public bool? UseSubcategories { get; }

        /// <summary>
        /// Gets the uTP/TCP mixed mode algorithm: <c>0</c> for prefer TCP or <c>1</c> for peer proportional.
        /// </summary>
        [JsonPropertyName("utp_tcp_mixed_mode")]
        public UtpTcpMixedMode UtpTcpMixedMode { get; }

        /// <summary>
        /// Gets a value indicating whether HTTPS tracker certificates are validated.
        /// </summary>
        [JsonPropertyName("validate_https_tracker_certificate")]
        public bool ValidateHttpsTrackerCertificate { get; }

        /// <summary>
        /// Gets the hostname resolver cache TTL in seconds when returned by the server.
        /// </summary>
        [JsonPropertyName("hostname_cache_ttl")]
        public int? HostnameCacheTtl { get; }

        /// <summary>
        /// Gets the Web UI address.
        /// </summary>
        [JsonPropertyName("web_ui_address")]
        public string WebUiAddress { get; }

        /// <summary>
        /// Gets the Web UI API key.
        /// </summary>
        [JsonPropertyName("web_ui_api_key")]
        public string WebUiApiKey { get; }

        /// <summary>
        /// Gets the Web UI ban duration in seconds.
        /// </summary>
        [JsonPropertyName("web_ui_ban_duration")]
        public int WebUiBanDuration { get; }

        /// <summary>
        /// Gets a value indicating whether Web UI clickjacking protection is enabled.
        /// </summary>
        [JsonPropertyName("web_ui_clickjacking_protection_enabled")]
        public bool WebUiClickjackingProtectionEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether Web UI CSRF protection is enabled.
        /// </summary>
        [JsonPropertyName("web_ui_csrf_protection_enabled")]
        public bool WebUiCsrfProtectionEnabled { get; }

        /// <summary>
        /// Gets the Web UI custom HTTP headers.
        /// </summary>
        [JsonPropertyName("web_ui_custom_http_headers")]
        public string WebUiCustomHttpHeaders { get; }

        /// <summary>
        /// Gets the Web UI domain list.
        /// </summary>
        [JsonPropertyName("web_ui_domain_list")]
        public string WebUiDomainList { get; }

        /// <summary>
        /// Gets a value indicating whether Web UI host-header validation is enabled.
        /// </summary>
        [JsonPropertyName("web_ui_host_header_validation_enabled")]
        public bool WebUiHostHeaderValidationEnabled { get; }

        /// <summary>
        /// Gets the Web UI HTTPS cert path.
        /// </summary>
        [JsonPropertyName("web_ui_https_cert_path")]
        public string WebUiHttpsCertPath { get; }

        /// <summary>
        /// Gets the Web UI HTTPS key path.
        /// </summary>
        [JsonPropertyName("web_ui_https_key_path")]
        public string WebUiHttpsKeyPath { get; }

        /// <summary>
        /// Gets the maximum failed Web UI authentication attempt count before a ban is applied.
        /// </summary>
        [JsonPropertyName("web_ui_max_auth_fail_count")]
        public int WebUiMaxAuthFailCount { get; }

        /// <summary>
        /// Gets the Web UI port number.
        /// </summary>
        [JsonPropertyName("web_ui_port")]
        public int WebUiPort { get; }

        /// <summary>
        /// Gets the Web UI reverse proxies list.
        /// </summary>
        [JsonPropertyName("web_ui_reverse_proxies_list")]
        public string WebUiReverseProxiesList { get; }

        /// <summary>
        /// Gets a value indicating whether Web UI reverse-proxy support is enabled.
        /// </summary>
        [JsonPropertyName("web_ui_reverse_proxy_enabled")]
        public bool WebUiReverseProxyEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether Web UI secure cookies are enabled.
        /// </summary>
        [JsonPropertyName("web_ui_secure_cookie_enabled")]
        public bool WebUiSecureCookieEnabled { get; }

        /// <summary>
        /// Gets the Web UI session timeout in seconds.
        /// </summary>
        [JsonPropertyName("web_ui_session_timeout")]
        public int WebUiSessionTimeout { get; }

        /// <summary>
        /// Gets a value indicating whether UPnP is enabled for the Web UI.
        /// </summary>
        [JsonPropertyName("web_ui_upnp")]
        public bool WebUiUpnp { get; }

        /// <summary>
        /// Gets a value indicating whether custom Web UI HTTP headers are enabled.
        /// </summary>
        [JsonPropertyName("web_ui_use_custom_http_headers_enabled")]
        public bool WebUiUseCustomHttpHeadersEnabled { get; }

        /// <summary>
        /// Gets the Web UI username.
        /// </summary>
        [JsonPropertyName("web_ui_username")]
        public string WebUiUsername { get; }

        /// <summary>
        /// Gets a value indicating whether torrent deletion requires confirmation.
        /// </summary>
        [JsonPropertyName("confirm_torrent_deletion")]
        public bool ConfirmTorrentDeletion { get; }

        /// <summary>
        /// Gets a value indicating whether torrent rechecks require confirmation.
        /// </summary>
        [JsonPropertyName("confirm_torrent_recheck")]
        public bool ConfirmTorrentRecheck { get; }

        /// <summary>
        /// Gets a value indicating whether the status bar displays the external IP address.
        /// </summary>
        [JsonPropertyName("status_bar_external_ip")]
        public bool StatusBarExternalIp { get; }
    }
}
