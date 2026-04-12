namespace QBittorrent.ApiClient
{
    internal sealed class ApiClientCompatibilityProfile
    {
        private static readonly Version _rssFeedRefreshIntervalMinimumVersion = new(2, 11, 5);
        private static readonly Version _directoryContentMetadataMinimumVersion = new(2, 11, 8);
        private static readonly Version _torrentListIncludeFilesMinimumVersion = new(2, 11, 8);
        private static readonly Version _torrentAddFilePrioritiesMinimumVersion = new(2, 11, 8);
        private static readonly Version _torrentMetadataMinimumVersion = new(2, 11, 9);
        private static readonly Version _trackerBatchOperationsMinimumVersion = new(2, 11, 9);
        private static readonly Version _trackerAllValueMinimumVersion = new(2, 11, 9);
        private static readonly Version _reannounceUrlsMinimumVersion = new(2, 11, 10);
        private static readonly Version _torrentShareLimitActionMinimumVersion = new(2, 12, 0);
        private static readonly Version _torrentCommentEditingMinimumVersion = new(2, 12, 1);
        private static readonly Version _trackerTierEditingMinimumVersion = new(2, 13, 0);
        private static readonly Version _clientDataMinimumVersion = new(2, 13, 1);
        private static readonly Version _torrentAddDownloaderMinimumVersion = new(2, 13, 1);
        private static readonly Version _apiKeyManagementMinimumVersion = new(2, 14, 1);
        private static readonly Version _processInfoMinimumVersion = new(2, 15, 1);
        private static readonly Version _torrentPieceAvailabilityMinimumVersion = new(2, 15, 1);

        public ApiClientCompatibilityProfile(Version webApiVersion)
        {
            WebApiVersion = webApiVersion;
            SupportsClientData = webApiVersion >= _clientDataMinimumVersion;
            SupportsProcessInfo = webApiVersion >= _processInfoMinimumVersion;
            SupportsApiKeyManagement = webApiVersion >= _apiKeyManagementMinimumVersion;
            SupportsDirectoryContentMetadata = webApiVersion >= _directoryContentMetadataMinimumVersion;
            SupportsRssFeedRefreshInterval = webApiVersion >= _rssFeedRefreshIntervalMinimumVersion;
            SupportsTorrentListIncludeFiles = webApiVersion >= _torrentListIncludeFilesMinimumVersion;
            SupportsTorrentAddDownloader = webApiVersion >= _torrentAddDownloaderMinimumVersion;
            SupportsTorrentAddFilePriorities = webApiVersion >= _torrentAddFilePrioritiesMinimumVersion;
            SupportsTrackerBatchOperations = webApiVersion >= _trackerBatchOperationsMinimumVersion;
            SupportsTrackerTierEditing = webApiVersion >= _trackerTierEditingMinimumVersion;
            SupportsReannounceUrls = webApiVersion >= _reannounceUrlsMinimumVersion;
            SupportsTorrentPieceAvailability = webApiVersion >= _torrentPieceAvailabilityMinimumVersion;
            SupportsTorrentCommentEditing = webApiVersion >= _torrentCommentEditingMinimumVersion;
            SupportsTorrentMetadata = webApiVersion >= _torrentMetadataMinimumVersion;
            SupportsTorrentShareLimitAction = webApiVersion >= _torrentShareLimitActionMinimumVersion;
            TrackerAllValue = webApiVersion >= _trackerAllValueMinimumVersion
                ? "all"
                : "*";
        }

        public Version WebApiVersion { get; }

        public bool SupportsClientData { get; }

        public bool SupportsProcessInfo { get; }

        public bool SupportsApiKeyManagement { get; }

        public bool SupportsDirectoryContentMetadata { get; }

        public bool SupportsRssFeedRefreshInterval { get; }

        public bool SupportsTorrentListIncludeFiles { get; }

        public bool SupportsTorrentAddDownloader { get; }

        public bool SupportsTorrentAddFilePriorities { get; }

        public bool SupportsTrackerBatchOperations { get; }

        public bool SupportsTrackerTierEditing { get; }

        public bool SupportsReannounceUrls { get; }

        public bool SupportsTorrentPieceAvailability { get; }

        public bool SupportsTorrentCommentEditing { get; }

        public bool SupportsTorrentMetadata { get; }

        public bool SupportsTorrentShareLimitAction { get; }

        public string TrackerAllValue { get; }
    }
}
