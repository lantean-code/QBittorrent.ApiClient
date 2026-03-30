using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient
{
    /// <summary>
    /// Provides convenience extension methods for common qBittorrent API client operations.
    /// </summary>
    public static class ApiClientExtensions
    {
        /// <summary>
        /// Stops a single torrent.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> StopTorrentAsync(this IApiClient apiClient, string hash, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).StopTorrentsAsync(GetTorrentSelector(hash, nameof(hash)), cancellationToken);
        }

        /// <summary>
        /// Stops multiple torrents.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> StopTorrentsAsync(this IApiClient apiClient, IEnumerable<string> hashes, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).StopTorrentsAsync(GetTorrentSelector(hashes, nameof(hashes)), cancellationToken);
        }

        /// <summary>
        /// Stops all torrents.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> StopAllTorrentsAsync(this IApiClient apiClient, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).StopTorrentsAsync(TorrentSelector.AllTorrents(), cancellationToken);
        }

        /// <summary>
        /// Starts a single torrent.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> StartTorrentAsync(this IApiClient apiClient, string hash, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).StartTorrentsAsync(GetTorrentSelector(hash, nameof(hash)), cancellationToken);
        }

        /// <summary>
        /// Starts multiple torrents.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> StartTorrentsAsync(this IApiClient apiClient, IEnumerable<string> hashes, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).StartTorrentsAsync(GetTorrentSelector(hashes, nameof(hashes)), cancellationToken);
        }

        /// <summary>
        /// Starts all torrents.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> StartAllTorrentsAsync(this IApiClient apiClient, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).StartTorrentsAsync(TorrentSelector.AllTorrents(), cancellationToken);
        }

        /// <summary>
        /// Deletes a single torrent.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="deleteFiles">Whether to delete downloaded files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> DeleteTorrentAsync(this IApiClient apiClient, string hash, bool deleteFiles, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).DeleteTorrentsAsync(GetTorrentSelector(hash, nameof(hash)), deleteFiles, cancellationToken);
        }

        /// <summary>
        /// Deletes multiple torrents.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="deleteFiles">Whether to delete downloaded files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> DeleteTorrentsAsync(this IApiClient apiClient, IEnumerable<string> hashes, bool deleteFiles, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).DeleteTorrentsAsync(GetTorrentSelector(hashes, nameof(hashes)), deleteFiles, cancellationToken);
        }

        /// <summary>
        /// Deletes all torrents.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="deleteFiles">Whether to delete downloaded files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> DeleteAllTorrentsAsync(this IApiClient apiClient, bool deleteFiles, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).DeleteTorrentsAsync(TorrentSelector.AllTorrents(), deleteFiles, cancellationToken);
        }

        /// <summary>
        /// Gets a single torrent by hash.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that returns the matching torrent, or <see langword="null" /> when no torrent matches.</returns>
        public static async Task<ApiResult<Torrent?>> GetTorrentAsync(this IApiClient apiClient, string hash, CancellationToken cancellationToken = default)
        {
            var torrents = await GetRequiredApiClient(apiClient).GetTorrentListAsync(cancellationToken: cancellationToken, selector: GetTorrentSelector(hash, nameof(hash)));
            if (!torrents.TryGetValue(out var torrentList))
            {
                return torrents.Failure.ToResult<Torrent?>();
            }

            if (torrentList.Count == 0)
            {
                return ApiResult<Torrent?>.Success(null);
            }

            return ApiResult<Torrent?>.Success(torrentList[0]);
        }

        /// <summary>
        /// Sets the category for a single torrent.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="category">The category to apply.</param>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> SetTorrentCategoryAsync(this IApiClient apiClient, string category, string hash, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).SetTorrentCategoryAsync(GetTorrentSelector(hash, nameof(hash)), GetRequiredString(category, nameof(category)), cancellationToken);
        }

        /// <summary>
        /// Sets the category for multiple torrents.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="category">The category to apply.</param>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> SetTorrentCategoryAsync(this IApiClient apiClient, string category, IEnumerable<string> hashes, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).SetTorrentCategoryAsync(GetTorrentSelector(hashes, nameof(hashes)), GetRequiredString(category, nameof(category)), cancellationToken);
        }

        /// <summary>
        /// Removes the category from a single torrent.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> RemoveTorrentCategoryAsync(this IApiClient apiClient, string hash, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).SetTorrentCategoryAsync(GetTorrentSelector(hash, nameof(hash)), string.Empty, cancellationToken);
        }

        /// <summary>
        /// Removes the category from multiple torrents.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> RemoveTorrentCategoryAsync(this IApiClient apiClient, IEnumerable<string> hashes, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).SetTorrentCategoryAsync(GetTorrentSelector(hashes, nameof(hashes)), string.Empty, cancellationToken);
        }

        /// <summary>
        /// Removes tags from a single torrent.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="tags">The tags to remove.</param>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> RemoveTorrentTagsAsync(this IApiClient apiClient, IEnumerable<string> tags, string hash, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).RemoveTorrentTagsAsync(GetTorrentSelector(hash, nameof(hash)), GetRequiredStrings(tags, nameof(tags)), cancellationToken);
        }

        /// <summary>
        /// Removes tags from multiple torrents.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="tags">The tags to remove.</param>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> RemoveTorrentTagsAsync(this IApiClient apiClient, IEnumerable<string> tags, IEnumerable<string> hashes, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).RemoveTorrentTagsAsync(GetTorrentSelector(hashes, nameof(hashes)), GetRequiredStrings(tags, nameof(tags)), cancellationToken);
        }

        /// <summary>
        /// Removes a single tag from a torrent.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="tag">The tag to remove.</param>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> RemoveTorrentTagAsync(this IApiClient apiClient, string tag, string hash, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).RemoveTorrentTagsAsync(
                GetTorrentSelector(hash, nameof(hash)),
                [GetRequiredString(tag, nameof(tag))],
                cancellationToken);
        }

        /// <summary>
        /// Removes a single tag from multiple torrents.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="tag">The tag to remove.</param>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> RemoveTorrentTagAsync(this IApiClient apiClient, string tag, IEnumerable<string> hashes, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).RemoveTorrentTagsAsync(GetTorrentSelector(hashes, nameof(hashes)), [GetRequiredString(tag, nameof(tag))], cancellationToken);
        }

        /// <summary>
        /// Adds tags to a single torrent.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="tags">The tags to add.</param>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> AddTorrentTagsAsync(this IApiClient apiClient, IEnumerable<string> tags, string hash, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).AddTorrentTagsAsync(GetTorrentSelector(hash, nameof(hash)), GetRequiredStrings(tags, nameof(tags)), cancellationToken);
        }

        /// <summary>
        /// Adds tags to multiple torrents.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="tags">The tags to add.</param>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> AddTorrentTagsAsync(this IApiClient apiClient, IEnumerable<string> tags, IEnumerable<string> hashes, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).AddTorrentTagsAsync(GetTorrentSelector(hashes, nameof(hashes)), GetRequiredStrings(tags, nameof(tags)), cancellationToken);
        }

        /// <summary>
        /// Adds a single tag to a torrent.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="tag">The tag to add.</param>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> AddTorrentTagAsync(this IApiClient apiClient, string tag, string hash, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).AddTorrentTagsAsync(
                GetTorrentSelector(hash, nameof(hash)),
                [GetRequiredString(tag, nameof(tag))],
                cancellationToken);
        }

        /// <summary>
        /// Adds a single tag to multiple torrents.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="tag">The tag to add.</param>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> AddTorrentTagAsync(this IApiClient apiClient, string tag, IEnumerable<string> hashes, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).AddTorrentTagsAsync(GetTorrentSelector(hashes, nameof(hashes)), [GetRequiredString(tag, nameof(tag))], cancellationToken);
        }

        /// <summary>
        /// Rechecks a single torrent.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> RecheckTorrentAsync(this IApiClient apiClient, string hash, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).RecheckTorrentsAsync(GetTorrentSelector(hash, nameof(hash)), cancellationToken);
        }

        /// <summary>
        /// Reannounces a single torrent.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<ApiResult> ReannounceTorrentAsync(this IApiClient apiClient, string hash, CancellationToken cancellationToken = default)
        {
            return GetRequiredApiClient(apiClient).ReannounceTorrentsAsync(GetTorrentSelector(hash, nameof(hash)), cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Removes categories that are no longer assigned to any torrents.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that returns the removed category names.</returns>
        public static async Task<ApiResult<IEnumerable<string>>> RemoveUnusedCategoriesAsync(this IApiClient apiClient, CancellationToken cancellationToken = default)
        {
            var validatedApiClient = GetRequiredApiClient(apiClient);
            var torrents = await validatedApiClient.GetTorrentListAsync(cancellationToken: cancellationToken);
            var categories = await validatedApiClient.GetAllCategoriesAsync(cancellationToken);
            if (!torrents.TryGetValue(out var torrentList))
            {
                return torrents.Failure.ToResult<IEnumerable<string>>();
            }

            if (!categories.TryGetValue(out var categoryDictionary))
            {
                return categories.Failure.ToResult<IEnumerable<string>>();
            }

            var selectedCategories = torrentList.Select(t => t.Category).Distinct().ToList();

            var unusedCategories = categoryDictionary.Values.Select(v => v.Name).Except(selectedCategories).Where(v => v is not null).Select(v => v!).ToArray();

            var removeResult = await validatedApiClient.RemoveCategoriesAsync(unusedCategories, cancellationToken);
            if (!removeResult.IsSuccess)
            {
                return removeResult.Failure.ToResult<IEnumerable<string>>();
            }

            return ApiResult<IEnumerable<string>>.Success(unusedCategories);
        }

        /// <summary>
        /// Removes tags that are no longer assigned to any torrents.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that returns the removed tags.</returns>
        public static async Task<ApiResult<IEnumerable<string>>> RemoveUnusedTagsAsync(this IApiClient apiClient, CancellationToken cancellationToken = default)
        {
            var validatedApiClient = GetRequiredApiClient(apiClient);
            var torrents = await validatedApiClient.GetTorrentListAsync(cancellationToken: cancellationToken);
            var tags = await validatedApiClient.GetAllTagsAsync(cancellationToken);
            if (!torrents.TryGetValue(out var torrentList))
            {
                return torrents.Failure.ToResult<IEnumerable<string>>();
            }

            if (!tags.TryGetValue(out var tagList))
            {
                return tags.Failure.ToResult<IEnumerable<string>>();
            }

            var selectedTags = torrentList.Where(t => t.Tags is not null).SelectMany(t => t.Tags!).Distinct().ToList();

            var unusedTags = tagList.Except(selectedTags).ToArray();

            var deleteResult = await validatedApiClient.DeleteTagsAsync(unusedTags, cancellationToken);
            if (!deleteResult.IsSuccess)
            {
                return deleteResult.Failure.ToResult<IEnumerable<string>>();
            }

            return ApiResult<IEnumerable<string>>.Success(unusedTags);
        }

        private static IApiClient GetRequiredApiClient(IApiClient apiClient)
        {
            ArgumentNullException.ThrowIfNull(apiClient);
            return apiClient;
        }

        private static string GetRequiredString(string value, string paramName)
        {
            ArgumentNullException.ThrowIfNull(value, paramName);
            return value;
        }

        private static string[] GetRequiredStrings(IEnumerable<string> values, string paramName)
        {
            ArgumentNullException.ThrowIfNull(values, paramName);

            return values.Select(
                    value => value ?? throw new ArgumentException("Collection items cannot be null.", paramName))
                .ToArray();
        }

        private static TorrentSelector GetTorrentSelector(string hash, string paramName)
        {
            return TorrentSelector.FromHash(GetRequiredString(hash, paramName));
        }

        private static TorrentSelector GetTorrentSelector(IEnumerable<string> hashes, string paramName)
        {
            return TorrentSelector.FromHashes(GetRequiredStrings(hashes, paramName));
        }
    }
}
