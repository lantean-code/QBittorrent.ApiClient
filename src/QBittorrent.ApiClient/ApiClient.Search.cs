using System.Net;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient
{
    internal partial class ApiClient
    {
        public Task<ApiResult<int>> StartSearchAsync(string pattern, IEnumerable<string> plugins, string category = "all", CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("pattern", pattern)
                .AddPipeSeparated("plugins", plugins)
                .Add("category", category)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("search/start", content, ct),
                ReadSearchIdentifierAsync,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> StopSearchAsync(int id, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("id", id)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("search/stop", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult<SearchStatus?>> GetSearchStatusAsync(int id, CancellationToken cancellationToken = default)
        {
            var query = new QueryBuilder();
            query.Add("id", id);

            static async Task<ApiResult<SearchStatus?>> handleSearchStatusResponse(HttpResponseMessage response, string operation, CancellationToken currentCancellationToken)
            {
                using (response)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return ApiResult<SearchStatus?>.Success(null);
                    }

                    return await CreateResultAsync(operation, response, readSearchStatus, currentCancellationToken);
                }

                static async Task<SearchStatus?> readSearchStatus(HttpContent content, CancellationToken readCancellationToken)
                {
                    var statuses = await GetJsonListAsync<SearchStatus>(content, readCancellationToken);
                    return statuses.Count > 0 ? statuses[0] : null;
                }
            }

            return ExecuteAsync(
                ct => _httpClient.GetAsync($"search/status{query}", ct),
                handleSearchStatusResponse,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyList<SearchStatus>>> GetSearchesStatusAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync("search/status", ct),
                GetJsonListAsync<SearchStatus>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<SearchResults>> GetSearchResultsAsync(int id, int? limit = null, int? offset = null, CancellationToken cancellationToken = default)
        {
            var query = new QueryBuilder();
            query.Add("id", id);
            if (limit is not null)
            {
                query.Add("limit", limit.Value);
            }
            if (offset is not null)
            {
                query.Add("offset", offset.Value);
            }

            static Task<ApiResult<SearchResults>> handleSearchResultsResponse(HttpResponseMessage response, string operation, CancellationToken currentCancellationToken)
            {
                return CreateResultAsync(operation, response, readSearchResults, currentCancellationToken, createSearchResultsFailure);

                static Task<SearchResults> readSearchResults(HttpContent content, CancellationToken readCancellationToken)
                {
                    return GetJsonAsync<SearchResults>(content, readCancellationToken);
                }

                ApiFailure? createSearchResultsFailure(HttpStatusCode statusCode, string? responseBody)
                {
                    return statusCode switch
                    {
                        HttpStatusCode.NotFound => new ApiFailure
                        {
                            Kind = ApiFailureKind.NotFound,
                            Operation = operation,
                            StatusCode = statusCode,
                            UserMessage = responseBody ?? "The search job could not be found.",
                            Detail = responseBody,
                            Reason = SearchFailureReason.SearchMissing,
                            ResponseBody = responseBody,
                        },
                        HttpStatusCode.Conflict => new ApiFailure
                        {
                            Kind = ApiFailureKind.Conflict,
                            Operation = operation,
                            StatusCode = statusCode,
                            UserMessage = responseBody ?? "The requested search result range is invalid.",
                            Detail = responseBody,
                            Reason = SearchFailureReason.OffsetOutOfRange,
                            ResponseBody = responseBody,
                        },
                        _ => null
                    };
                }
            }

            return ExecuteAsync(
                ct => _httpClient.GetAsync($"search/results{query}", ct),
                handleSearchResultsResponse,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> DeleteSearchAsync(int id, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("id", id)
                .ToFormUrlEncodedContent();

            static Task<ApiResult> handleDeleteSearchResponse(HttpResponseMessage response, string operation, CancellationToken currentCancellationToken)
            {
                return CreateResultAsync(response, operation, currentCancellationToken, createDeleteSearchFailure);

                ApiFailure? createDeleteSearchFailure(HttpStatusCode statusCode, string? responseBody)
                {
                    return statusCode == HttpStatusCode.NotFound
                        ? new ApiFailure
                        {
                            Kind = ApiFailureKind.NotFound,
                            Operation = operation,
                            StatusCode = statusCode,
                            UserMessage = responseBody ?? "The search job could not be found.",
                            Detail = responseBody,
                            Reason = SearchFailureReason.SearchMissing,
                            ResponseBody = responseBody,
                        }
                        : null;
                }
            }

            return ExecuteAsync(
                ct => _httpClient.PostAsync("search/delete", content, ct),
                handleDeleteSearchResponse,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyList<SearchPlugin>>> GetSearchPluginsAsync(CancellationToken cancellationToken = default)
        {
            Task<ApiResult<IReadOnlyList<SearchPlugin>>> handleGetSearchPluginsResponse(HttpResponseMessage response, string operation, CancellationToken currentCancellationToken)
            {
                return CreateResultAsync(operation, response, readSearchPlugins, currentCancellationToken, createSearchPluginsFailure);

                Task<IReadOnlyList<SearchPlugin>> readSearchPlugins(HttpContent content, CancellationToken readCancellationToken)
                {
                    return GetJsonListAsync<SearchPlugin>(content, readCancellationToken);
                }

                ApiFailure? createSearchPluginsFailure(HttpStatusCode statusCode, string? responseBody)
                {
                    return statusCode == HttpStatusCode.Forbidden && responseBody is not null
                        ? new ApiFailure
                        {
                            Kind = ApiFailureKind.AccessDenied,
                            Operation = operation,
                            StatusCode = statusCode,
                            UserMessage = responseBody,
                            Detail = responseBody,
                            Reason = SearchFailureReason.SearchUnavailable,
                            ResponseBody = responseBody,
                        }
                        : null;
                }
            }

            return ExecuteAsync(
                ct => _httpClient.GetAsync("search/plugins", ct),
                handleGetSearchPluginsResponse,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> InstallSearchPluginsAsync(IEnumerable<string> sources, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddPipeSeparated("sources", sources)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("search/installPlugin", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> UninstallSearchPluginsAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddPipeSeparated("names", names)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("search/uninstallPlugin", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> EnableSearchPluginsAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
               .AddPipeSeparated("names", names)
               .Add("enable", true)
               .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("search/enablePlugin", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> DisableSearchPluginsAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddPipeSeparated("names", names)
                .Add("enable", false)
               .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("search/enablePlugin", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> DownloadSearchResultAsync(string pluginName, string torrentUrl, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("pluginName", pluginName)
                .Add("torrentUrl", torrentUrl)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("search/downloadTorrent", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> UpdateSearchPluginsAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(ct => _httpClient.PostAsync("search/updatePlugins", null, ct), cancellationToken: cancellationToken);
        }
    }
}
