using System.Globalization;
using System.Net;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient
{
    internal partial class ApiClient
    {
        public async Task<ApiResult<IReadOnlyList<Torrent>>> GetTorrentListAsync(
            string? filter = null,
            string? category = null,
            string? tag = null,
            string? sort = null,
            bool? reverse = null,
            int? limit = null,
            int? offset = null,
            bool? isPrivate = null,
            bool? includeFiles = null,
            bool? includeTrackers = null,
            TorrentSelector? selector = null,
            CancellationToken cancellationToken = default)
        {
            var query = new QueryBuilder();
            if (filter is not null)
            {
                query.Add("filter", filter);
            }
            if (category is not null)
            {
                query.Add("category", category);
            }
            if (tag is not null)
            {
                query.Add("tag", tag);
            }
            if (sort is not null)
            {
                query.Add("sort", sort);
            }
            if (reverse is not null)
            {
                query.Add("reverse", reverse.Value);
            }
            if (limit is not null)
            {
                query.Add("limit", limit.Value);
            }
            if (offset is not null)
            {
                query.Add("offset", offset.Value);
            }
            if (selector is not null && !selector.All)
            {
                query.Add("hashes", string.Join('|', selector.Hashes));
            }
            if (isPrivate is not null)
            {
                query.Add("private", isPrivate.Value ? "true" : "false");
            }

            if (includeFiles is not null)
            {
                var profile = CompatibilityProfile;
                if (!profile.SupportsTorrentListIncludeFiles)
                {
                    if (includeFiles.Value)
                    {
                        return CreateUnsupportedCompatibilityFailure(
                            nameof(GetTorrentListAsync),
                            profile,
                            $"qBittorrent Web API {profile.WebApiVersion} does not support including torrent file data in torrent list responses.").ToResult<IReadOnlyList<Torrent>>();
                    }
                }
                else
                {
                    query.Add("includeFiles", includeFiles.Value ? "true" : "false");
                }
            }
            if (includeTrackers is not null)
            {
                query.Add("includeTrackers", includeTrackers.Value ? "true" : "false");
            }

            return await ExecuteAsync(
                ct => _httpClient.GetAsync("torrents/info", query, ct),
                GetJsonListAsync<Torrent>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<int>> GetTorrentCountAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync("torrents/count", ct),
                ReadInt32Async,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<TorrentProperties>> GetTorrentPropertiesAsync(string hash, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync($"torrents/properties?hash={hash}", ct),
                GetJsonAsync<TorrentProperties>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyList<TorrentTracker>>> GetTorrentTrackersAsync(string hash, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync($"torrents/trackers?hash={hash}", ct),
                GetJsonListAsync<TorrentTracker>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyList<WebSeed>>> GetTorrentWebSeedsAsync(string hash, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync($"torrents/webseeds?hash={hash}", ct),
                GetJsonListAsync<WebSeed>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> AddTorrentWebSeedsAsync(string hash, IEnumerable<string> urls, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("hash", hash)
                .Add("urls", string.Join('|', urls))
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/addWebSeeds", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> EditTorrentWebSeedAsync(string hash, string originalUrl, string newUrl, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("hash", hash)
                .Add("origUrl", originalUrl)
                .Add("newUrl", newUrl)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/editWebSeed", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> RemoveTorrentWebSeedsAsync(string hash, IEnumerable<string> urls, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("hash", hash)
                .Add("urls", string.Join('|', urls))
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/removeWebSeeds", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyList<FileData>>> GetTorrentContentsAsync(string hash, IEnumerable<int>? indexes = null, CancellationToken cancellationToken = default)
        {
            var normalizedIndexes = indexes?.ToArray() ?? [];
            var query = new QueryBuilder();
            query.Add("hash", hash);
            if (normalizedIndexes.Length > 0)
            {
                query.Add("indexes", string.Join('|', normalizedIndexes));
            }

            return ExecuteAsync(
                ct => _httpClient.GetAsync("torrents/files", query, ct),
                GetJsonListAsync<FileData>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyList<PieceState>>> GetTorrentPieceStatesAsync(string hash, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync($"torrents/pieceStates?hash={hash}", ct),
                GetJsonListAsync<PieceState>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyList<string>>> GetTorrentPieceHashesAsync(string hash, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync($"torrents/pieceHashes?hash={hash}", ct),
                GetJsonListAsync<string>,
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult<IReadOnlyList<int>>> GetTorrentPieceAvailabilityAsync(string hash, CancellationToken cancellationToken = default)
        {
            var profile = CompatibilityProfile;
            if (!profile.SupportsTorrentPieceAvailability)
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(GetTorrentPieceAvailabilityAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support torrent piece availability.").ToResult<IReadOnlyList<int>>();
            }

            return await ExecuteAsync(
                ct => _httpClient.GetAsync($"torrents/pieceAvailability?hash={hash}", ct),
                GetJsonListAsync<int>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> StopTorrentsAsync(TorrentSelector selector, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/stop", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> StartTorrentsAsync(TorrentSelector selector, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/start", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> DeleteTorrentsAsync(TorrentSelector selector, bool deleteFiles = false, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .Add("deleteFiles", deleteFiles)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/delete", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> RecheckTorrentsAsync(TorrentSelector selector, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/recheck", content, ct),
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult> ReannounceTorrentsAsync(TorrentSelector selector, IEnumerable<string>? urls = null, CancellationToken cancellationToken = default)
        {
            var normalizedUrls = urls?
                .Where(url => !string.IsNullOrWhiteSpace(url))
                .Select(url => url.Trim())
                .Distinct(StringComparer.Ordinal)
                .ToArray() ?? [];

            if (normalizedUrls.Length > 0)
            {
                var profile = CompatibilityProfile;
                if (!profile.SupportsReannounceUrls)
                {
                    return CreateUnsupportedCompatibilityFailure(
                        nameof(ReannounceTorrentsAsync),
                        profile,
                        $"qBittorrent Web API {profile.WebApiVersion} does not support tracker-targeted reannounce URLs.").ToResult();
                }
            }

            var contentBuilder = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector);

            if (normalizedUrls.Length > 0)
            {
                contentBuilder.AddPipeSeparated("urls", normalizedUrls);
            }

            return await ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/reannounce", contentBuilder.ToFormUrlEncodedContent(), ct),
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult<AddTorrentResult>> AddTorrentAsync(AddTorrentParams addTorrentParams, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(addTorrentParams);

            if ((addTorrentParams.Downloader is not null) || (addTorrentParams.FilePriorities is not null))
            {
                var profile = CompatibilityProfile;
                if ((addTorrentParams.Downloader is not null) && !profile.SupportsTorrentAddDownloader)
                {
                    return CreateUnsupportedCompatibilityFailure(
                        nameof(AddTorrentAsync),
                        profile,
                        $"qBittorrent Web API {profile.WebApiVersion} does not support add-torrent downloader selection.").ToResult<AddTorrentResult>();
                }

                if ((addTorrentParams.FilePriorities is not null) && !profile.SupportsTorrentAddFilePriorities)
                {
                    return CreateUnsupportedCompatibilityFailure(
                        nameof(AddTorrentAsync),
                        profile,
                        $"qBittorrent Web API {profile.WebApiVersion} does not support add-torrent file priorities.").ToResult<AddTorrentResult>();
                }
            }

            using var content = new MultipartFormDataContent();

            if (addTorrentParams.Urls?.Any() == true)
            {
                content.AddString("urls", string.Join('\n', addTorrentParams.Urls));
            }

            if (addTorrentParams.Torrents is not null)
            {
                foreach (var (name, stream) in addTorrentParams.Torrents)
                {
                    content.Add(await CreateOwnedTorrentContentAsync(stream, cancellationToken), "torrents", name);
                }
            }

            if (addTorrentParams.SkipChecking is not null)
            {
                content.AddString("skip_checking", addTorrentParams.SkipChecking.Value);
            }
            if (addTorrentParams.SequentialDownload is not null)
            {
                content.AddString("sequentialDownload", addTorrentParams.SequentialDownload.Value);
            }
            if (addTorrentParams.FirstLastPiecePriority is not null)
            {
                content.AddString("firstLastPiecePrio", addTorrentParams.FirstLastPiecePriority.Value);
            }
            if (addTorrentParams.AddToTopOfQueue is not null)
            {
                content.AddString("addToTopOfQueue", addTorrentParams.AddToTopOfQueue.Value);
            }
            if (addTorrentParams.Forced is not null)
            {
                content.AddString("forced", addTorrentParams.Forced.Value);
            }
            if (addTorrentParams.Stopped is not null)
            {
                content.AddString("stopped", addTorrentParams.Stopped.Value);
            }
            if (addTorrentParams.SavePath is not null)
            {
                content.AddString("savepath", addTorrentParams.SavePath);
            }
            if (addTorrentParams.DownloadPath is not null)
            {
                content.AddString("downloadPath", addTorrentParams.DownloadPath);
            }
            if (addTorrentParams.UseDownloadPath is not null)
            {
                content.AddString("useDownloadPath", addTorrentParams.UseDownloadPath.Value);
            }
            if (addTorrentParams.Category is not null)
            {
                content.AddString("category", addTorrentParams.Category);
            }
            if (addTorrentParams.Tags is not null)
            {
                content.AddString("tags", string.Join(',', addTorrentParams.Tags));
            }
            if (addTorrentParams.RenameTorrent is not null)
            {
                content.AddString("rename", addTorrentParams.RenameTorrent);
            }
            if (addTorrentParams.UploadLimit is not null)
            {
                content.AddString("upLimit", addTorrentParams.UploadLimit.Value);
            }
            if (addTorrentParams.DownloadLimit is not null)
            {
                content.AddString("dlLimit", addTorrentParams.DownloadLimit.Value);
            }
            if (addTorrentParams.RatioLimit is not null)
            {
                content.AddString("ratioLimit", addTorrentParams.RatioLimit.Value);
            }
            if (addTorrentParams.SeedingTimeLimit is not null)
            {
                content.AddString("seedingTimeLimit", addTorrentParams.SeedingTimeLimit.Value);
            }
            if (addTorrentParams.InactiveSeedingTimeLimit is not null)
            {
                content.AddString("inactiveSeedingTimeLimit", addTorrentParams.InactiveSeedingTimeLimit.Value);
            }
            if (addTorrentParams.ShareLimitAction is not null)
            {
                content.AddString("shareLimitAction", addTorrentParams.ShareLimitAction.Value);
            }
            if (addTorrentParams.AutoTorrentManagement is not null)
            {
                content.AddString("autoTMM", addTorrentParams.AutoTorrentManagement.Value);
            }
            if (addTorrentParams.StopCondition is not null)
            {
                content.AddString("stopCondition", addTorrentParams.StopCondition.Value);
            }
            if (addTorrentParams.ContentLayout is not null)
            {
                content.AddString("contentLayout", addTorrentParams.ContentLayout.Value);
            }
            if (addTorrentParams.Downloader is not null)
            {
                content.AddString("downloader", addTorrentParams.Downloader);
            }
            if (addTorrentParams.FilePriorities is not null)
            {
                var priorities = string.Join(',', addTorrentParams.FilePriorities.Select(priority => ((int)priority).ToString(CultureInfo.InvariantCulture)));
                content.AddString("filePriorities", priorities);
            }
            if (!string.IsNullOrWhiteSpace(addTorrentParams.SslCertificate))
            {
                content.AddString("ssl_certificate", addTorrentParams.SslCertificate);
            }
            if (!string.IsNullOrWhiteSpace(addTorrentParams.SslPrivateKey))
            {
                content.AddString("ssl_private_key", addTorrentParams.SslPrivateKey);
            }
            if (!string.IsNullOrWhiteSpace(addTorrentParams.SslDhParams))
            {
                content.AddString("ssl_dh_params", addTorrentParams.SslDhParams);
            }
            if (!string.IsNullOrWhiteSpace(addTorrentParams.Cookie))
            {
                content.AddString("cookie", addTorrentParams.Cookie);
            }

            var expectedTorrentCount = (addTorrentParams.Torrents?.Count ?? 0) + (addTorrentParams.Urls?.Count() ?? 0);

            async Task<ApiResult<AddTorrentResult>> handleAddTorrentResponse(HttpResponseMessage response, string operation, CancellationToken currentCancellationToken)
            {
                async Task<AddTorrentResult> readAddTorrentResult(HttpContent httpContent, CancellationToken readCancellationToken)
                {
                    var payload = await httpContent.ReadAsStringAsync(readCancellationToken);

                    switch (payload)
                    {
                        case "Fails.":
                            return new AddTorrentResult(0, 1);

                        case "Ok.":
                            return new AddTorrentResult(1, 0);

                        case null:
                        case "":
                            return new AddTorrentResult(0, 0);
                    }

                    var result = DeserializeJson<AddTorrentResult>(payload);
                    if (result is null)
                    {
                        return new AddTorrentResult(0, expectedTorrentCount);
                    }

                    return result;
                }

                ApiFailure? createAddTorrentFailure(HttpStatusCode statusCode, string? responseBody)
                {
                    return statusCode switch
                    {
                        HttpStatusCode.BadRequest => new ApiFailure
                        {
                            Kind = ApiFailureKind.ValidationFailed,
                            Operation = operation,
                            StatusCode = statusCode,
                            UserMessage = responseBody ?? "The torrent request was rejected.",
                            Detail = responseBody,
                            Reason = AddTorrentFailureReason.ValidationFailed,
                            ResponseBody = responseBody,
                        },
                        HttpStatusCode.Conflict => new ApiFailure
                        {
                            Kind = ApiFailureKind.Conflict,
                            Operation = operation,
                            StatusCode = statusCode,
                            UserMessage = responseBody ?? "All torrents failed to add.",
                            Detail = responseBody,
                            Reason = AddTorrentFailureReason.AllTorrentsFailed,
                            ResponseBody = responseBody,
                        },
                        HttpStatusCode.UnsupportedMediaType => new ApiFailure
                        {
                            Kind = ApiFailureKind.UnsupportedData,
                            Operation = operation,
                            StatusCode = statusCode,
                            UserMessage = responseBody ?? "The supplied torrent data is not supported.",
                            Detail = responseBody,
                            Reason = AddTorrentFailureReason.InvalidTorrentData,
                            ResponseBody = responseBody,
                        },
                        _ => null
                    };
                }

                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    return await CreatePendingResultAsync(operation, response, readAddTorrentResult, currentCancellationToken);
                }

                return await CreateResultAsync(operation, response, readAddTorrentResult, currentCancellationToken, createAddTorrentFailure);
            }

            return await ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/add", content, ct),
                handleAddTorrentResponse,
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult> AddTrackersToTorrentAsync(TorrentSelector selector, IEnumerable<string> urls, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var applyToAll = selector.All;
            var normalizedHashes = selector.Hashes ?? [];

            var profile = CompatibilityProfile;
            if (!profile.SupportsTrackerBatchOperations)
            {
                if (applyToAll)
                {
                    return CreateUnsupportedCompatibilityFailure(
                        nameof(AddTrackersToTorrentAsync),
                        profile,
                        $"qBittorrent Web API {profile.WebApiVersion} does not support adding trackers to all torrents in a single request.").ToResult();
                }

                if (normalizedHashes.Count > 1)
                {
                    return CreateUnsupportedCompatibilityFailure(
                        nameof(AddTrackersToTorrentAsync),
                        profile,
                        $"qBittorrent Web API {profile.WebApiVersion} does not support adding trackers to multiple torrents in a single request.").ToResult();
                }
            }

            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hash", selector, profile.TrackerAllValue)
                .Add("urls", string.Join('\n', urls))
                .ToFormUrlEncodedContent();

            return await ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/addTrackers", content, ct),
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult> EditTrackerAsync(string hash, string url, string? newUrl = null, int? tier = null, CancellationToken cancellationToken = default)
        {
            var normalizedNewUrl = string.IsNullOrWhiteSpace(newUrl)
                ? null
                : newUrl;

            if ((normalizedNewUrl is null) && (tier is null))
            {
                throw new ArgumentException("Must specify at least one of newUrl or tier.");
            }

            var profile = CompatibilityProfile;
            if (!profile.SupportsTrackerTierEditing && (tier is not null))
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(EditTrackerAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support editing tracker tiers.").ToResult();
            }

            var content = new FormUrlEncodedBuilder()
                .Add("hash", hash);

            if (profile.SupportsTrackerTierEditing)
            {
                content.Add("url", url);

                if (normalizedNewUrl is not null)
                {
                    content.Add("newUrl", normalizedNewUrl);
                }

                if (tier is not null)
                {
                    content.Add("tier", tier.Value);
                }
            }
            else
            {
                content.Add("origUrl", url);

                if (normalizedNewUrl is not null)
                {
                    content.Add("newUrl", normalizedNewUrl);
                }
            }

            var form = content.ToFormUrlEncodedContent();

            return await ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/editTracker", form, ct),
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult> RemoveTrackersAsync(TorrentSelector selector, IEnumerable<string> urls, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var applyToAll = selector.All;
            var normalizedHashes = selector.Hashes ?? [];

            var profile = CompatibilityProfile;
            if (!profile.SupportsTrackerBatchOperations && (normalizedHashes.Count > 1))
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(RemoveTrackersAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support removing trackers from multiple torrents in a single request.").ToResult();
            }

            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hash", selector, profile.TrackerAllValue)
                .AddPipeSeparated("urls", urls)
                .ToFormUrlEncodedContent();

            return await ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/removeTrackers", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> AddPeersAsync(TorrentSelector selector, IEnumerable<PeerId> peers, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .AddPipeSeparated("peers", peers)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/addPeers", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> IncreaseTorrentPriorityAsync(TorrentSelector selector, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/increasePrio", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> DecreaseTorrentPriorityAsync(TorrentSelector selector, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/decreasePrio", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> MaxTorrentPriorityAsync(TorrentSelector selector, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/topPrio", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> MinTorrentPriorityAsync(TorrentSelector selector, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/bottomPrio", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetFilePriorityAsync(string hash, IEnumerable<int> id, Priority priority, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("hash", hash)
                .AddPipeSeparated("id", id)
                .Add("priority", priority)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/filePrio", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyDictionary<string, long>>> GetTorrentDownloadLimitAsync(TorrentSelector selector, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/downloadLimit", content, ct),
                GetJsonDictionaryAsync<string, long>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetTorrentDownloadLimitAsync(TorrentSelector selector, long limit, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .Add("limit", limit)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setDownloadLimit", content, ct),
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult> SetTorrentShareLimitAsync(
            TorrentSelector selector,
            float ratioLimit,
            int seedingTimeLimit,
            int inactiveSeedingTimeLimit,
            ShareLimitAction? shareLimitAction = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var profile = CompatibilityProfile;
            if (profile.RequiresTorrentShareLimitAction)
            {
                if (shareLimitAction is null)
                {
                    return CreateUnsupportedCompatibilityFailure(
                        nameof(SetTorrentShareLimitAsync),
                        profile,
                        $"qBittorrent Web API {profile.WebApiVersion} requires shareLimitAction when setting share limits.").ToResult();
                }
            }
            else if (shareLimitAction is not null)
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(SetTorrentShareLimitAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support shareLimitAction when setting share limits.").ToResult();
            }

            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .Add("ratioLimit", ratioLimit)
                .Add("seedingTimeLimit", seedingTimeLimit)
                .Add("inactiveSeedingTimeLimit", inactiveSeedingTimeLimit);

            if (shareLimitAction is not null)
            {
                content.Add("shareLimitAction", shareLimitAction.Value.ToString());
            }

            var form = content.ToFormUrlEncodedContent();

            return await ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setShareLimits", form, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyDictionary<string, long>>> GetTorrentUploadLimitAsync(TorrentSelector selector, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/uploadLimit", content, ct),
                GetJsonDictionaryAsync<string, long>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetTorrentUploadLimitAsync(TorrentSelector selector, long limit, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .Add("limit", limit)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setUploadLimit", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetTorrentLocationAsync(TorrentSelector selector, string location, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .Add("location", location)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setLocation", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetTorrentSavePathAsync(TorrentSelector selector, string path, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("id", selector)
                .Add("path", path)
                .ToFormUrlEncodedContent();

            static Task<ApiResult> handleSetTorrentSavePathResponse(HttpResponseMessage response, string operation, CancellationToken currentCancellationToken)
            {
                ApiFailure? createSetTorrentPathFailure(HttpStatusCode statusCode, string? responseBody)
                {
                    if (statusCode == HttpStatusCode.Forbidden
                        && responseBody is not null
                        && responseBody.Contains("write", StringComparison.OrdinalIgnoreCase))
                    {
                        return new ApiFailure
                        {
                            Kind = ApiFailureKind.AccessDenied,
                            Operation = operation,
                            StatusCode = statusCode,
                            UserMessage = responseBody,
                            Detail = responseBody,
                            Reason = TorrentPathFailureReason.DirectoryNotWritable,
                            ResponseBody = responseBody,
                        };
                    }

                    if (statusCode == HttpStatusCode.Conflict)
                    {
                        return new ApiFailure
                        {
                            Kind = ApiFailureKind.Conflict,
                            Operation = operation,
                            StatusCode = statusCode,
                            UserMessage = responseBody ?? "The target directory could not be created.",
                            Detail = responseBody,
                            Reason = TorrentPathFailureReason.DirectoryCreationFailed,
                            ResponseBody = responseBody,
                        };
                    }

                    return null;
                }

                return CreateResultAsync(response, operation, currentCancellationToken, createSetTorrentPathFailure);
            }

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setSavePath", content, ct),
                handleSetTorrentSavePathResponse,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetTorrentDownloadPathAsync(TorrentSelector selector, string? path, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("id", selector)
                .Add("path", path ?? string.Empty)
                .ToFormUrlEncodedContent();

            static Task<ApiResult> handleSetTorrentDownloadPathResponse(HttpResponseMessage response, string operation, CancellationToken currentCancellationToken)
            {
                ApiFailure? createSetTorrentPathFailure(HttpStatusCode statusCode, string? responseBody)
                {
                    if (statusCode == HttpStatusCode.Forbidden
                        && responseBody is not null
                        && responseBody.Contains("write", StringComparison.OrdinalIgnoreCase))
                    {
                        return new ApiFailure
                        {
                            Kind = ApiFailureKind.AccessDenied,
                            Operation = operation,
                            StatusCode = statusCode,
                            UserMessage = responseBody,
                            Detail = responseBody,
                            Reason = TorrentPathFailureReason.DirectoryNotWritable,
                            ResponseBody = responseBody,
                        };
                    }

                    if (statusCode == HttpStatusCode.Conflict)
                    {
                        return new ApiFailure
                        {
                            Kind = ApiFailureKind.Conflict,
                            Operation = operation,
                            StatusCode = statusCode,
                            UserMessage = responseBody ?? "The target directory could not be created.",
                            Detail = responseBody,
                            Reason = TorrentPathFailureReason.DirectoryCreationFailed,
                            ResponseBody = responseBody,
                        };
                    }

                    return null;
                }

                return CreateResultAsync(response, operation, currentCancellationToken, createSetTorrentPathFailure);
            }

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setDownloadPath", content, ct),
                handleSetTorrentDownloadPathResponse,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetTorrentNameAsync(string hash, string name, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("hash", hash)
                .Add("name", name)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/rename", content, ct),
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult> SetTorrentCommentAsync(TorrentSelector selector, string comment, CancellationToken cancellationToken = default)
        {
            var profile = CompatibilityProfile;
            if (!profile.SupportsTorrentCommentEditing)
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(SetTorrentCommentAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support torrent comments.").ToResult();
            }

            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .Add("comment", comment)
                .ToFormUrlEncodedContent();

            return await ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setComment", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetTorrentCategoryAsync(TorrentSelector selector, string category, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .Add("category", category)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setCategory", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyDictionary<string, Category>>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync("torrents/categories", ct),
                GetJsonDictionaryAsync<string, Category>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> AddCategoryAsync(string category, string savePath, DownloadPathOption? downloadPathOption = null, CancellationToken cancellationToken = default)
        {
            var builder = new FormUrlEncodedBuilder()
                .Add("category", category)
                .Add("savePath", savePath);

            if (downloadPathOption is not null)
            {
                builder.Add("downloadPathEnabled", downloadPathOption.Enabled);
                if (!string.IsNullOrWhiteSpace(downloadPathOption.Path))
                {
                    builder.Add("downloadPath", downloadPathOption.Path);
                }
            }

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/createCategory", builder.ToFormUrlEncodedContent(), ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> EditCategoryAsync(string category, string savePath, DownloadPathOption? downloadPathOption = null, CancellationToken cancellationToken = default)
        {
            var builder = new FormUrlEncodedBuilder()
                .Add("category", category)
                .Add("savePath", savePath);

            if (downloadPathOption is not null)
            {
                builder.Add("downloadPathEnabled", downloadPathOption.Enabled);
                if (!string.IsNullOrWhiteSpace(downloadPathOption.Path))
                {
                    builder.Add("downloadPath", downloadPathOption.Path);
                }
            }

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/editCategory", builder.ToFormUrlEncodedContent(), ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> RemoveCategoriesAsync(IEnumerable<string> categories, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("categories", string.Join('\n', categories))
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/removeCategories", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> AddTorrentTagsAsync(TorrentSelector selector, IEnumerable<string> tags, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .AddCommaSeparated("tags", tags)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/addTags", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetTorrentTagsAsync(TorrentSelector selector, IEnumerable<string> tags, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .AddCommaSeparated("tags", tags)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setTags", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> RemoveTorrentTagsAsync(TorrentSelector selector, IEnumerable<string> tags, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .AddCommaSeparated("tags", tags)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/removeTags", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyList<string>>> GetAllTagsAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync("torrents/tags", ct),
                GetJsonListAsync<string>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> CreateTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddCommaSeparated("tags", tags)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/createTags", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> DeleteTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddCommaSeparated("tags", tags)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/deleteTags", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetAutomaticTorrentManagementAsync(TorrentSelector selector, bool enable, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .Add("enable", enable)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setAutoManagement", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> ToggleSequentialDownloadAsync(TorrentSelector selector, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/toggleSequentialDownload", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetFirstLastPiecePriorityAsync(TorrentSelector selector, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/toggleFirstLastPiecePrio", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetForceStartAsync(TorrentSelector selector, bool value, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .Add("value", value)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setForceStart", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetSuperSeedingAsync(TorrentSelector selector, bool value, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddTorrentSelector("hashes", selector)
                .Add("value", value)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setSuperSeeding", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> RenameFileAsync(string hash, string oldPath, string newPath, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("hash", hash)
                .Add("oldPath", oldPath)
                .Add("newPath", newPath)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/renameFile", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> RenameFolderAsync(string hash, string oldPath, string newPath, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("hash", hash)
                .Add("oldPath", oldPath)
                .Add("newPath", newPath)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/renameFolder", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<string>> GetExportUrlAsync(string hash)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(hash);

            var baseAddress = _httpClient.BaseAddress;
            if (baseAddress is null)
            {
                return Task.FromResult(CreateConfigurationFailure(nameof(GetExportUrlAsync), "HttpClient BaseAddress must be configured.").ToResult<string>());
            }

            var uriBuilder = new UriBuilder(baseAddress)
            {
                Path = $"{baseAddress.AbsolutePath.TrimEnd('/')}/torrents/export",
                Query = $"hash={Uri.EscapeDataString(hash)}"
            };

            return Task.FromResult(ApiResult.CreateSuccess(uriBuilder.Uri.AbsoluteUri));
        }

        public Task<ApiResult<byte[]>> ExportTorrentAsync(string hash, CancellationToken cancellationToken = default)
        {
            var query = new QueryBuilder()
                .Add("hash", hash);

            return ExecuteAsync(
                ct => _httpClient.GetAsync("torrents/export", query, ct),
                (content, ct) => content.ReadAsByteArrayAsync(ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<SslParameters>> GetTorrentSslParametersAsync(string hash, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync($"torrents/SSLParameters?hash={hash}", ct),
                GetJsonAsync<SslParameters>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetTorrentSslParametersAsync(string hash, SslParameters parameters, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(parameters);
            if (string.IsNullOrWhiteSpace(parameters.Certificate) || string.IsNullOrWhiteSpace(parameters.PrivateKey) || string.IsNullOrWhiteSpace(parameters.DhParams))
            {
                throw new ArgumentException("Certificate, private key, and DH params are required.", nameof(parameters));
            }

            var content = new FormUrlEncodedBuilder()
                .Add("hash", hash)
                .Add("ssl_certificate", parameters.Certificate)
                .Add("ssl_private_key", parameters.PrivateKey)
                .Add("ssl_dh_params", parameters.DhParams)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setSSLParameters", content, ct),
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult<TorrentMetadata, FetchTorrentMetadataPendingResult>> FetchTorrentMetadataAsync(string source, string? downloader = null, CancellationToken cancellationToken = default)
        {
            var profile = CompatibilityProfile;
            if (!profile.SupportsTorrentMetadata)
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(FetchTorrentMetadataAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support torrent metadata APIs.").ToResult<TorrentMetadata, FetchTorrentMetadataPendingResult>();
            }

            var content = new FormUrlEncodedBuilder()
                .Add("source", source);

            if (!string.IsNullOrWhiteSpace(downloader))
            {
                content.Add("downloader", downloader);
            }

            static async Task<ApiResult<TorrentMetadata, FetchTorrentMetadataPendingResult>> handleFetchMetadataResponse(HttpResponseMessage response, string operation, CancellationToken currentCancellationToken)
            {
                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    return await CreatePendingResultAsync<TorrentMetadata, FetchTorrentMetadataPendingResult>(
                        operation,
                        response,
                        GetJsonAsync<FetchTorrentMetadataPendingResult>,
                        currentCancellationToken);
                }

                var result = await CreateResultAsync(operation, response, GetJsonAsync<TorrentMetadata>, currentCancellationToken);
                if (result.IsFailure)
                {
                    return result.Failure.ToResult<TorrentMetadata, FetchTorrentMetadataPendingResult>();
                }

                var torrentMetadata = result.Value!;

                return ApiResult.CreateSuccess<TorrentMetadata, FetchTorrentMetadataPendingResult>(torrentMetadata);
            }

            return await ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/fetchMetadata", content.ToFormUrlEncodedContent(), ct),
                handleFetchMetadataResponse,
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult<IReadOnlyList<TorrentMetadata>>> ParseTorrentMetadataAsync(IReadOnlyDictionary<string, Stream> torrents, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(torrents);
            var torrentNames = torrents.Keys.ToList();

            var profile = CompatibilityProfile;
            if (!profile.SupportsTorrentMetadata)
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(ParseTorrentMetadataAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support torrent metadata APIs.").ToResult<IReadOnlyList<TorrentMetadata>>();
            }

            using var content = new MultipartFormDataContent();
            foreach (var (name, stream) in torrents)
            {
                content.Add(await CreateOwnedTorrentContentAsync(stream, cancellationToken), "torrents", name);
            }

            static async Task<IReadOnlyList<TorrentMetadata>> readParsedTorrentMetadata(HttpContent responseContent, ApiClientCompatibilityProfile compatibilityProfile, IReadOnlyList<string> requestedTorrentNames, CancellationToken readCancellationToken)
            {
                if (compatibilityProfile.SupportsTorrentMetadataArrayResponse)
                {
                    return (await GetJsonAsync<List<TorrentMetadata>>(responseContent, readCancellationToken)).AsReadOnly();
                }

                var metadataByTorrentName = await GetJsonAsync<Dictionary<string, TorrentMetadata>>(responseContent, readCancellationToken);
                if (metadataByTorrentName.Count != requestedTorrentNames.Count)
                {
                    throw new ResponseDeserializationException("IReadOnlyList<TorrentMetadata>");
                }

                var orderedMetadata = new List<TorrentMetadata>(requestedTorrentNames.Count);
                foreach (var requestedTorrentName in requestedTorrentNames)
                {
                    if (!metadataByTorrentName.TryGetValue(requestedTorrentName, out var torrentMetadata))
                    {
                        throw new ResponseDeserializationException("IReadOnlyList<TorrentMetadata>");
                    }

                    orderedMetadata.Add(torrentMetadata);
                }

                return orderedMetadata.AsReadOnly();
            }

            return await ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/parseMetadata", content, ct),
                (responseContent, ct) => readParsedTorrentMetadata(responseContent, profile, torrentNames, ct),
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult<byte[]>> SaveTorrentMetadataAsync(string source, CancellationToken cancellationToken = default)
        {
            var profile = CompatibilityProfile;
            if (!profile.SupportsTorrentMetadata)
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(SaveTorrentMetadataAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support torrent metadata APIs.").ToResult<byte[]>();
            }

            var content = new FormUrlEncodedBuilder()
                .Add("source", source)
                .ToFormUrlEncodedContent();

            return await ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/saveMetadata", content, ct),
                (httpContent, ct) => httpContent.ReadAsByteArrayAsync(ct),
                cancellationToken: cancellationToken);
        }

        private static async Task<StreamContent> CreateOwnedTorrentContentAsync(Stream stream, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(stream);

            if (!stream.CanRead)
            {
                throw new ArgumentException("Torrent stream must be readable.", nameof(stream));
            }

            var originalPosition = 0L;
            var shouldRestorePosition = stream.CanSeek;
            if (shouldRestorePosition)
            {
                originalPosition = stream.Position;
            }

            try
            {
                var copy = new MemoryStream();
                await stream.CopyToAsync(copy, cancellationToken);
                copy.Position = 0;

                return new StreamContent(copy);
            }
            finally
            {
                if (shouldRestorePosition)
                {
                    stream.Position = originalPosition;
                }
            }
        }
    }
}
