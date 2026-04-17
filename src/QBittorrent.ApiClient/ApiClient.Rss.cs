using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient
{
    internal partial class ApiClient
    {
        public Task<ApiResult> AddRssFolderAsync(string path, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("path", path)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("rss/addFolder", content, ct), cancellationToken: cancellationToken);
        }

        public async Task<ApiResult> AddRssFeedAsync(string url, string? path = null, long? refreshInterval = null, CancellationToken cancellationToken = default)
        {
            if (refreshInterval is not null)
            {
                var profile = CompatibilityProfile;
                if (!profile.SupportsRssFeedRefreshInterval)
                {
                    return CreateUnsupportedCompatibilityFailure(
                        nameof(AddRssFeedAsync),
                        profile,
                        $"qBittorrent Web API {profile.WebApiVersion} does not support RSS feed refresh intervals.").ToResult();
                }
            }

            var content = new FormUrlEncodedBuilder()
                .Add("url", url)
                .Add("path", path ?? string.Empty);

            if (refreshInterval is not null)
            {
                content.Add("refreshInterval", refreshInterval.Value);
            }

            return await ExecuteAsync(ct => _httpClient.PostAsync("rss/addFeed", content.ToFormUrlEncodedContent(), ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> RemoveRssItemAsync(string path, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("path", path)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("rss/removeItem", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> MoveRssItemAsync(string itemPath, string destPath, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("itemPath", itemPath)
                .Add("destPath", destPath)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("rss/moveItem", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetRssFeedUrlAsync(string path, string url, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("path", path)
                .Add("url", url)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("rss/setFeedURL", content, ct), cancellationToken: cancellationToken);
        }

        public async Task<ApiResult> SetRssFeedRefreshIntervalAsync(string path, long refreshInterval, CancellationToken cancellationToken = default)
        {
            var profile = CompatibilityProfile;
            if (!profile.SupportsRssFeedRefreshInterval)
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(SetRssFeedRefreshIntervalAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support RSS feed refresh intervals.").ToResult();
            }

            var content = new FormUrlEncodedBuilder()
                .Add("path", path)
                .Add("refreshInterval", refreshInterval)
                .ToFormUrlEncodedContent();

            return await ExecuteAsync(ct => _httpClient.PostAsync("rss/setFeedRefreshInterval", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyDictionary<string, RssItem>>> GetAllRssItemsAsync(bool? withData = null, CancellationToken cancellationToken = default)
        {
            var content = new QueryBuilder()
                .AddIfNotNullOrEmpty("withData", withData);

            return ExecuteAsync(
                ct => _httpClient.GetAsync("rss/items", content, ct),
                GetJsonDictionaryAsync<string, RssItem>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> MarkRssItemAsReadAsync(string itemPath, string? articleId = null, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("itemPath", itemPath)
                .AddIfNotNullOrEmpty("articleId", articleId)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("rss/markAsRead", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> RefreshRssItemAsync(string itemPath, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("itemPath", itemPath)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("rss/refreshItem", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetRssAutoDownloadingRuleAsync(string ruleName, AutoDownloadingRule ruleDef, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("ruleName", ruleName)
                .Add("ruleDef", SerializeJson(ruleDef))
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("rss/setRule", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> RenameRssAutoDownloadingRuleAsync(string ruleName, string newRuleName, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("ruleName", ruleName)
                .Add("newRuleName", newRuleName)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("rss/renameRule", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> RemoveRssAutoDownloadingRuleAsync(string ruleName, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("ruleName", ruleName)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("rss/removeRule", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyDictionary<string, AutoDownloadingRule>>> GetAllRssAutoDownloadingRulesAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync("rss/rules", ct),
                GetJsonDictionaryAsync<string, AutoDownloadingRule>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>> GetRssMatchingArticlesAsync(string ruleName, CancellationToken cancellationToken = default)
        {
            var query = new QueryBuilder()
                .Add("ruleName", ruleName);

            static async Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> readMatchingArticles(HttpContent content, CancellationToken currentCancellationToken)
            {
                var dictionary = await GetJsonDictionaryAsync<string, List<string>>(content, currentCancellationToken);
                return dictionary.ToDictionary(d => d.Key, d => (IReadOnlyList<string>)d.Value.AsReadOnly()).AsReadOnly();
            }

            return ExecuteAsync<IReadOnlyDictionary<string, IReadOnlyList<string>>>(
                ct => _httpClient.GetAsync($"rss/matchingArticles{query}", ct),
                readMatchingArticles,
                cancellationToken: cancellationToken);
        }
    }
}
