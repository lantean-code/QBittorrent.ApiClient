using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient
{
    /// <summary>
    /// Provides convenience extension methods for common qBittorrent API client operations.
    /// </summary>
    public static class ApiClientExtensions
    {
        /// <summary>
        /// Gets a single torrent by hash.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that returns the matching torrent, or <see langword="null" /> when no torrent matches.</returns>
        public static async Task<ApiResult<Torrent?>> GetTorrentAsync(this IApiClient apiClient, string hash, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(apiClient);

            var torrents = await apiClient.GetTorrentListAsync(selector: TorrentSelector.FromHash(hash), cancellationToken: cancellationToken);
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
        /// Removes categories that are no longer assigned to any torrents.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that returns the removed category names.</returns>
        public static async Task<ApiResult<IEnumerable<string>>> RemoveUnusedCategoriesAsync(this IApiClient apiClient, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(apiClient);

            var torrents = await apiClient.GetTorrentListAsync(cancellationToken: cancellationToken);
            var categories = await apiClient.GetAllCategoriesAsync(cancellationToken);
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

            var removeResult = await apiClient.RemoveCategoriesAsync(unusedCategories, cancellationToken);
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
            ArgumentNullException.ThrowIfNull(apiClient);

            var torrents = await apiClient.GetTorrentListAsync(cancellationToken: cancellationToken);
            var tags = await apiClient.GetAllTagsAsync(cancellationToken);
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

            var deleteResult = await apiClient.DeleteTagsAsync(unusedTags, cancellationToken);
            if (!deleteResult.IsSuccess)
            {
                return deleteResult.Failure.ToResult<IEnumerable<string>>();
            }

            return ApiResult<IEnumerable<string>>.Success(unusedTags);
        }
    }
}
