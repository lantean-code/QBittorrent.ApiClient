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
        /// <returns>A task that returns the matching torrent.</returns>
        public static async Task<ApiResult<Torrent>> GetTorrentAsync(this IApiClient apiClient, string hash, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(apiClient);

            var torrents = await apiClient.GetTorrentListAsync(selector: TorrentSelector.FromHash(hash), cancellationToken: cancellationToken);
            if (torrents.IsFailure)
            {
                return torrents.Failure.ToResult<Torrent>();
            }

            var torrentList = torrents.Value;

            if (torrentList.Count == 0)
            {
                return new ApiFailure
                {
                    Kind = ApiFailureKind.NotFound,
                    Operation = nameof(GetTorrentAsync),
                    UserMessage = "The torrent could not be found.",
                }.ToResult<Torrent>();
            }

            return ApiResult.CreateSuccess(torrentList[0]);
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
            if (torrents.IsFailure)
            {
                return torrents.Failure.ToResult<IEnumerable<string>>();
            }

            if (categories.IsFailure)
            {
                return categories.Failure.ToResult<IEnumerable<string>>();
            }

            var torrentList = torrents.Value;
            var categoryDictionary = categories.Value;

            var selectedCategories = torrentList.Select(t => t.Category).Distinct().ToList();

            var unusedCategories = categoryDictionary.Values.Select(v => v.Name).Except(selectedCategories).Where(v => v is not null).Select(v => v!).ToArray();

            var removeResult = await apiClient.RemoveCategoriesAsync(unusedCategories, cancellationToken);
            if (removeResult.IsFailure)
            {
                return removeResult.Failure.ToResult<IEnumerable<string>>();
            }

            return ApiResult.CreateSuccess<IEnumerable<string>>(unusedCategories);
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
            if (torrents.IsFailure)
            {
                return torrents.Failure.ToResult<IEnumerable<string>>();
            }

            if (tags.IsFailure)
            {
                return tags.Failure.ToResult<IEnumerable<string>>();
            }

            var torrentList = torrents.Value;
            var tagList = tags.Value;

            var selectedTags = torrentList.Where(t => t.Tags is not null).SelectMany(t => t.Tags!).Distinct().ToList();

            var unusedTags = tagList.Except(selectedTags).ToArray();

            var deleteResult = await apiClient.DeleteTagsAsync(unusedTags, cancellationToken);
            if (deleteResult.IsFailure)
            {
                return deleteResult.Failure.ToResult<IEnumerable<string>>();
            }

            return ApiResult.CreateSuccess<IEnumerable<string>>(unusedTags);
        }
    }
}
