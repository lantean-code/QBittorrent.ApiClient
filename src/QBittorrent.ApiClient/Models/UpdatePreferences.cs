using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents preference changes to apply to qBittorrent.
    /// </summary>
    public record UpdatePreferences
    {
        /// <summary>
        /// Gets or sets a value indicating whether new torrents are added to the top of the queue.
        /// </summary>
        [JsonPropertyName("add_to_top_of_queue")]
        public bool? AddToTopOfQueue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether new torrents are added in a stopped state.
        /// </summary>
        [JsonPropertyName("add_stopped_enabled")]
        public bool? AddStoppedEnabled { get; set; }

        /// <summary>
        /// Gets or sets the additional trackers.
        /// </summary>
        [JsonPropertyName("add_trackers")]
        public string? AddTrackers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether additional trackers are appended automatically.
        /// </summary>
        [JsonPropertyName("add_trackers_enabled")]
        public bool? AddTrackersEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether trackers are loaded from the configured URL.
        /// </summary>
        [JsonPropertyName("add_trackers_from_url_enabled")]
        public bool? AddTrackersFromUrlEnabled { get; set; }

        /// <summary>
        /// Gets or sets the additional tracker URL.
        /// </summary>
        [JsonPropertyName("add_trackers_url")]
        public string? AddTrackersUrl { get; set; }

        /// <summary>
        /// Gets or sets the additional tracker URL list.
        /// </summary>
        [JsonPropertyName("add_trackers_url_list")]
        public string? AddTrackersUrlList { get; set; }

        /// <summary>
        /// Gets or sets the alternative download limit in bytes per second.
        /// </summary>
        [JsonPropertyName("alt_dl_limit")]
        public int? AltDlLimit { get; set; }

        /// <summary>
        /// Gets or sets the alternative upload limit in bytes per second.
        /// </summary>
        [JsonPropertyName("alt_up_limit")]
        public int? AltUpLimit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the alternative Web UI is enabled.
        /// </summary>
        [JsonPropertyName("alternative_webui_enabled")]
        public bool? AlternativeWebuiEnabled { get; set; }

        /// <summary>
        /// Gets or sets the alternative Web UI path.
        /// </summary>
        [JsonPropertyName("alternative_webui_path")]
        public string? AlternativeWebuiPath { get; set; }

        /// <summary>
        /// Gets or sets the announce IP address.
        /// </summary>
        [JsonPropertyName("announce_ip")]
        public string? AnnounceIp { get; set; }

        /// <summary>
        /// Gets or sets the announce port number.
        /// </summary>
        [JsonPropertyName("announce_port")]
        public int? AnnouncePort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether announces are sent to all tracker tiers.
        /// </summary>
        [JsonPropertyName("announce_to_all_tiers")]
        public bool? AnnounceToAllTiers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether announces are sent to all trackers.
        /// </summary>
        [JsonPropertyName("announce_to_all_trackers")]
        public bool? AnnounceToAllTrackers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether anonymous mode is enabled.
        /// </summary>
        [JsonPropertyName("anonymous_mode")]
        public bool? AnonymousMode { get; set; }

        /// <summary>
        /// Gets or sets the application instance name.
        /// </summary>
        [JsonPropertyName("app_instance_name")]
        public string? AppInstanceName { get; set; }

        /// <summary>
        /// Gets or sets the async I/O thread count.
        /// </summary>
        [JsonPropertyName("async_io_threads")]
        public int? AsyncIoThreads { get; set; }

        /// <summary>
        /// Gets or sets the torrent file auto-delete mode: <c>0</c> for never, <c>1</c> for if added, or <c>2</c> for always.
        /// </summary>
        [JsonPropertyName("auto_delete_mode")]
        public AutoDeleteMode? AutoDeleteMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether automatic torrent management is enabled.
        /// </summary>
        [JsonPropertyName("auto_tmm_enabled")]
        public bool? AutoTmmEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the autorun command is enabled.
        /// </summary>
        [JsonPropertyName("autorun_enabled")]
        public bool? AutorunEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the torrent-added autorun command is enabled.
        /// </summary>
        [JsonPropertyName("autorun_on_torrent_added_enabled")]
        public bool? AutorunOnTorrentAddedEnabled { get; set; }

        /// <summary>
        /// Gets or sets the autorun on torrent added program.
        /// </summary>
        [JsonPropertyName("autorun_on_torrent_added_program")]
        public string? AutorunOnTorrentAddedProgram { get; set; }

        /// <summary>
        /// Gets or sets the autorun program.
        /// </summary>
        [JsonPropertyName("autorun_program")]
        public string? AutorunProgram { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether torrent content files are deleted with the torrent.
        /// </summary>
        [JsonPropertyName("delete_torrent_content_files")]
        public bool? DeleteTorrentContentFiles { get; set; }

        /// <summary>
        /// Gets or sets the banned IP addresses.
        /// </summary>
        [JsonPropertyName("banned_IPs")]
        public string? BannedIPs { get; set; }

        /// <summary>
        /// Gets or sets the bdecode depth limit.
        /// </summary>
        [JsonPropertyName("bdecode_depth_limit")]
        public int? BdecodeDepthLimit { get; set; }

        /// <summary>
        /// Gets or sets the bdecode token limit.
        /// </summary>
        [JsonPropertyName("bdecode_token_limit")]
        public int? BdecodeTokenLimit { get; set; }

        /// <summary>
        /// Gets or sets the peer connection protocol mode: <c>0</c> for TCP and uTP, <c>1</c> for TCP only, or <c>2</c> for uTP only.
        /// </summary>
        [JsonPropertyName("bittorrent_protocol")]
        public BittorrentProtocol? BittorrentProtocol { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether peers on privileged ports are blocked.
        /// </summary>
        [JsonPropertyName("block_peers_on_privileged_ports")]
        public bool? BlockPeersOnPrivilegedPorts { get; set; }

        /// <summary>
        /// Gets or sets the bypass auth subnet whitelist.
        /// </summary>
        [JsonPropertyName("bypass_auth_subnet_whitelist")]
        public string? BypassAuthSubnetWhitelist { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the authentication-bypass subnet whitelist is enabled.
        /// </summary>
        [JsonPropertyName("bypass_auth_subnet_whitelist_enabled")]
        public bool? BypassAuthSubnetWhitelistEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether local clients bypass authentication.
        /// </summary>
        [JsonPropertyName("bypass_local_auth")]
        public bool? BypassLocalAuth { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether automatic torrent management reacts to category changes.
        /// </summary>
        [JsonPropertyName("category_changed_tmm_enabled")]
        public bool? CategoryChangedTmmEnabled { get; set; }

        /// <summary>
        /// Gets or sets the outstanding memory when checking torrents in MiB.
        /// </summary>
        [JsonPropertyName("checking_memory_use")]
        public int? CheckingMemoryUse { get; set; }

        /// <summary>
        /// Gets or sets the outgoing connection rate limit in connections per second.
        /// </summary>
        [JsonPropertyName("connection_speed")]
        public int? ConnectionSpeed { get; set; }

        /// <summary>
        /// Gets or sets the current interface address.
        /// </summary>
        [JsonPropertyName("current_interface_address")]
        public string? CurrentInterfaceAddress { get; set; }

        /// <summary>
        /// Gets or sets the current interface name.
        /// </summary>
        [JsonPropertyName("current_interface_name")]
        public string? CurrentInterfaceName { get; set; }

        /// <summary>
        /// Gets or sets the current network interface.
        /// </summary>
        [JsonPropertyName("current_network_interface")]
        public string? CurrentNetworkInterface { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether DHT is enabled.
        /// </summary>
        [JsonPropertyName("dht")]
        public bool? Dht { get; set; }

        /// <summary>
        /// Gets or sets the DHT bootstrap nodes.
        /// </summary>
        [JsonPropertyName("dht_bootstrap_nodes")]
        public string? DhtBootstrapNodes { get; set; }

        /// <summary>
        /// Gets or sets the disk cache size in MiB.
        /// </summary>
        [JsonPropertyName("disk_cache")]
        public int? DiskCache { get; set; }

        /// <summary>
        /// Gets or sets the disk cache expiry interval in seconds.
        /// </summary>
        [JsonPropertyName("disk_cache_ttl")]
        public int? DiskCacheTtl { get; set; }

        /// <summary>
        /// Gets or sets the disk I/O read mode: <c>0</c> for disable OS cache or <c>1</c> for enable OS cache.
        /// </summary>
        [JsonPropertyName("disk_io_read_mode")]
        public DiskIoReadMode? DiskIoReadMode { get; set; }

        /// <summary>
        /// Gets or sets the disk I/O type: <c>0</c> for default, <c>1</c> for memory mapped files, <c>2</c> for POSIX-compliant, or <c>3</c> for simple pread/pwrite.
        /// </summary>
        [JsonPropertyName("disk_io_type")]
        public DiskIoType? DiskIoType { get; set; }

        /// <summary>
        /// Gets or sets the disk I/O write mode: <c>0</c> for disable OS cache, <c>1</c> for enable OS cache, or <c>2</c> for write-through when supported by the upstream build.
        /// </summary>
        [JsonPropertyName("disk_io_write_mode")]
        public DiskIoWriteMode? DiskIoWriteMode { get; set; }

        /// <summary>
        /// Gets or sets the disk queue size in bytes.
        /// </summary>
        [JsonPropertyName("disk_queue_size")]
        public int? DiskQueueSize { get; set; }

        /// <summary>
        /// Gets or sets the download limit in bytes per second.
        /// </summary>
        [JsonPropertyName("dl_limit")]
        public int? DlLimit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether slow torrents are excluded from queueing limits.
        /// </summary>
        [JsonPropertyName("dont_count_slow_torrents")]
        public bool? DontCountSlowTorrents { get; set; }

        /// <summary>
        /// Gets or sets the dyndns domain.
        /// </summary>
        [JsonPropertyName("dyndns_domain")]
        public string? DyndnsDomain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether dynamic DNS updates are enabled.
        /// </summary>
        [JsonPropertyName("dyndns_enabled")]
        public bool? DyndnsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the dyndns password.
        /// </summary>
        [JsonPropertyName("dyndns_password")]
        public string? DyndnsPassword { get; set; }

        /// <summary>
        /// Gets or sets the dynamic DNS service selector: <c>0</c> for DynDNS or <c>1</c> for No-IP.
        /// </summary>
        [JsonPropertyName("dyndns_service")]
        public DyndnsService? DyndnsService { get; set; }

        /// <summary>
        /// Gets or sets the dyndns username.
        /// </summary>
        [JsonPropertyName("dyndns_username")]
        public string? DyndnsUsername { get; set; }

        /// <summary>
        /// Gets or sets the embedded tracker port number.
        /// </summary>
        [JsonPropertyName("embedded_tracker_port")]
        public int? EmbeddedTrackerPort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the embedded tracker port is forwarded.
        /// </summary>
        [JsonPropertyName("embedded_tracker_port_forwarding")]
        public bool? EmbeddedTrackerPortForwarding { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether coalesced read and write operations are enabled.
        /// </summary>
        [JsonPropertyName("enable_coalesce_read_write")]
        public bool? EnableCoalesceReadWrite { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the embedded tracker is enabled.
        /// </summary>
        [JsonPropertyName("enable_embedded_tracker")]
        public bool? EnableEmbeddedTracker { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether multiple connections from the same IP are allowed.
        /// </summary>
        [JsonPropertyName("enable_multi_connections_from_same_ip")]
        public bool? EnableMultiConnectionsFromSameIp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether piece-extent affinity is enabled.
        /// </summary>
        [JsonPropertyName("enable_piece_extent_affinity")]
        public bool? EnablePieceExtentAffinity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether upload suggestions are enabled.
        /// </summary>
        [JsonPropertyName("enable_upload_suggestions")]
        public bool? EnableUploadSuggestions { get; set; }

        /// <summary>
        /// Gets or sets the encryption mode: <c>0</c> for allow encryption, <c>1</c> for require encryption, or <c>2</c> for disable encryption.
        /// </summary>
        [JsonPropertyName("encryption")]
        public EncryptionMode? Encryption { get; set; }

        /// <summary>
        /// Gets or sets the excluded file names.
        /// </summary>
        [JsonPropertyName("excluded_file_names")]
        public string? ExcludedFileNames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether excluded file-name filtering is enabled.
        /// </summary>
        [JsonPropertyName("excluded_file_names_enabled")]
        public bool? ExcludedFileNamesEnabled { get; set; }

        /// <summary>
        /// Gets or sets the export dir.
        /// </summary>
        [JsonPropertyName("export_dir")]
        public string? ExportDir { get; set; }

        /// <summary>
        /// Gets or sets the export dir fin.
        /// </summary>
        [JsonPropertyName("export_dir_fin")]
        public string? ExportDirFin { get; set; }

        /// <summary>
        /// Gets or sets the backup log retention age in the units selected by <see cref="FileLogAgeType" />.
        /// </summary>
        [JsonPropertyName("file_log_age")]
        public int? FileLogAge { get; set; }

        /// <summary>
        /// Gets or sets the backup log retention unit selector: <c>0</c> for days, <c>1</c> for months, or <c>2</c> for years.
        /// </summary>
        [JsonPropertyName("file_log_age_type")]
        public int? FileLogAgeType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether file-log backups are enabled.
        /// </summary>
        [JsonPropertyName("file_log_backup_enabled")]
        public bool? FileLogBackupEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether old log files are deleted.
        /// </summary>
        [JsonPropertyName("file_log_delete_old")]
        public bool? FileLogDeleteOld { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether file logging is enabled.
        /// </summary>
        [JsonPropertyName("file_log_enabled")]
        public bool? FileLogEnabled { get; set; }

        /// <summary>
        /// Gets or sets the log file backup threshold in KiB.
        /// </summary>
        [JsonPropertyName("file_log_max_size")]
        public int? FileLogMaxSize { get; set; }

        /// <summary>
        /// Gets or sets the file log path.
        /// </summary>
        [JsonPropertyName("file_log_path")]
        public string? FileLogPath { get; set; }

        /// <summary>
        /// Gets or sets the file pool size.
        /// </summary>
        [JsonPropertyName("file_pool_size")]
        public int? FilePoolSize { get; set; }

        /// <summary>
        /// Gets or sets the hashing threads.
        /// </summary>
        [JsonPropertyName("hashing_threads")]
        public int? HashingThreads { get; set; }

        /// <summary>
        /// Gets or sets the I2P address.
        /// </summary>
        [JsonPropertyName("i2p_address")]
        public string? I2pAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether I2P is enabled.
        /// </summary>
        [JsonPropertyName("i2p_enabled")]
        public bool? I2pEnabled { get; set; }

        /// <summary>
        /// Gets or sets the I2P inbound length.
        /// </summary>
        [JsonPropertyName("i2p_inbound_length")]
        public int? I2pInboundLength { get; set; }

        /// <summary>
        /// Gets or sets the I2P inbound quantity.
        /// </summary>
        [JsonPropertyName("i2p_inbound_quantity")]
        public int? I2pInboundQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether I2P mixed mode is enabled.
        /// </summary>
        [JsonPropertyName("i2p_mixed_mode")]
        public bool? I2pMixedMode { get; set; }

        /// <summary>
        /// Gets or sets the I2P outbound length.
        /// </summary>
        [JsonPropertyName("i2p_outbound_length")]
        public int? I2pOutboundLength { get; set; }

        /// <summary>
        /// Gets or sets the I2P outbound quantity.
        /// </summary>
        [JsonPropertyName("i2p_outbound_quantity")]
        public int? I2pOutboundQuantity { get; set; }

        /// <summary>
        /// Gets or sets the I2P port number.
        /// </summary>
        [JsonPropertyName("i2p_port")]
        public int? I2pPort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether idn support is enabled.
        /// </summary>
        [JsonPropertyName("idn_support_enabled")]
        public bool? IdnSupportEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the incomplete-files extension is used.
        /// </summary>
        [JsonPropertyName("incomplete_files_ext")]
        public bool? IncompleteFilesExt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether unwanted folder is used.
        /// </summary>
        [JsonPropertyName("use_unwanted_folder")]
        public bool? UseUnwantedFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IP filtering is enabled.
        /// </summary>
        [JsonPropertyName("ip_filter_enabled")]
        public bool? IpFilterEnabled { get; set; }

        /// <summary>
        /// Gets or sets the IP filter path.
        /// </summary>
        [JsonPropertyName("ip_filter_path")]
        public string? IpFilterPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tracker traffic is filtered through the IP filter.
        /// </summary>
        [JsonPropertyName("ip_filter_trackers")]
        public bool? IpFilterTrackers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether LAN peers are excluded from transfer limits.
        /// </summary>
        [JsonPropertyName("limit_lan_peers")]
        public bool? LimitLanPeers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether TCP overhead counts toward transfer limits.
        /// </summary>
        [JsonPropertyName("limit_tcp_overhead")]
        public bool? LimitTcpOverhead { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether uTP traffic counts toward transfer limits.
        /// </summary>
        [JsonPropertyName("limit_utp_rate")]
        public bool? LimitUtpRate { get; set; }

        /// <summary>
        /// Gets or sets the listen port number.
        /// </summary>
        [JsonPropertyName("listen_port")]
        public int? ListenPort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SSL is enabled.
        /// </summary>
        [JsonPropertyName("ssl_enabled")]
        public bool? SslEnabled { get; set; }

        /// <summary>
        /// Gets or sets the SSL listen port number.
        /// </summary>
        [JsonPropertyName("ssl_listen_port")]
        public int? SslListenPort { get; set; }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        [JsonPropertyName("locale")]
        public string? Locale { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether LSD is enabled.
        /// </summary>
        [JsonPropertyName("LSD")]
        public bool? Lsd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SMTP authentication is enabled.
        /// </summary>
        [JsonPropertyName("mail_notification_auth_enabled")]
        public bool? MailNotificationAuthEnabled { get; set; }

        /// <summary>
        /// Gets or sets the mail notification email.
        /// </summary>
        [JsonPropertyName("mail_notification_email")]
        public string? MailNotificationEmail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether mail notifications are enabled.
        /// </summary>
        [JsonPropertyName("mail_notification_enabled")]
        public bool? MailNotificationEnabled { get; set; }

        /// <summary>
        /// Gets or sets the mail notification password.
        /// </summary>
        [JsonPropertyName("mail_notification_password")]
        public string? MailNotificationPassword { get; set; }

        /// <summary>
        /// Gets or sets the mail notification sender.
        /// </summary>
        [JsonPropertyName("mail_notification_sender")]
        public string? MailNotificationSender { get; set; }

        /// <summary>
        /// Gets or sets the mail notification SMTP server.
        /// </summary>
        [JsonPropertyName("mail_notification_smtp")]
        public string? MailNotificationSmtp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether mail notification SSL is enabled.
        /// </summary>
        [JsonPropertyName("mail_notification_ssl_enabled")]
        public bool? MailNotificationSslEnabled { get; set; }

        /// <summary>
        /// Gets or sets the mail notification username.
        /// </summary>
        [JsonPropertyName("mail_notification_username")]
        public string? MailNotificationUsername { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Mark of the Web is applied to downloaded files.
        /// </summary>
        [JsonPropertyName("mark_of_the_web")]
        public bool? MarkOfTheWeb { get; set; }

        /// <summary>
        /// Gets or sets the maximum active checking torrent count.
        /// </summary>
        [JsonPropertyName("max_active_checking_torrents")]
        public int? MaxActiveCheckingTorrents { get; set; }

        /// <summary>
        /// Gets or sets the maximum active download count.
        /// </summary>
        [JsonPropertyName("max_active_downloads")]
        public int? MaxActiveDownloads { get; set; }

        /// <summary>
        /// Gets or sets the maximum active torrent count.
        /// </summary>
        [JsonPropertyName("max_active_torrents")]
        public int? MaxActiveTorrents { get; set; }

        /// <summary>
        /// Gets or sets the maximum active upload count.
        /// </summary>
        [JsonPropertyName("max_active_uploads")]
        public int? MaxActiveUploads { get; set; }

        /// <summary>
        /// Gets or sets the maximum concurrent HTTP announce count.
        /// </summary>
        [JsonPropertyName("max_concurrent_http_announces")]
        public int? MaxConcurrentHttpAnnounces { get; set; }

        /// <summary>
        /// Gets or sets the maximum connection count.
        /// </summary>
        [JsonPropertyName("max_connec")]
        public int? MaxConnec { get; set; }

        /// <summary>
        /// Gets or sets the maximum connection count per torrent.
        /// </summary>
        [JsonPropertyName("max_connec_per_torrent")]
        public int? MaxConnecPerTorrent { get; set; }

        /// <summary>
        /// Gets or sets the max inactive seeding time in minutes.
        /// </summary>
        [JsonPropertyName("max_inactive_seeding_time")]
        public int? MaxInactiveSeedingTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the maximum inactive seeding time is enabled.
        /// </summary>
        [JsonPropertyName("max_inactive_seeding_time_enabled")]
        public bool? MaxInactiveSeedingTimeEnabled { get; set; }

        /// <summary>
        /// Gets or sets the max ratio as a unitless ratio value.
        /// </summary>
        [JsonPropertyName("max_ratio")]
        public double? MaxRatio { get; set; }

        /// <summary>
        /// Gets or sets the share-limit action: <c>0</c> for stop torrent, <c>1</c> for remove torrent, <c>2</c> for enable super seeding, or <c>3</c> for remove torrent and its files.
        /// </summary>
        [JsonPropertyName("max_ratio_act")]
        public MaxRatioAction? MaxRatioAct { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the maximum ratio is enabled.
        /// </summary>
        [JsonPropertyName("max_ratio_enabled")]
        public bool? MaxRatioEnabled { get; set; }

        /// <summary>
        /// Gets or sets the max seeding time in minutes.
        /// </summary>
        [JsonPropertyName("max_seeding_time")]
        public int? MaxSeedingTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the maximum seeding time is enabled.
        /// </summary>
        [JsonPropertyName("max_seeding_time_enabled")]
        public bool? MaxSeedingTimeEnabled { get; set; }

        /// <summary>
        /// Gets or sets the maximum upload slot count.
        /// </summary>
        [JsonPropertyName("max_uploads")]
        public int? MaxUploads { get; set; }

        /// <summary>
        /// Gets or sets the maximum upload slot count per torrent.
        /// </summary>
        [JsonPropertyName("max_uploads_per_torrent")]
        public int? MaxUploadsPerTorrent { get; set; }

        /// <summary>
        /// Gets or sets the memory working set limit in MiB.
        /// </summary>
        [JsonPropertyName("memory_working_set_limit")]
        public int? MemoryWorkingSetLimit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether trackers from multiple sources are merged.
        /// </summary>
        [JsonPropertyName("merge_trackers")]
        public bool? MergeTrackers { get; set; }

        /// <summary>
        /// Gets or sets the maximum outgoing port number.
        /// </summary>
        [JsonPropertyName("outgoing_ports_max")]
        public int? OutgoingPortsMax { get; set; }

        /// <summary>
        /// Gets or sets the minimum outgoing port number.
        /// </summary>
        [JsonPropertyName("outgoing_ports_min")]
        public int? OutgoingPortsMin { get; set; }

        /// <summary>
        /// Gets or sets the peer Type of Service value.
        /// </summary>
        [JsonPropertyName("peer_tos")]
        public int? PeerTos { get; set; }

        /// <summary>
        /// Gets or sets the peer turnover disconnect percentage.
        /// </summary>
        [JsonPropertyName("peer_turnover")]
        public int? PeerTurnover { get; set; }

        /// <summary>
        /// Gets or sets the peer turnover threshold percentage.
        /// </summary>
        [JsonPropertyName("peer_turnover_cutoff")]
        public int? PeerTurnoverCutoff { get; set; }

        /// <summary>
        /// Gets or sets the peer turnover disconnect interval in seconds.
        /// </summary>
        [JsonPropertyName("peer_turnover_interval")]
        public int? PeerTurnoverInterval { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether performance warnings are enabled.
        /// </summary>
        [JsonPropertyName("performance_warning")]
        public bool? PerformanceWarning { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether PEX is enabled.
        /// </summary>
        [JsonPropertyName("PEX")]
        public bool? Pex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all files are preallocated.
        /// </summary>
        [JsonPropertyName("preallocate_all")]
        public bool? PreallocateAll { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether proxy auth is enabled.
        /// </summary>
        [JsonPropertyName("proxy_auth_enabled")]
        public bool? ProxyAuthEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether BitTorrent traffic uses the proxy.
        /// </summary>
        [JsonPropertyName("proxy_bittorrent")]
        public bool? ProxyBittorrent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether hostname lookups use the proxy.
        /// </summary>
        [JsonPropertyName("proxy_hostname_lookup")]
        public bool? ProxyHostnameLookup { get; set; }

        /// <summary>
        /// Gets or sets the proxy IP address.
        /// </summary>
        [JsonPropertyName("proxy_ip")]
        public string? ProxyIp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether miscellaneous traffic uses the proxy.
        /// </summary>
        [JsonPropertyName("proxy_misc")]
        public bool? ProxyMisc { get; set; }

        /// <summary>
        /// Gets or sets the proxy password.
        /// </summary>
        [JsonPropertyName("proxy_password")]
        public string? ProxyPassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether peer connections use the proxy.
        /// </summary>
        [JsonPropertyName("proxy_peer_connections")]
        public bool? ProxyPeerConnections { get; set; }

        /// <summary>
        /// Gets or sets the proxy port number.
        /// </summary>
        [JsonPropertyName("proxy_port")]
        public int? ProxyPort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether RSS traffic uses the proxy.
        /// </summary>
        [JsonPropertyName("proxy_rss")]
        public bool? ProxyRss { get; set; }

        /// <summary>
        /// Gets or sets the proxy type.
        /// </summary>
        [JsonPropertyName("proxy_type")]
        public ProxyType? ProxyType { get; set; }

        /// <summary>
        /// Gets or sets the proxy username.
        /// </summary>
        [JsonPropertyName("proxy_username")]
        public string? ProxyUsername { get; set; }

        /// <summary>
        /// Gets or sets the python executable path.
        /// </summary>
        [JsonPropertyName("python_executable_path")]
        public string? PythonExecutablePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether queueing is enabled.
        /// </summary>
        [JsonPropertyName("queueing_enabled")]
        public bool? QueueingEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a random listening port is used.
        /// </summary>
        [JsonPropertyName("random_port")]
        public bool? RandomPort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether torrents are reannounced when the address changes.
        /// </summary>
        [JsonPropertyName("reannounce_when_address_changed")]
        public bool? ReannounceWhenAddressChanged { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether completed torrents are rechecked.
        /// </summary>
        [JsonPropertyName("recheck_completed_torrents")]
        public bool? RecheckCompletedTorrents { get; set; }

        /// <summary>
        /// Gets or sets the refresh interval in milliseconds.
        /// </summary>
        [JsonPropertyName("refresh_interval")]
        public int? RefreshInterval { get; set; }

        /// <summary>
        /// Gets or sets the maximum outstanding request count to a single peer.
        /// </summary>
        [JsonPropertyName("request_queue_size")]
        public int? RequestQueueSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether peer countries are resolved.
        /// </summary>
        [JsonPropertyName("resolve_peer_countries")]
        public bool? ResolvePeerCountries { get; set; }

        /// <summary>
        /// Gets or sets the resume data storage type.
        /// </summary>
        [JsonPropertyName("resume_data_storage_type")]
        public ResumeDataStorageType? ResumeDataStorageType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether RSS auto downloading is enabled.
        /// </summary>
        [JsonPropertyName("rss_auto_downloading_enabled")]
        public bool? RssAutoDownloadingEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether RSS repack and proper episodes are downloaded.
        /// </summary>
        [JsonPropertyName("rss_download_repack_proper_episodes")]
        public bool? RssDownloadRepackProperEpisodes { get; set; }

        /// <summary>
        /// Gets or sets the same-host RSS request delay in seconds.
        /// </summary>
        [JsonPropertyName("rss_fetch_delay")]
        public long? RssFetchDelay { get; set; }

        /// <summary>
        /// Gets or sets the RSS max articles per feed.
        /// </summary>
        [JsonPropertyName("rss_max_articles_per_feed")]
        public int? RssMaxArticlesPerFeed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether RSS processing is enabled.
        /// </summary>
        [JsonPropertyName("rss_processing_enabled")]
        public bool? RssProcessingEnabled { get; set; }

        /// <summary>
        /// Gets or sets the RSS refresh interval in minutes.
        /// </summary>
        [JsonPropertyName("rss_refresh_interval")]
        public int? RssRefreshInterval { get; set; }

        /// <summary>
        /// Gets or sets the RSS smart episode filters.
        /// </summary>
        [JsonPropertyName("rss_smart_episode_filters")]
        public string? RssSmartEpisodeFilters { get; set; }

        /// <summary>
        /// Gets or sets the save path.
        /// </summary>
        [JsonPropertyName("save_path")]
        public string? SavePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether save path changed TMM is enabled.
        /// </summary>
        [JsonPropertyName("save_path_changed_tmm_enabled")]
        public bool? SavePathChangedTmmEnabled { get; set; }

        /// <summary>
        /// Gets or sets the save resume data interval in minutes.
        /// </summary>
        [JsonPropertyName("save_resume_data_interval")]
        public int? SaveResumeDataInterval { get; set; }

        /// <summary>
        /// Gets or sets the save statistics interval in minutes.
        /// </summary>
        [JsonPropertyName("save_statistics_interval")]
        public int? SaveStatisticsInterval { get; set; }

        /// <summary>
        /// Gets or sets the monitored scan directories.
        /// </summary>
        [JsonPropertyName("scan_dirs")]
        public Dictionary<string, SaveLocation>? ScanDirs { get; set; }

        /// <summary>
        /// Gets or sets the scheduled start hour in 24-hour time.
        /// </summary>
        [JsonPropertyName("schedule_from_hour")]
        public int? ScheduleFromHour { get; set; }

        /// <summary>
        /// Gets or sets the scheduled start minute.
        /// </summary>
        [JsonPropertyName("schedule_from_min")]
        public int? ScheduleFromMin { get; set; }

        /// <summary>
        /// Gets or sets the scheduled end hour in 24-hour time.
        /// </summary>
        [JsonPropertyName("schedule_to_hour")]
        public int? ScheduleToHour { get; set; }

        /// <summary>
        /// Gets or sets the scheduled end minute.
        /// </summary>
        [JsonPropertyName("schedule_to_min")]
        public int? ScheduleToMin { get; set; }

        /// <summary>
        /// Gets or sets the scheduler day selection: <c>0</c> for every day, <c>1</c> for weekdays, <c>2</c> for weekends, <c>3</c> for Monday, <c>4</c> for Tuesday, <c>5</c> for Wednesday, <c>6</c> for Thursday, <c>7</c> for Friday, <c>8</c> for Saturday, or <c>9</c> for Sunday.
        /// </summary>
        [JsonPropertyName("scheduler_days")]
        public SchedulerDays? SchedulerDays { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether scheduled alternative speed limits are enabled.
        /// </summary>
        [JsonPropertyName("scheduler_enabled")]
        public bool? SchedulerEnabled { get; set; }

        /// <summary>
        /// Gets or sets the send buffer low watermark in KiB.
        /// </summary>
        [JsonPropertyName("send_buffer_low_watermark")]
        public int? SendBufferLowWatermark { get; set; }

        /// <summary>
        /// Gets or sets the send buffer watermark in KiB.
        /// </summary>
        [JsonPropertyName("send_buffer_watermark")]
        public int? SendBufferWatermark { get; set; }

        /// <summary>
        /// Gets or sets the send buffer watermark factor as a percentage.
        /// </summary>
        [JsonPropertyName("send_buffer_watermark_factor")]
        public int? SendBufferWatermarkFactor { get; set; }

        /// <summary>
        /// Gets or sets the slow torrent download rate threshold in KiB/s.
        /// </summary>
        [JsonPropertyName("slow_torrent_dl_rate_threshold")]
        public int? SlowTorrentDlRateThreshold { get; set; }

        /// <summary>
        /// Gets or sets the slow torrent inactivity timer in seconds.
        /// </summary>
        [JsonPropertyName("slow_torrent_inactive_timer")]
        public int? SlowTorrentInactiveTimer { get; set; }

        /// <summary>
        /// Gets or sets the slow torrent upload rate threshold in KiB/s.
        /// </summary>
        [JsonPropertyName("slow_torrent_ul_rate_threshold")]
        public int? SlowTorrentUlRateThreshold { get; set; }

        /// <summary>
        /// Gets or sets the socket backlog size as a connection count.
        /// </summary>
        [JsonPropertyName("socket_backlog_size")]
        public int? SocketBacklogSize { get; set; }

        /// <summary>
        /// Gets or sets the socket receive buffer size in bytes. A value of <c>0</c> uses the system default.
        /// </summary>
        [JsonPropertyName("socket_receive_buffer_size")]
        public int? SocketReceiveBufferSize { get; set; }

        /// <summary>
        /// Gets or sets the socket send buffer size in bytes. A value of <c>0</c> uses the system default.
        /// </summary>
        [JsonPropertyName("socket_send_buffer_size")]
        public int? SocketSendBufferSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SSRF mitigation is enabled.
        /// </summary>
        [JsonPropertyName("ssrf_mitigation")]
        public bool? SsrfMitigation { get; set; }

        /// <summary>
        /// Gets or sets the stop tracker timeout in seconds.
        /// </summary>
        [JsonPropertyName("stop_tracker_timeout")]
        public int? StopTrackerTimeout { get; set; }

        /// <summary>
        /// Gets or sets the temp path.
        /// </summary>
        [JsonPropertyName("temp_path")]
        public string? TempPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the temporary path is enabled.
        /// </summary>
        [JsonPropertyName("temp_path_enabled")]
        public bool? TempPathEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether automatic torrent management reacts to torrent changes.
        /// </summary>
        [JsonPropertyName("torrent_changed_tmm_enabled")]
        public bool? TorrentChangedTmmEnabled { get; set; }

        /// <summary>
        /// Gets or sets the torrent content layout.
        /// </summary>
        [JsonPropertyName("torrent_content_layout")]
        public TorrentContentLayout? TorrentContentLayout { get; set; }

        /// <summary>
        /// Gets or sets the torrent content remove option.
        /// </summary>
        [JsonPropertyName("torrent_content_remove_option")]
        public TorrentContentRemoveOption? TorrentContentRemoveOption { get; set; }

        /// <summary>
        /// Gets or sets the torrent file size limit in bytes.
        /// </summary>
        [JsonPropertyName("torrent_file_size_limit")]
        public int? TorrentFileSizeLimit { get; set; }

        /// <summary>
        /// Gets or sets the torrent stop condition.
        /// </summary>
        [JsonPropertyName("torrent_stop_condition")]
        public StopCondition? TorrentStopCondition { get; set; }

        /// <summary>
        /// Gets or sets the upload limit in bytes per second.
        /// </summary>
        [JsonPropertyName("up_limit")]
        public int? UpLimit { get; set; }

        /// <summary>
        /// Gets or sets the upload choking algorithm: <c>0</c> for round-robin, <c>1</c> for fastest upload, or <c>2</c> for anti-leech.
        /// </summary>
        [JsonPropertyName("upload_choking_algorithm")]
        public UploadChokingAlgorithm? UploadChokingAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the upload slots behavior: <c>0</c> for fixed slots or <c>1</c> for upload-rate-based slots.
        /// </summary>
        [JsonPropertyName("upload_slots_behavior")]
        public UploadSlotsBehavior? UploadSlotsBehavior { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether UPnP is enabled.
        /// </summary>
        [JsonPropertyName("upnp")]
        public bool? Upnp { get; set; }

        /// <summary>
        /// Gets or sets the UPnP lease duration in seconds. A value of <c>0</c> requests a permanent lease.
        /// </summary>
        [JsonPropertyName("upnp_lease_duration")]
        public int? UpnpLeaseDuration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether category paths are used in manual mode.
        /// </summary>
        [JsonPropertyName("use_category_paths_in_manual_mode")]
        public bool? UseCategoryPathsInManualMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HTTPS is enabled.
        /// </summary>
        [JsonPropertyName("use_https")]
        public bool? UseHttps { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SSL certificate errors are ignored.
        /// </summary>
        [JsonPropertyName("ignore_ssl_errors")]
        public bool? IgnoreSslErrors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether subcategories are used.
        /// </summary>
        [JsonPropertyName("use_subcategories")]
        public bool? UseSubcategories { get; set; }

        /// <summary>
        /// Gets or sets the uTP/TCP mixed mode algorithm: <c>0</c> for prefer TCP or <c>1</c> for peer proportional.
        /// </summary>
        [JsonPropertyName("utp_tcp_mixed_mode")]
        public UtpTcpMixedMode? UtpTcpMixedMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HTTPS tracker certificates are validated.
        /// </summary>
        [JsonPropertyName("validate_https_tracker_certificate")]
        public bool? ValidateHttpsTrackerCertificate { get; set; }

        /// <summary>
        /// Gets or sets the Web UI address.
        /// </summary>
        [JsonPropertyName("web_ui_address")]
        public string? WebUiAddress { get; set; }

        /// <summary>
        /// Gets or sets the Web UI API key.
        /// </summary>
        [JsonPropertyName("web_ui_api_key")]
        public string? WebUiApiKey { get; set; }

        /// <summary>
        /// Gets or sets the Web UI ban duration in seconds.
        /// </summary>
        [JsonPropertyName("web_ui_ban_duration")]
        public int? WebUiBanDuration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Web UI clickjacking protection is enabled.
        /// </summary>
        [JsonPropertyName("web_ui_clickjacking_protection_enabled")]
        public bool? WebUiClickjackingProtectionEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Web UI CSRF protection is enabled.
        /// </summary>
        [JsonPropertyName("web_ui_csrf_protection_enabled")]
        public bool? WebUiCsrfProtectionEnabled { get; set; }

        /// <summary>
        /// Gets or sets the Web UI custom HTTP headers.
        /// </summary>
        [JsonPropertyName("web_ui_custom_http_headers")]
        public string? WebUiCustomHttpHeaders { get; set; }

        /// <summary>
        /// Gets or sets the Web UI domain list.
        /// </summary>
        [JsonPropertyName("web_ui_domain_list")]
        public string? WebUiDomainList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Web UI host-header validation is enabled.
        /// </summary>
        [JsonPropertyName("web_ui_host_header_validation_enabled")]
        public bool? WebUiHostHeaderValidationEnabled { get; set; }

        /// <summary>
        /// Gets or sets the Web UI HTTPS cert path.
        /// </summary>
        [JsonPropertyName("web_ui_https_cert_path")]
        public string? WebUiHttpsCertPath { get; set; }

        /// <summary>
        /// Gets or sets the Web UI HTTPS key path.
        /// </summary>
        [JsonPropertyName("web_ui_https_key_path")]
        public string? WebUiHttpsKeyPath { get; set; }

        /// <summary>
        /// Gets or sets the maximum failed Web UI authentication attempt count before a ban is applied.
        /// </summary>
        [JsonPropertyName("web_ui_max_auth_fail_count")]
        public int? WebUiMaxAuthFailCount { get; set; }

        /// <summary>
        /// Gets or sets the Web UI port number.
        /// </summary>
        [JsonPropertyName("web_ui_port")]
        public int? WebUiPort { get; set; }

        /// <summary>
        /// Gets or sets the Web UI reverse proxies list.
        /// </summary>
        [JsonPropertyName("web_ui_reverse_proxies_list")]
        public string? WebUiReverseProxiesList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Web UI reverse-proxy support is enabled.
        /// </summary>
        [JsonPropertyName("web_ui_reverse_proxy_enabled")]
        public bool? WebUiReverseProxyEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Web UI secure cookies are enabled.
        /// </summary>
        [JsonPropertyName("web_ui_secure_cookie_enabled")]
        public bool? WebUiSecureCookieEnabled { get; set; }

        /// <summary>
        /// Gets or sets the Web UI session timeout in seconds.
        /// </summary>
        [JsonPropertyName("web_ui_session_timeout")]
        public int? WebUiSessionTimeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether UPnP is enabled for the Web UI.
        /// </summary>
        [JsonPropertyName("web_ui_upnp")]
        public bool? WebUiUpnp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether custom Web UI HTTP headers are enabled.
        /// </summary>
        [JsonPropertyName("web_ui_use_custom_http_headers_enabled")]
        public bool? WebUiUseCustomHttpHeadersEnabled { get; set; }

        /// <summary>
        /// Gets or sets the Web UI username.
        /// </summary>
        [JsonPropertyName("web_ui_username")]
        public string? WebUiUsername { get; set; }

        /// <summary>
        /// Gets or sets the Web UI password.
        /// </summary>
        [JsonPropertyName("web_ui_password")]
        public string? WebUiPassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether torrent deletion requires confirmation.
        /// </summary>
        [JsonPropertyName("confirm_torrent_deletion")]
        public bool? ConfirmTorrentDeletion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether torrent rechecks require confirmation.
        /// </summary>
        [JsonPropertyName("confirm_torrent_recheck")]
        public bool? ConfirmTorrentRecheck { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the status bar displays the external IP address.
        /// </summary>
        [JsonPropertyName("status_bar_external_ip")]
        public bool? StatusBarExternalIp { get; set; }

        /// <summary>
        /// Validates the preference changes before they are sent to qBittorrent.
        /// </summary>
        public void Validate()
        {
            if (MaxRatio.HasValue && MaxRatioEnabled.HasValue)
            {
                throw new InvalidOperationException("Specify either max_ratio or max_ratio_enabled, not both.");
            }

            if (MaxSeedingTime.HasValue && MaxSeedingTimeEnabled.HasValue)
            {
                throw new InvalidOperationException("Specify either max_seeding_time or max_seeding_time_enabled, not both.");
            }

            if (MaxInactiveSeedingTime.HasValue && MaxInactiveSeedingTimeEnabled.HasValue)
            {
                throw new InvalidOperationException("Specify either max_inactive_seeding_time or max_inactive_seeding_time_enabled, not both.");
            }
        }
    }
}
