using QBittorrent.ApiClient.Models;
using System.Net;

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

            async Task<ApiResult<SearchStatus?>> HandleSearchStatusResponse(HttpResponseMessage response, string operation, CancellationToken currentCancellationToken)
            {
                using (response)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return ApiResult<SearchStatus?>.Success(null);
                    }

                    return await CreateResultAsync(operation, response, ReadSearchStatus, currentCancellationToken);
                }

                async Task<SearchStatus?> ReadSearchStatus(HttpContent content, CancellationToken readCancellationToken)
                {
                    return (await GetJsonListAsync<SearchStatus>(content, readCancellationToken)).FirstOrDefault();
                }
            }

            return ExecuteAsync(
                ct => _httpClient.GetAsync($"search/status{query}", ct),
                HandleSearchStatusResponse,
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

            Task<ApiResult<SearchResults>> HandleSearchResultsResponse(HttpResponseMessage response, string operation, CancellationToken currentCancellationToken)
            {
                return CreateResultAsync(operation, response, ReadSearchResults, currentCancellationToken, CreateSearchResultsFailure);

                Task<SearchResults> ReadSearchResults(HttpContent content, CancellationToken readCancellationToken)
                {
                    return GetJsonAsync<SearchResults>(content, readCancellationToken);
                }

                ApiFailure? CreateSearchResultsFailure(HttpStatusCode statusCode, string? responseBody)
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
                HandleSearchResultsResponse,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> DeleteSearchAsync(int id, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("id", id)
                .ToFormUrlEncodedContent();

            Task<ApiResult> HandleDeleteSearchResponse(HttpResponseMessage response, string operation, CancellationToken currentCancellationToken)
            {
                return CreateResultAsync(response, operation, currentCancellationToken, CreateDeleteSearchFailure);

                ApiFailure? CreateDeleteSearchFailure(HttpStatusCode statusCode, string? responseBody)
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
                HandleDeleteSearchResponse,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyList<SearchPlugin>>> GetSearchPluginsAsync(CancellationToken cancellationToken = default)
        {
            Task<ApiResult<IReadOnlyList<SearchPlugin>>> HandleGetSearchPluginsResponse(HttpResponseMessage response, string operation, CancellationToken currentCancellationToken)
            {
                return CreateResultAsync(operation, response, ReadSearchPlugins, currentCancellationToken, CreateSearchPluginsFailure);

                Task<IReadOnlyList<SearchPlugin>> ReadSearchPlugins(HttpContent content, CancellationToken readCancellationToken)
                {
                    return GetJsonListAsync<SearchPlugin>(content, readCancellationToken);
                }

                ApiFailure? CreateSearchPluginsFailure(HttpStatusCode statusCode, string? responseBody)
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
                HandleGetSearchPluginsResponse,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> InstallSearchPluginsAsync(CancellationToken cancellationToken = default, params string[] sources)
        {
            var content = new FormUrlEncodedBuilder()
                .AddPipeSeparated("sources", sources)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("search/installPlugin", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> UninstallSearchPluginsAsync(CancellationToken cancellationToken = default, params string[] names)
        {
            var content = new FormUrlEncodedBuilder()
                .AddPipeSeparated("names", names)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("search/uninstallPlugin", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> EnableSearchPluginsAsync(CancellationToken cancellationToken = default, params string[] names)
        {
            var content = new FormUrlEncodedBuilder()
               .AddPipeSeparated("names", names)
               .Add("enable", true)
               .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("search/enablePlugin", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> DisableSearchPluginsAsync(CancellationToken cancellationToken = default, params string[] names)
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
