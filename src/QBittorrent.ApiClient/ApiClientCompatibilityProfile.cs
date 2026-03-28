namespace QBittorrent.ApiClient
{
    internal sealed class ApiClientCompatibilityProfile
    {
        private static readonly Version _clientDataMinimumVersion = new(2, 13, 1);
        private static readonly Version _modernTrackerOperationsMinimumVersion = new(2, 15, 2);

        public ApiClientCompatibilityProfile(Version webApiVersion)
        {
            WebApiVersion = webApiVersion;
            SupportsClientData = webApiVersion >= _clientDataMinimumVersion;
            SupportsTorrentListIncludeFiles = webApiVersion >= _modernTrackerOperationsMinimumVersion;
            SupportsTrackerBatchOperations = webApiVersion >= _modernTrackerOperationsMinimumVersion;
            SupportsTrackerTierEditing = webApiVersion >= _modernTrackerOperationsMinimumVersion;
            RequiresShareLimitAction = webApiVersion >= _modernTrackerOperationsMinimumVersion;
            TrackerAllValue = webApiVersion >= _modernTrackerOperationsMinimumVersion
                ? "all"
                : "*";
        }

        public Version WebApiVersion { get; }

        public bool SupportsClientData { get; }

        public bool SupportsTorrentListIncludeFiles { get; }

        public bool SupportsTrackerBatchOperations { get; }

        public bool SupportsTrackerTierEditing { get; }

        public bool RequiresShareLimitAction { get; }

        public string TrackerAllValue { get; }
    }
}
