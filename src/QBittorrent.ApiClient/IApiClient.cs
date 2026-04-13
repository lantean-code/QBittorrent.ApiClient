using System.Text.Json;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient
{
    /// <summary>
    /// Defines the public qBittorrent Web API client surface.
    /// </summary>
    public interface IApiClient
    {
        #region Authentication

        /// <summary>Checks whether the current HTTP client is authenticated against the qBittorrent WebUI.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result whose value is <see langword="true" /> when the current session is authenticated; otherwise, <see langword="false" />.</returns>
        Task<ApiResult<bool>> CheckAuthStateAsync(CancellationToken cancellationToken = default);

        /// <summary>Authenticates against the qBittorrent WebUI.</summary>
        /// <param name="username">The WebUI username.</param>
        /// <param name="password">The WebUI password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default);

        /// <summary>Ends the current qBittorrent WebUI session.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> LogoutAsync(CancellationToken cancellationToken = default);

        #endregion Authentication

        #region Application

        /// <summary>Gets the qBittorrent application version.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the qBittorrent version string.</returns>
        Task<ApiResult<string>> GetApplicationVersionAsync(CancellationToken cancellationToken = default);

        /// <summary>Gets the qBittorrent Web API version.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the Web API version string.</returns>
        Task<ApiResult<string>> GetAPIVersionAsync(CancellationToken cancellationToken = default);

        /// <summary>Clears and refreshes the cached qBittorrent compatibility profile.</summary>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        /// <remarks>This method re-queries qBittorrent version metadata used for internal compatibility decisions. Call this after changing the target server or after upgrading qBittorrent.</remarks>
        Task<ApiResult> RefreshCompatibilityAsync(CancellationToken cancellationToken = default);

        /// <summary>Loads client-side data stored by qBittorrent.</summary>
        /// <param name="keys">The optional keys to load. When <see langword="null" /> or empty, all stored data is returned.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the stored client-side data.</returns>
        Task<ApiResult<IReadOnlyDictionary<string, JsonElement>>> LoadClientDataAsync(IEnumerable<string>? keys = null, CancellationToken cancellationToken = default);

        /// <summary>Stores client-side data patches through qBittorrent's client data API.</summary>
        /// <param name="data">The key-value patch to apply. A <see langword="null" /> value deletes the key; a non-null value upserts the exact JSON payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> StoreClientDataAsync(IReadOnlyDictionary<string, JsonElement?> data, CancellationToken cancellationToken = default);

        /// <summary>Upserts client-side data through qBittorrent's client data API.</summary>
        /// <param name="data">The key-value data to upsert.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> UpsertClientDataAsync(IReadOnlyDictionary<string, JsonElement> data, CancellationToken cancellationToken = default);

        /// <summary>Deletes stored client-side data keys through qBittorrent's client data API.</summary>
        /// <param name="keys">The keys to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> DeleteClientDataAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);

        /// <summary>Gets qBittorrent build information.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the qBittorrent build information.</returns>
        Task<ApiResult<BuildInfo>> GetBuildInfoAsync(CancellationToken cancellationToken = default);

        /// <summary>Gets qBittorrent process information.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the qBittorrent process information.</returns>
        Task<ApiResult<ProcessInfo>> GetProcessInfoAsync(CancellationToken cancellationToken = default);

        /// <summary>Requests qBittorrent shutdown.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> ShutdownAsync(CancellationToken cancellationToken = default);

        /// <summary>Gets the current qBittorrent application preferences.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the current application preferences.</returns>
        Task<ApiResult<Preferences>> GetApplicationPreferencesAsync(CancellationToken cancellationToken = default);

        /// <summary>Updates qBittorrent application preferences.</summary>
        /// <param name="preferences">The preference changes to apply.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetApplicationPreferencesAsync(UpdatePreferences preferences, CancellationToken cancellationToken = default);

        /// <summary>Gets the cookies stored by qBittorrent's download manager.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the stored cookies.</returns>
        Task<ApiResult<IReadOnlyList<ApplicationCookie>>> GetApplicationCookiesAsync(CancellationToken cancellationToken = default);

        /// <summary>Replaces the cookies stored by qBittorrent's download manager.</summary>
        /// <param name="cookies">The cookies to persist.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetApplicationCookiesAsync(IEnumerable<ApplicationCookie> cookies, CancellationToken cancellationToken = default);

        /// <summary>Rotates the qBittorrent Web API key.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the new Web API key.</returns>
        Task<ApiResult<ApiKey>> RotateAPIKeyAsync(CancellationToken cancellationToken = default);

        /// <summary>Deletes the qBittorrent Web API key.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> DeleteAPIKeyAsync(CancellationToken cancellationToken = default);

        /// <summary>Requests that qBittorrent send a test email using the configured mail settings.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SendTestEmailAsync(CancellationToken cancellationToken = default);

        /// <summary>Gets the contents of a directory on the qBittorrent host.</summary>
        /// <param name="directoryPath">The absolute directory path to inspect.</param>
        /// <param name="mode">The entry types to include in the result.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the matching directory entries.</returns>
        Task<ApiResult<IReadOnlyList<string>>> GetDirectoryContentAsync(string directoryPath, DirectoryContentMode mode = DirectoryContentMode.All, CancellationToken cancellationToken = default);

        /// <summary>Gets the contents of a directory on the qBittorrent host including metadata for each entry.</summary>
        /// <param name="directoryPath">The absolute directory path to inspect.</param>
        /// <param name="mode">The entry types to include in the result.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the matching directory entries and their metadata.</returns>
        Task<ApiResult<IReadOnlyList<DirectoryContentEntry>>> GetDirectoryContentEntriesAsync(string directoryPath, DirectoryContentMode mode = DirectoryContentMode.All, CancellationToken cancellationToken = default);

        /// <summary>Gets qBittorrent's default save path.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the default save path.</returns>
        Task<ApiResult<string>> GetDefaultSavePathAsync(CancellationToken cancellationToken = default);

        /// <summary>Gets the network interfaces visible to qBittorrent.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the available network interfaces.</returns>
        Task<ApiResult<IReadOnlyList<NetworkInterface>>> GetNetworkInterfacesAsync(CancellationToken cancellationToken = default);

        /// <summary>Gets the bindable addresses for a network interface.</summary>
        /// <param name="interface">The interface identifier. Pass an empty string to return addresses from all interfaces.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the matching interface addresses.</returns>
        Task<ApiResult<IReadOnlyList<string>>> GetNetworkInterfaceAddressListAsync(string @interface, CancellationToken cancellationToken = default);

        #endregion Application

        #region Log

        /// <summary>Gets the qBittorrent main log entries.</summary>
        /// <param name="normal">Whether to include normal log entries.</param>
        /// <param name="info">Whether to include informational log entries.</param>
        /// <param name="warning">Whether to include warning log entries.</param>
        /// <param name="critical">Whether to include critical log entries.</param>
        /// <param name="lastKnownId">The last known log identifier to exclude older entries.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the matching log entries.</returns>
        Task<ApiResult<IReadOnlyList<Log>>> GetLogAsync(bool? normal = null, bool? info = null, bool? warning = null, bool? critical = null, int? lastKnownId = null, CancellationToken cancellationToken = default);

        /// <summary>Gets the qBittorrent peer log entries.</summary>
        /// <param name="lastKnownId">The last known peer-log identifier to exclude older entries.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the matching peer log entries.</returns>
        Task<ApiResult<IReadOnlyList<PeerLog>>> GetPeerLogAsync(int? lastKnownId = null, CancellationToken cancellationToken = default);

        #endregion Log

        #region Sync

        /// <summary>Gets qBittorrent main synchronization data.</summary>
        /// <param name="requestId">The last synchronization response identifier supplied by the caller.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the main synchronization payload.</returns>
        Task<ApiResult<MainData>> GetMainDataAsync(int requestId, CancellationToken cancellationToken = default);

        /// <summary>Gets peer synchronization data for a torrent.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="requestId">The last synchronization response identifier supplied by the caller.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the torrent peer synchronization payload.</returns>
        Task<ApiResult<TorrentPeers>> GetTorrentPeersDataAsync(string hash, int requestId, CancellationToken cancellationToken = default);

        #endregion Sync

        #region Transfer info

        /// <summary>Gets the global transfer statistics.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the current global transfer information.</returns>
        Task<ApiResult<GlobalTransferStatistics>> GetGlobalTransferStatisticsAsync(CancellationToken cancellationToken = default);

        /// <summary>Gets whether alternative speed limits are enabled.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result whose value is <see langword="true" /> when alternative speed limits are enabled; otherwise, <see langword="false" />.</returns>
        Task<ApiResult<bool>> GetAlternativeSpeedLimitsStateAsync(CancellationToken cancellationToken = default);

        /// <summary>Sets whether alternative speed limits are enabled.</summary>
        /// <param name="enabled">The value to apply.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetAlternativeSpeedLimitsStateAsync(bool enabled, CancellationToken cancellationToken = default);

        /// <summary>Toggles the alternative speed-limits state.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> ToggleAlternativeSpeedLimitsAsync(CancellationToken cancellationToken = default);

        /// <summary>Gets the global download limit.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the global download limit in bytes per second.</returns>
        Task<ApiResult<long>> GetGlobalDownloadLimitAsync(CancellationToken cancellationToken = default);

        /// <summary>Sets the global download limit.</summary>
        /// <param name="limit">The limit in bytes per second.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetGlobalDownloadLimitAsync(long limit, CancellationToken cancellationToken = default);

        /// <summary>Gets the global upload limit.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the global upload limit in bytes per second.</returns>
        Task<ApiResult<long>> GetGlobalUploadLimitAsync(CancellationToken cancellationToken = default);

        /// <summary>Sets the global upload limit.</summary>
        /// <param name="limit">The limit in bytes per second.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetGlobalUploadLimitAsync(long limit, CancellationToken cancellationToken = default);

        /// <summary>Bans one or more peers by address.</summary>
        /// <param name="peers">The peers to ban.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> BanPeersAsync(IEnumerable<PeerId> peers, CancellationToken cancellationToken = default);

        #endregion Transfer info

        #region Torrent management

        /// <summary>Gets the torrent list.</summary>
        /// <param name="filter">The torrent-state filter.</param>
        /// <param name="category">The category filter.</param>
        /// <param name="tag">The tag filter.</param>
        /// <param name="sort">The sort column.</param>
        /// <param name="reverse">Whether to reverse the sort order.</param>
        /// <param name="limit">The maximum number of torrents to return.</param>
        /// <param name="offset">The starting result offset.</param>
        /// <param name="isPrivate">Whether to filter by private torrents.</param>
        /// <param name="includeFiles">Whether to include files in the serialized torrent payload.</param>
        /// <param name="includeTrackers">Whether to include trackers in the serialized torrent payload.</param>
        /// <param name="selector">The torrent selection to filter by.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the matching torrents.</returns>
        Task<ApiResult<IReadOnlyList<Torrent>>> GetTorrentListAsync(string? filter = null, string? category = null, string? tag = null, string? sort = null, bool? reverse = null, int? limit = null, int? offset = null, bool? isPrivate = null, bool? includeFiles = null, bool? includeTrackers = null, TorrentSelector? selector = null, CancellationToken cancellationToken = default);

        /// <summary>Gets the total torrent count.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the total torrent count.</returns>
        Task<ApiResult<int>> GetTorrentCountAsync(CancellationToken cancellationToken = default);

        /// <summary>Gets the detailed properties for a torrent.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the torrent properties.</returns>
        Task<ApiResult<TorrentProperties>> GetTorrentPropertiesAsync(string hash, CancellationToken cancellationToken = default);

        /// <summary>Gets the trackers associated with a torrent.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the torrent trackers.</returns>
        Task<ApiResult<IReadOnlyList<TorrentTracker>>> GetTorrentTrackersAsync(string hash, CancellationToken cancellationToken = default);

        /// <summary>Gets the web seeds associated with a torrent.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the torrent web seeds.</returns>
        Task<ApiResult<IReadOnlyList<WebSeed>>> GetTorrentWebSeedsAsync(string hash, CancellationToken cancellationToken = default);

        /// <summary>Adds web seeds to a torrent.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="urls">The web seed URLs to add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> AddTorrentWebSeedsAsync(string hash, IEnumerable<string> urls, CancellationToken cancellationToken = default);

        /// <summary>Replaces a web seed URL on a torrent.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="originalUrl">The existing web seed URL.</param>
        /// <param name="newUrl">The replacement web seed URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> EditTorrentWebSeedAsync(string hash, string originalUrl, string newUrl, CancellationToken cancellationToken = default);

        /// <summary>Removes web seeds from a torrent.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="urls">The web seed URLs to remove.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> RemoveTorrentWebSeedsAsync(string hash, IEnumerable<string> urls, CancellationToken cancellationToken = default);

        /// <summary>Gets the files contained in a torrent.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="indexes">The optional file indexes to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the torrent files.</returns>
        Task<ApiResult<IReadOnlyList<FileData>>> GetTorrentContentsAsync(string hash, IEnumerable<int>? indexes = null, CancellationToken cancellationToken = default);

        /// <summary>Gets the piece states for a torrent.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the torrent piece states.</returns>
        Task<ApiResult<IReadOnlyList<PieceState>>> GetTorrentPieceStatesAsync(string hash, CancellationToken cancellationToken = default);

        /// <summary>Gets the piece hashes for a torrent.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the torrent piece hashes.</returns>
        Task<ApiResult<IReadOnlyList<string>>> GetTorrentPieceHashesAsync(string hash, CancellationToken cancellationToken = default);

        /// <summary>Gets the piece availability counts for a torrent.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the piece availability counts.</returns>
        Task<ApiResult<IReadOnlyList<int>>> GetTorrentPieceAvailabilityAsync(string hash, CancellationToken cancellationToken = default);

        /// <summary>Starts one or more torrents.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> StartTorrentsAsync(TorrentSelector selector, CancellationToken cancellationToken = default);

        /// <summary>Stops one or more torrents.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> StopTorrentsAsync(TorrentSelector selector, CancellationToken cancellationToken = default);

        /// <summary>Deletes one or more torrents.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="deleteFiles">Whether to also delete the torrent content.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> DeleteTorrentsAsync(TorrentSelector selector, bool deleteFiles = false, CancellationToken cancellationToken = default);

        /// <summary>Forces one or more torrents to recheck their data.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> RecheckTorrentsAsync(TorrentSelector selector, CancellationToken cancellationToken = default);

        /// <summary>Forces one or more torrents to reannounce.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="urls">The optional tracker URLs to reannounce against.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> ReannounceTorrentsAsync(TorrentSelector selector, IEnumerable<string>? urls = null, CancellationToken cancellationToken = default);

        /// <summary>Adds one or more torrents.</summary>
        /// <param name="addTorrentParams">The torrent-add parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the outcome of the add-torrent request.</returns>
        Task<ApiResult<AddTorrentResult>> AddTorrentAsync(AddTorrentParams addTorrentParams, CancellationToken cancellationToken = default);

        /// <summary>Adds one or more trackers to one or more torrents.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="urls">The tracker URLs to add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> AddTrackersToTorrentAsync(TorrentSelector selector, IEnumerable<string> urls, CancellationToken cancellationToken = default);

        /// <summary>Edits a tracker entry for a torrent.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="url">The existing tracker URL.</param>
        /// <param name="newUrl">The replacement tracker URL, if any.</param>
        /// <param name="tier">The replacement tracker tier, if any.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> EditTrackerAsync(string hash, string url, string? newUrl = null, int? tier = null, CancellationToken cancellationToken = default);

        /// <summary>Removes one or more trackers from one or more torrents.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="urls">The tracker URLs to remove.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> RemoveTrackersAsync(TorrentSelector selector, IEnumerable<string> urls, CancellationToken cancellationToken = default);

        /// <summary>Adds peers to one or more torrents.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="peers">The peers to connect to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> AddPeersAsync(TorrentSelector selector, IEnumerable<PeerId> peers, CancellationToken cancellationToken = default);

        /// <summary>Moves one or more torrents up in the queue.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> IncreaseTorrentPriorityAsync(TorrentSelector selector, CancellationToken cancellationToken = default);

        /// <summary>Moves one or more torrents down in the queue.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> DecreaseTorrentPriorityAsync(TorrentSelector selector, CancellationToken cancellationToken = default);

        /// <summary>Moves one or more torrents to the top of the queue.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> MaxTorrentPriorityAsync(TorrentSelector selector, CancellationToken cancellationToken = default);

        /// <summary>Moves one or more torrents to the bottom of the queue.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> MinTorrentPriorityAsync(TorrentSelector selector, CancellationToken cancellationToken = default);

        /// <summary>Sets the priority for one or more files within a torrent.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="id">The file identifiers to update.</param>
        /// <param name="priority">The new file priority.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetFilePriorityAsync(string hash, IEnumerable<int> id, Priority priority, CancellationToken cancellationToken = default);

        /// <summary>Gets per-torrent download limits.</summary>
        /// <param name="selector">The torrent selection to query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the per-torrent download limits.</returns>
        Task<ApiResult<IReadOnlyDictionary<string, long>>> GetTorrentDownloadLimitAsync(TorrentSelector selector, CancellationToken cancellationToken = default);

        /// <summary>Sets per-torrent download limits.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="limit">The limit in bytes per second.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetTorrentDownloadLimitAsync(TorrentSelector selector, long limit, CancellationToken cancellationToken = default);

        /// <summary>Sets per-torrent share limits.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="ratioLimit">The ratio limit.</param>
        /// <param name="seedingTimeLimit">The seeding-time limit in whole minutes.</param>
        /// <param name="inactiveSeedingTimeLimit">The inactive-seeding-time limit in whole minutes.</param>
        /// <param name="shareLimitAction">The action to take when limits are reached. Required by qBittorrent Web API 2.12.0 and later, and unsupported before 2.12.0.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetTorrentShareLimitAsync(TorrentSelector selector, float ratioLimit, int seedingTimeLimit, int inactiveSeedingTimeLimit, ShareLimitAction? shareLimitAction = null, CancellationToken cancellationToken = default);

        /// <summary>Gets per-torrent upload limits.</summary>
        /// <param name="selector">The torrent selection to query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the per-torrent upload limits.</returns>
        Task<ApiResult<IReadOnlyDictionary<string, long>>> GetTorrentUploadLimitAsync(TorrentSelector selector, CancellationToken cancellationToken = default);

        /// <summary>Sets per-torrent upload limits.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="limit">The limit in bytes per second.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetTorrentUploadLimitAsync(TorrentSelector selector, long limit, CancellationToken cancellationToken = default);

        /// <summary>Moves one or more torrents to a new save location.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="location">The new save location.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetTorrentLocationAsync(TorrentSelector selector, string location, CancellationToken cancellationToken = default);

        /// <summary>Sets the save path for one or more torrents.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="path">The new save path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetTorrentSavePathAsync(TorrentSelector selector, string path, CancellationToken cancellationToken = default);

        /// <summary>Sets the download path for one or more torrents.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="path">The new download path, or <see langword="null" /> to clear it.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetTorrentDownloadPathAsync(TorrentSelector selector, string? path, CancellationToken cancellationToken = default);

        /// <summary>Renames a torrent.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="name">The new torrent name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetTorrentNameAsync(string hash, string name, CancellationToken cancellationToken = default);

        /// <summary>Sets the comment for one or more torrents.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="comment">The comment to apply.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetTorrentCommentAsync(TorrentSelector selector, string comment, CancellationToken cancellationToken = default);

        /// <summary>Sets the category for one or more torrents.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="category">The category to assign.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetTorrentCategoryAsync(TorrentSelector selector, string category, CancellationToken cancellationToken = default);

        /// <summary>Gets all torrent categories.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the configured categories.</returns>
        Task<ApiResult<IReadOnlyDictionary<string, Category>>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);

        /// <summary>Creates a torrent category.</summary>
        /// <param name="category">The category name.</param>
        /// <param name="savePath">The category save path.</param>
        /// <param name="downloadPathOption">The optional download-path settings.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> AddCategoryAsync(string category, string savePath, DownloadPathOption? downloadPathOption = null, CancellationToken cancellationToken = default);

        /// <summary>Updates a torrent category.</summary>
        /// <param name="category">The category name.</param>
        /// <param name="savePath">The category save path.</param>
        /// <param name="downloadPathOption">The optional download-path settings.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> EditCategoryAsync(string category, string savePath, DownloadPathOption? downloadPathOption = null, CancellationToken cancellationToken = default);

        /// <summary>Removes one or more torrent categories.</summary>
        /// <param name="categories">The categories to remove.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> RemoveCategoriesAsync(IEnumerable<string> categories, CancellationToken cancellationToken = default);

        /// <summary>Adds tags to one or more torrents.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="tags">The tags to add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> AddTorrentTagsAsync(TorrentSelector selector, IEnumerable<string> tags, CancellationToken cancellationToken = default);

        /// <summary>Replaces the tags on one or more torrents.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="tags">The tags to assign.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetTorrentTagsAsync(TorrentSelector selector, IEnumerable<string> tags, CancellationToken cancellationToken = default);

        /// <summary>Removes tags from one or more torrents.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="tags">The tags to remove.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> RemoveTorrentTagsAsync(TorrentSelector selector, IEnumerable<string> tags, CancellationToken cancellationToken = default);

        /// <summary>Gets all defined torrent tags.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the configured tags.</returns>
        Task<ApiResult<IReadOnlyList<string>>> GetAllTagsAsync(CancellationToken cancellationToken = default);

        /// <summary>Creates one or more torrent tags.</summary>
        /// <param name="tags">The tags to create.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> CreateTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default);

        /// <summary>Deletes one or more torrent tags.</summary>
        /// <param name="tags">The tags to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> DeleteTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default);

        /// <summary>Sets automatic torrent management on one or more torrents.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="enable">Whether automatic torrent management should be enabled.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetAutomaticTorrentManagementAsync(TorrentSelector selector, bool enable, CancellationToken cancellationToken = default);

        /// <summary>Toggles sequential download on one or more torrents.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> ToggleSequentialDownloadAsync(TorrentSelector selector, CancellationToken cancellationToken = default);

        /// <summary>Toggles first-and-last-piece priority on one or more torrents.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetFirstLastPiecePriorityAsync(TorrentSelector selector, CancellationToken cancellationToken = default);

        /// <summary>Sets force-start mode on one or more torrents.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="value">Whether force-start should be enabled.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetForceStartAsync(TorrentSelector selector, bool value, CancellationToken cancellationToken = default);

        /// <summary>Sets super-seeding mode on one or more torrents.</summary>
        /// <param name="selector">The torrent selection to target.</param>
        /// <param name="value">Whether super-seeding should be enabled.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetSuperSeedingAsync(TorrentSelector selector, bool value, CancellationToken cancellationToken = default);

        /// <summary>Renames a file inside a torrent.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="oldPath">The existing file path inside the torrent.</param>
        /// <param name="newPath">The replacement file path inside the torrent.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> RenameFileAsync(string hash, string oldPath, string newPath, CancellationToken cancellationToken = default);

        /// <summary>Renames a folder inside a torrent.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="oldPath">The existing folder path inside the torrent.</param>
        /// <param name="newPath">The replacement folder path inside the torrent.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> RenameFolderAsync(string hash, string oldPath, string newPath, CancellationToken cancellationToken = default);

        /// <summary>Builds the export URL for a torrent file.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <returns>A result with the fully-qualified export URL.</returns>
        /// <remarks>This method only builds the export URL. qBittorrent is contacted when the returned URL is requested.</remarks>
        Task<ApiResult<string>> GetExportUrlAsync(string hash);

        /// <summary>Downloads the exported torrent file for a hash.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the exported torrent file bytes.</returns>
        Task<ApiResult<byte[]>> ExportTorrentAsync(string hash, CancellationToken cancellationToken = default);

        /// <summary>Gets the SSL parameters associated with a torrent.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the torrent SSL parameters.</returns>
        Task<ApiResult<SslParameters>> GetTorrentSslParametersAsync(string hash, CancellationToken cancellationToken = default);

        /// <summary>Sets the SSL parameters associated with a torrent.</summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="parameters">The SSL parameters to apply.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetTorrentSslParametersAsync(string hash, SslParameters parameters, CancellationToken cancellationToken = default);

        /// <summary>Fetches resolved torrent metadata for a URI or hash.</summary>
        /// <param name="source">The torrent source URI or hash.</param>
        /// <param name="downloader">The optional search plugin downloader to use.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the resolved torrent metadata. When qBittorrent accepts the request but has not completed it yet, the result fails with <see cref="ApiFailureKind.OperationPending" />.</returns>
        Task<ApiResult<TorrentMetadata>> FetchTorrentMetadataAsync(string source, string? downloader = null, CancellationToken cancellationToken = default);

        /// <summary>Parses torrent metadata from uploaded torrent files.</summary>
        /// <param name="torrents">The torrent files keyed by file name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the parsed torrent metadata in request order.</returns>
        Task<ApiResult<IReadOnlyList<TorrentMetadata>>> ParseTorrentMetadataAsync(IReadOnlyDictionary<string, Stream> torrents, CancellationToken cancellationToken = default);

        /// <summary>Saves previously fetched torrent metadata as a torrent file.</summary>
        /// <param name="source">The torrent source URI or hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the torrent file bytes.</returns>
        Task<ApiResult<byte[]>> SaveTorrentMetadataAsync(string source, CancellationToken cancellationToken = default);

        #endregion Torrent management

        #region Torrent creator

        /// <summary>Creates a torrent-creation task.</summary>
        /// <param name="request">The torrent-creation request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the created torrent-creation task identifier.</returns>
        Task<ApiResult<string>> AddTorrentCreationTaskAsync(TorrentCreationTaskRequest request, CancellationToken cancellationToken = default);

        /// <summary>Gets torrent-creation tasks.</summary>
        /// <param name="taskId">The optional task identifier to query. When omitted, all tasks are returned.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the matching torrent-creation tasks.</returns>
        Task<ApiResult<IReadOnlyList<TorrentCreationTaskStatus>>> GetTorrentCreationTasksAsync(string? taskId = null, CancellationToken cancellationToken = default);

        /// <summary>Gets the generated torrent file for a torrent-creation task.</summary>
        /// <param name="taskId">The torrent-creation task identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the generated torrent file bytes.</returns>
        Task<ApiResult<byte[]>> GetTorrentCreationTaskFileAsync(string taskId, CancellationToken cancellationToken = default);

        /// <summary>Deletes a torrent-creation task.</summary>
        /// <param name="taskId">The torrent-creation task identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> DeleteTorrentCreationTaskAsync(string taskId, CancellationToken cancellationToken = default);

        #endregion Torrent creator

        #region RSS

        /// <summary>Creates an RSS folder.</summary>
        /// <param name="path">The RSS folder path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> AddRssFolderAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>Creates an RSS feed.</summary>
        /// <param name="url">The RSS feed URL.</param>
        /// <param name="path">The RSS path to place the feed under, or <see langword="null" /> to let qBittorrent derive one.</param>
        /// <param name="refreshInterval">The optional refresh interval in seconds.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> AddRssFeedAsync(string url, string? path = null, long? refreshInterval = null, CancellationToken cancellationToken = default);

        /// <summary>Removes an RSS folder or feed.</summary>
        /// <param name="path">The RSS item path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> RemoveRssItemAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>Moves an RSS item.</summary>
        /// <param name="itemPath">The RSS item path.</param>
        /// <param name="destPath">The destination RSS path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> MoveRssItemAsync(string itemPath, string destPath, CancellationToken cancellationToken = default);

        /// <summary>Updates the source URL for an RSS feed.</summary>
        /// <param name="path">The RSS feed path.</param>
        /// <param name="url">The replacement feed URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetRssFeedUrlAsync(string path, string url, CancellationToken cancellationToken = default);

        /// <summary>Updates the refresh interval for an RSS feed.</summary>
        /// <param name="path">The RSS feed path.</param>
        /// <param name="refreshInterval">The refresh interval in seconds.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetRssFeedRefreshIntervalAsync(string path, long refreshInterval, CancellationToken cancellationToken = default);

        /// <summary>Gets all RSS items.</summary>
        /// <param name="withData">Whether to include full article data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the RSS item tree.</returns>
        Task<ApiResult<IReadOnlyDictionary<string, RssItem>>> GetAllRssItemsAsync(bool? withData = null, CancellationToken cancellationToken = default);

        /// <summary>Marks an RSS item or article as read.</summary>
        /// <param name="itemPath">The RSS item path.</param>
        /// <param name="articleId">The optional article identifier for feed articles.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> MarkRssItemAsReadAsync(string itemPath, string? articleId = null, CancellationToken cancellationToken = default);

        /// <summary>Refreshes an RSS item.</summary>
        /// <param name="itemPath">The RSS item path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> RefreshRssItemAsync(string itemPath, CancellationToken cancellationToken = default);

        /// <summary>Creates or updates an RSS auto-downloading rule.</summary>
        /// <param name="ruleName">The rule name.</param>
        /// <param name="ruleDef">The rule definition.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> SetRssAutoDownloadingRuleAsync(string ruleName, AutoDownloadingRule ruleDef, CancellationToken cancellationToken = default);

        /// <summary>Renames an RSS auto-downloading rule.</summary>
        /// <param name="ruleName">The existing rule name.</param>
        /// <param name="newRuleName">The new rule name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> RenameRssAutoDownloadingRuleAsync(string ruleName, string newRuleName, CancellationToken cancellationToken = default);

        /// <summary>Removes an RSS auto-downloading rule.</summary>
        /// <param name="ruleName">The rule name to remove.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> RemoveRssAutoDownloadingRuleAsync(string ruleName, CancellationToken cancellationToken = default);

        /// <summary>Gets all RSS auto-downloading rules.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the configured auto-downloading rules.</returns>
        Task<ApiResult<IReadOnlyDictionary<string, AutoDownloadingRule>>> GetAllRssAutoDownloadingRulesAsync(CancellationToken cancellationToken = default);

        /// <summary>Gets RSS articles that match an auto-downloading rule.</summary>
        /// <param name="ruleName">The rule name to evaluate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the matching article titles grouped by feed.</returns>
        Task<ApiResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>> GetRssMatchingArticlesAsync(string ruleName, CancellationToken cancellationToken = default);

        #endregion RSS

        #region Search

        /// <summary>Starts a search.</summary>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="plugins">The plugins to search with.</param>
        /// <param name="category">The search category.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the created search identifier.</returns>
        Task<ApiResult<int>> StartSearchAsync(string pattern, IEnumerable<string> plugins, string category = "all", CancellationToken cancellationToken = default);

        /// <summary>Stops a search.</summary>
        /// <param name="id">The search identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> StopSearchAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>Gets the status of a search.</summary>
        /// <param name="id">The search identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the search status, or <see langword="null" /> when the search no longer exists.</returns>
        Task<ApiResult<SearchStatus?>> GetSearchStatusAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>Gets the status of all retained searches.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the search statuses.</returns>
        Task<ApiResult<IReadOnlyList<SearchStatus>>> GetSearchesStatusAsync(CancellationToken cancellationToken = default);

        /// <summary>Gets the results for a search.</summary>
        /// <param name="id">The search identifier.</param>
        /// <param name="limit">The maximum number of results to return.</param>
        /// <param name="offset">The result offset.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the search results.</returns>
        Task<ApiResult<SearchResults>> GetSearchResultsAsync(int id, int? limit = null, int? offset = null, CancellationToken cancellationToken = default);

        /// <summary>Deletes a retained search.</summary>
        /// <param name="id">The search identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> DeleteSearchAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>Gets the installed search plugins.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result with the installed search plugins.</returns>
        Task<ApiResult<IReadOnlyList<SearchPlugin>>> GetSearchPluginsAsync(CancellationToken cancellationToken = default);

        /// <summary>Installs one or more search plugins.</summary>
        /// <param name="sources">The plugin sources to install.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> InstallSearchPluginsAsync(IEnumerable<string> sources, CancellationToken cancellationToken = default);

        /// <summary>Uninstalls one or more search plugins.</summary>
        /// <param name="names">The plugin names to uninstall.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> UninstallSearchPluginsAsync(IEnumerable<string> names, CancellationToken cancellationToken = default);

        /// <summary>Enables one or more search plugins.</summary>
        /// <param name="names">The plugin names to enable.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> EnableSearchPluginsAsync(IEnumerable<string> names, CancellationToken cancellationToken = default);

        /// <summary>Disables one or more search plugins.</summary>
        /// <param name="names">The plugin names to disable.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> DisableSearchPluginsAsync(IEnumerable<string> names, CancellationToken cancellationToken = default);

        /// <summary>Downloads a search result into qBittorrent.</summary>
        /// <param name="pluginName">The search plugin name.</param>
        /// <param name="torrentUrl">The torrent or magnet URL to download.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> DownloadSearchResultAsync(string pluginName, string torrentUrl, CancellationToken cancellationToken = default);

        /// <summary>Requests an update check for installed search plugins.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A result indicating whether the operation succeeded.</returns>
        Task<ApiResult> UpdateSearchPluginsAsync(CancellationToken cancellationToken = default);

        #endregion Search
    }
}
