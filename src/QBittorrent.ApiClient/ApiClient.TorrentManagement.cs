using QBittorrent.ApiClient.Models;
using System.Globalization;
using System.Net;
using System.Text.Json;

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
            CancellationToken cancellationToken = default,
            params string[] hashes)
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
            if (hashes.Length > 0)
            {
                query.Add("hashes", string.Join('|', hashes));
            }
            if (isPrivate is not null)
            {
                query.Add("private", isPrivate.Value ? "true" : "false");
            }

            if (includeFiles is not null)
            {
                var profileResult = await GetCompatibilityProfileAsync(cancellationToken: cancellationToken);
                if (!profileResult.TryGetValue(out var profile))
                {
                    return profileResult.Failure.ToResult<IReadOnlyList<Torrent>>();
                }

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

        public Task<ApiResult<IReadOnlyList<FileData>>> GetTorrentContentsAsync(string hash, CancellationToken cancellationToken = default, params int[] indexes)
        {
            var query = new QueryBuilder();
            query.Add("hash", hash);
            if (indexes.Length > 0)
            {
                query.Add("indexes", string.Join('|', indexes));
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

        public Task<ApiResult> StopTorrentsAsync(bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/stop", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> StartTorrentsAsync(bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/start", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> DeleteTorrentsAsync(bool? all = null, bool deleteFiles = false, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .Add("deleteFiles", deleteFiles)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/delete", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> RecheckTorrentsAsync(bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/recheck", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> ReannounceTorrentsAsync(bool? all = null, IEnumerable<string>? trackers = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/reannounce", content, ct),
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult<AddTorrentResult>> AddTorrentAsync(AddTorrentParams addTorrentParams, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(addTorrentParams);

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
                content.AddString("ssl_certificate", addTorrentParams.SslCertificate!);
            }
            if (!string.IsNullOrWhiteSpace(addTorrentParams.SslPrivateKey))
            {
                content.AddString("ssl_private_key", addTorrentParams.SslPrivateKey!);
            }
            if (!string.IsNullOrWhiteSpace(addTorrentParams.SslDhParams))
            {
                content.AddString("ssl_dh_params", addTorrentParams.SslDhParams!);
            }
            if (!string.IsNullOrWhiteSpace(addTorrentParams.Cookie))
            {
                content.AddString("cookie", addTorrentParams.Cookie!);
            }

            async Task<ApiResult<AddTorrentResult>> HandleAddTorrentResponse(HttpResponseMessage response, string operation, CancellationToken currentCancellationToken)
            {
                async Task<AddTorrentResult> ReadAddTorrentResult(HttpContent httpContent, CancellationToken readCancellationToken)
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

                    var result = JsonSerializer.Deserialize<AddTorrentResult>(payload, _options);
                    if (result is null)
                    {
                        var count = (addTorrentParams.Torrents?.Count ?? 0) + (addTorrentParams.Urls?.Count() ?? 0);
                        return new AddTorrentResult(0, count);
                    }

                    return result;
                }

                ApiFailure? CreateAddTorrentFailure(HttpStatusCode statusCode, string? responseBody)
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

                return await CreateResultAsync(operation, response, ReadAddTorrentResult, currentCancellationToken, CreateAddTorrentFailure);
            }

            return await ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/add", content, ct),
                HandleAddTorrentResponse,
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult> AddTrackersToTorrentAsync(IEnumerable<string> urls, bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var applyToAll = all is true;
            var normalizedHashes = hashes ?? [];

            if (!applyToAll && normalizedHashes.Length == 0)
            {
                throw new ArgumentException("Specify at least one torrent hash or set all=true.", nameof(hashes));
            }

            var profileResult = await GetCompatibilityProfileAsync(cancellationToken: cancellationToken);
            if (!profileResult.TryGetValue(out var profile))
            {
                return profileResult.Failure.ToResult();
            }

            if (!profile.SupportsTrackerBatchOperations)
            {
                if (applyToAll)
                {
                    return CreateUnsupportedCompatibilityFailure(
                        nameof(AddTrackersToTorrentAsync),
                        profile,
                        $"qBittorrent Web API {profile.WebApiVersion} does not support adding trackers to all torrents in a single request.").ToResult();
                }

                if (normalizedHashes.Length > 1)
                {
                    return CreateUnsupportedCompatibilityFailure(
                        nameof(AddTrackersToTorrentAsync),
                        profile,
                        $"qBittorrent Web API {profile.WebApiVersion} does not support adding trackers to multiple torrents in a single request.").ToResult();
                }
            }

            var content = new FormUrlEncodedBuilder()
                .Add(
                    "hash",
                    applyToAll
                        ? profile.TrackerAllValue
                        : string.Join('|', normalizedHashes))
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

            var profileResult = await GetCompatibilityProfileAsync(cancellationToken: cancellationToken);
            if (!profileResult.TryGetValue(out var profile))
            {
                return profileResult.Failure.ToResult();
            }

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
                content
                    .Add("origUrl", url)
                    .Add("newUrl", normalizedNewUrl!);
            }

            var form = content.ToFormUrlEncodedContent();

            return await ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/editTracker", form, ct),
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult> RemoveTrackersAsync(IEnumerable<string> urls, bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var applyToAll = all is true;
            var normalizedHashes = hashes ?? [];

            if (!applyToAll && normalizedHashes.Length == 0)
            {
                throw new ArgumentException("Specify at least one torrent hash or set all=true.", nameof(hashes));
            }

            var profileResult = await GetCompatibilityProfileAsync(cancellationToken: cancellationToken);
            if (!profileResult.TryGetValue(out var profile))
            {
                return profileResult.Failure.ToResult();
            }

            if (!profile.SupportsTrackerBatchOperations && (normalizedHashes.Length > 1))
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(RemoveTrackersAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support removing trackers from multiple torrents in a single request.").ToResult();
            }

            var content = new FormUrlEncodedBuilder()
                .Add(
                    "hash",
                    applyToAll
                        ? profile.TrackerAllValue
                        : string.Join('|', normalizedHashes))
                .AddPipeSeparated("urls", urls)
                .ToFormUrlEncodedContent();

            return await ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/removeTrackers", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> AddPeersAsync(IEnumerable<string> hashes, IEnumerable<PeerId> peers, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddPipeSeparated("hashes", hashes)
                .AddPipeSeparated("peers", peers)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/addPeers", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> IncreaseTorrentPriorityAsync(bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/increasePrio", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> DecreaseTorrentPriorityAsync(bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/decreasePrio", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> MaxTorrentPriorityAsync(bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/topPrio", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> MinTorrentPriorityAsync(bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
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

        public Task<ApiResult<IReadOnlyDictionary<string, long>>> GetTorrentDownloadLimitAsync(bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/downloadLimit", content, ct),
                GetJsonDictionaryAsync<string, long>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetTorrentDownloadLimitAsync(long limit, bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .Add("limit", limit)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setDownloadLimit", content, ct),
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult> SetTorrentShareLimitAsync(
            float ratioLimit,
            float seedingTimeLimit,
            float inactiveSeedingTimeLimit,
            ShareLimitAction? shareLimitAction = null,
            bool? all = null,
            CancellationToken cancellationToken = default,
            params string[] hashes)
        {
            var profileResult = await GetCompatibilityProfileAsync(cancellationToken: cancellationToken);
            if (!profileResult.TryGetValue(out var profile))
            {
                return profileResult.Failure.ToResult();
            }

            if (profile.RequiresShareLimitAction && (shareLimitAction is null))
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(SetTorrentShareLimitAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} requires shareLimitAction when setting share limits.").ToResult();
            }

            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .Add("ratioLimit", ratioLimit)
                .Add("seedingTimeLimit", seedingTimeLimit)
                .Add("inactiveSeedingTimeLimit", inactiveSeedingTimeLimit);

            if (profile.RequiresShareLimitAction)
            {
                content.Add("shareLimitAction", shareLimitAction!.Value);
            }

            var form = content.ToFormUrlEncodedContent();

            return await ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setShareLimits", form, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyDictionary<string, long>>> GetTorrentUploadLimitAsync(bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/uploadLimit", content, ct),
                GetJsonDictionaryAsync<string, long>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetTorrentUploadLimitAsync(long limit, bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .Add("limit", limit)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setUploadLimit", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetTorrentLocationAsync(string location, bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .Add("location", location)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setLocation", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetTorrentSavePathAsync(IEnumerable<string> hashes, string path, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var hashArray = hashes?.Where(h => !string.IsNullOrWhiteSpace(h)).ToArray() ?? Array.Empty<string>();
            if (hashArray.Length == 0)
            {
                throw new ArgumentException("Specify at least one torrent hash.", nameof(hashes));
            }

            var content = new FormUrlEncodedBuilder()
                .Add("id", string.Join('|', hashArray))
                .Add("path", path)
                .ToFormUrlEncodedContent();

            Task<ApiResult> HandleSetTorrentSavePathResponse(HttpResponseMessage response, string operation, CancellationToken currentCancellationToken)
            {
                ApiFailure? CreateSetTorrentPathFailure(HttpStatusCode statusCode, string? responseBody)
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

                return CreateResultAsync(response, operation, currentCancellationToken, CreateSetTorrentPathFailure);
            }

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setSavePath", content, ct),
                HandleSetTorrentSavePathResponse,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetTorrentDownloadPathAsync(IEnumerable<string> hashes, string? path, CancellationToken cancellationToken = default)
        {
            var hashArray = hashes?.Where(h => !string.IsNullOrWhiteSpace(h)).ToArray() ?? Array.Empty<string>();
            if (hashArray.Length == 0)
            {
                throw new ArgumentException("Specify at least one torrent hash.", nameof(hashes));
            }

            var content = new FormUrlEncodedBuilder()
                .Add("id", string.Join('|', hashArray))
                .Add("path", path ?? string.Empty)
                .ToFormUrlEncodedContent();

            Task<ApiResult> HandleSetTorrentDownloadPathResponse(HttpResponseMessage response, string operation, CancellationToken currentCancellationToken)
            {
                ApiFailure? CreateSetTorrentPathFailure(HttpStatusCode statusCode, string? responseBody)
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

                return CreateResultAsync(response, operation, currentCancellationToken, CreateSetTorrentPathFailure);
            }

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setDownloadPath", content, ct),
                HandleSetTorrentDownloadPathResponse,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetTorrentNameAsync(string name, string hash, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("hash", hash)
                .Add("name", name)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/rename", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetTorrentCategoryAsync(string category, bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
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
                    builder.Add("downloadPath", downloadPathOption.Path!);
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
                    builder.Add("downloadPath", downloadPathOption.Path!);
                }
            }

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/editCategory", builder.ToFormUrlEncodedContent(), ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> RemoveCategoriesAsync(CancellationToken cancellationToken = default, params string[] categories)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("categories", string.Join('\n', categories))
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/removeCategories", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> AddTorrentTagsAsync(IEnumerable<string> tags, bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .AddCommaSeparated("tags", tags)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/addTags", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetTorrentTagsAsync(IEnumerable<string> tags, bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .AddCommaSeparated("tags", tags)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setTags", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> RemoveTorrentTagsAsync(IEnumerable<string> tags, bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
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

        public Task<ApiResult> DeleteTagsAsync(CancellationToken cancellationToken = default, params string[] tags)
        {
            var content = new FormUrlEncodedBuilder()
                .AddCommaSeparated("tags", tags)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/deleteTags", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetAutomaticTorrentManagementAsync(bool enable, bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .Add("enable", enable)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setAutoManagement", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> ToggleSequentialDownloadAsync(bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/toggleSequentialDownload", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetFirstLastPiecePriorityAsync(bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/toggleFirstLastPiecePrio", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetForceStartAsync(bool value, bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
                .Add("value", value)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setForceStart", content, ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetSuperSeedingAsync(bool value, bool? all = null, CancellationToken cancellationToken = default, params string[] hashes)
        {
            var content = new FormUrlEncodedBuilder()
                .AddAllOrPipeSeparated("hashes", all, hashes)
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

            return Task.FromResult(ApiResult<string>.Success(uriBuilder.Uri.AbsoluteUri));
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
                .Add("ssl_certificate", parameters.Certificate!)
                .Add("ssl_private_key", parameters.PrivateKey!)
                .Add("ssl_dh_params", parameters.DhParams!)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrents/setSSLParameters", content, ct),
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
