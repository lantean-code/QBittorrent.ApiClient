namespace QBittorrent.ApiClient
{
    internal sealed class ApiClientCompatibilityProfile
    {
        private static readonly Version _clientDataMinimumVersion = new(2, 13, 1);
        private static readonly Version _modernTrackerOperationsMinimumVersion = new(2, 15, 1);

        public ApiClientCompatibilityProfile(Version webApiVersion)
        {
            WebApiVersion = webApiVersion;
            SupportsClientData = webApiVersion >= _clientDataMinimumVersion;
            SupportsProcessInfo = webApiVersion >= _modernTrackerOperationsMinimumVersion;
            SupportsApiKeyManagement = webApiVersion >= _modernTrackerOperationsMinimumVersion;
            SupportsDirectoryContentMetadata = webApiVersion >= _modernTrackerOperationsMinimumVersion;
            SupportsRssFeedRefreshInterval = webApiVersion >= _modernTrackerOperationsMinimumVersion;
            SupportsTorrentListIncludeFiles = webApiVersion >= _modernTrackerOperationsMinimumVersion;
            SupportsTorrentAddDownloader = webApiVersion >= _modernTrackerOperationsMinimumVersion;
            SupportsTorrentAddFilePriorities = webApiVersion >= _modernTrackerOperationsMinimumVersion;
            SupportsTrackerBatchOperations = webApiVersion >= _modernTrackerOperationsMinimumVersion;
            SupportsTrackerTierEditing = webApiVersion >= _modernTrackerOperationsMinimumVersion;
            SupportsReannounceUrls = webApiVersion >= _modernTrackerOperationsMinimumVersion;
            SupportsTorrentPieceAvailability = webApiVersion >= _modernTrackerOperationsMinimumVersion;
            SupportsTorrentCommentEditing = webApiVersion >= _modernTrackerOperationsMinimumVersion;
            SupportsTorrentMetadata = webApiVersion >= _modernTrackerOperationsMinimumVersion;
            RequiresShareLimitAction = webApiVersion >= _modernTrackerOperationsMinimumVersion;
            TrackerAllValue = webApiVersion >= _modernTrackerOperationsMinimumVersion
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

        public bool RequiresShareLimitAction { get; }

        public string TrackerAllValue { get; }
    }
}
