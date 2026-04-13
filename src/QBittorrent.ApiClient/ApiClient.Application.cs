using System.Text.Json;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient
{
    internal partial class ApiClient
    {
        public Task<ApiResult<string>> GetApplicationVersionAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync("app/version", ct),
                (content, ct) => content.ReadAsStringAsync(ct),
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult<string>> GetAPIVersionAsync(CancellationToken cancellationToken = default)
        {
            if (_compatibilityProfileCache.TryGetValue(GetCompatibilityProfileCacheKey(), out var compatibilityProfile))
            {
                return ApiResult<string>.Success(compatibilityProfile.WebApiVersion.ToString());
            }

            var result = await GetRawApiVersionAsync(cancellationToken);
            if (result.TryGetValue(out var apiVersion))
            {
                await HydrateCompatibilityProfileAsync(apiVersion, cancellationToken);
            }

            return result;
        }

        private Task<ApiResult<string>> GetRawApiVersionAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync("app/webapiVersion", ct),
                (content, ct) => content.ReadAsStringAsync(ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> RefreshCompatibilityAsync(CancellationToken cancellationToken = default)
        {
            return RefreshCompatibilityCoreAsync(cancellationToken);
        }

        public async Task<ApiResult<IReadOnlyDictionary<string, JsonElement>>> LoadClientDataAsync(IEnumerable<string>? keys = null, CancellationToken cancellationToken = default)
        {
            var normalizedKeys = keys?
                .Where(key => !string.IsNullOrWhiteSpace(key))
                .Select(key => key.Trim())
                .Distinct(StringComparer.Ordinal)
                .ToArray() ?? [];

            var profileResult = await GetCompatibilityProfileAsync(cancellationToken: cancellationToken);
            if (!profileResult.TryGetValue(out var profile))
            {
                return profileResult.Failure.ToResult<IReadOnlyDictionary<string, JsonElement>>();
            }

            if (!profile.SupportsClientData)
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(LoadClientDataAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support the client data API.").ToResult<IReadOnlyDictionary<string, JsonElement>>();
            }

            var contentBuilder = new FormUrlEncodedBuilder();
            if (normalizedKeys.Length > 0)
            {
                var serializedKeys = SerializeJson(normalizedKeys);
                contentBuilder.Add("keys", serializedKeys);
            }

            static async Task<IReadOnlyDictionary<string, JsonElement>> readClientData(HttpContent content, CancellationToken currentCancellationToken)
            {
                return await GetJsonAsync<Dictionary<string, JsonElement>>(content, currentCancellationToken);
            }

            return await ExecuteAsync(
                ct => _httpClient.PostAsync("clientdata/load", contentBuilder.ToFormUrlEncodedContent(), ct),
                readClientData,
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult> StoreClientDataAsync(IReadOnlyDictionary<string, JsonElement?> data, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(data);

            var normalizedData = new Dictionary<string, JsonElement?>(data.Count, StringComparer.Ordinal);
            foreach (var entry in data)
            {
                if (entry.Value is { ValueKind: JsonValueKind.Null })
                {
                    throw new ArgumentException("JSON null values are not supported. Use a null dictionary entry to delete a client-data key.", nameof(data));
                }

                if (entry.Value is { ValueKind: JsonValueKind.Undefined })
                {
                    throw new ArgumentException("Undefined JsonElement values are not supported. Use a populated JsonElement or a null dictionary entry to delete a client-data key.", nameof(data));
                }

                normalizedData[entry.Key] = entry.Value;
            }

            var profileResult = await GetCompatibilityProfileAsync(cancellationToken: cancellationToken);
            if (!profileResult.TryGetValue(out var profile))
            {
                return profileResult.Failure.ToResult();
            }

            if (!profile.SupportsClientData)
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(StoreClientDataAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support the client data API.").ToResult();
            }

            var serializedData = SerializeJson(normalizedData);
            var content = new FormUrlEncodedBuilder()
                .Add("data", serializedData)
                .ToFormUrlEncodedContent();

            return await ExecuteAsync(ct => _httpClient.PostAsync("clientdata/store", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> UpsertClientDataAsync(IReadOnlyDictionary<string, JsonElement> data, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(data);

            var patch = new Dictionary<string, JsonElement?>(data.Count, StringComparer.Ordinal);
            foreach (var entry in data)
            {
                if (entry.Value.ValueKind == JsonValueKind.Null)
                {
                    throw new ArgumentException("JSON null values are not supported. Use DeleteClientDataAsync to remove a client-data key.", nameof(data));
                }

                if (entry.Value.ValueKind == JsonValueKind.Undefined)
                {
                    throw new ArgumentException("Undefined JsonElement values are not supported. Use a populated JsonElement or DeleteClientDataAsync to remove a client-data key.", nameof(data));
                }

                patch[entry.Key] = entry.Value;
            }

            return StoreClientDataAsync(patch, cancellationToken);
        }

        public Task<ApiResult> DeleteClientDataAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(keys);

            var patch = keys
                .Where(key => !string.IsNullOrWhiteSpace(key))
                .Select(key => key.Trim())
                .Distinct(StringComparer.Ordinal)
                .ToDictionary(key => key, _ => (JsonElement?)null, StringComparer.Ordinal);

            return StoreClientDataAsync(patch, cancellationToken);
        }

        public Task<ApiResult<BuildInfo>> GetBuildInfoAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync("app/buildInfo", ct),
                GetJsonAsync<BuildInfo>,
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult<ProcessInfo>> GetProcessInfoAsync(CancellationToken cancellationToken = default)
        {
            var profileResult = await GetCompatibilityProfileAsync(cancellationToken: cancellationToken);
            if (!profileResult.TryGetValue(out var profile))
            {
                return profileResult.Failure.ToResult<ProcessInfo>();
            }

            if (!profile.SupportsProcessInfo)
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(GetProcessInfoAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support process info.").ToResult<ProcessInfo>();
            }

            return await ExecuteAsync(
                ct => _httpClient.GetAsync("app/processInfo", ct),
                GetJsonAsync<ProcessInfo>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> ShutdownAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(ct => _httpClient.PostAsync("app/shutdown", null, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult<Preferences>> GetApplicationPreferencesAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync("app/preferences", ct),
                GetJsonAsync<Preferences>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetApplicationPreferencesAsync(UpdatePreferences preferences, CancellationToken cancellationToken = default)
        {
            preferences.Validate();

            var json = SerializeJson(preferences);

            var content = new FormUrlEncodedBuilder()
                .Add("json", json)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("app/setPreferences", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyList<ApplicationCookie>>> GetApplicationCookiesAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync("app/cookies", ct),
                GetJsonListAsync<ApplicationCookie>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetApplicationCookiesAsync(IEnumerable<ApplicationCookie> cookies, CancellationToken cancellationToken = default)
        {
            var serializedCookies = cookies.ToList();
            var json = SerializeJson(serializedCookies);

            var content = new FormUrlEncodedBuilder()
                .Add("cookies", json)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("app/setCookies", content, ct), cancellationToken: cancellationToken);
        }

        public async Task<ApiResult<ApiKey>> RotateAPIKeyAsync(CancellationToken cancellationToken = default)
        {
            var profileResult = await GetCompatibilityProfileAsync(cancellationToken: cancellationToken);
            if (!profileResult.TryGetValue(out var profile))
            {
                return profileResult.Failure.ToResult<ApiKey>();
            }

            if (!profile.SupportsApiKeyManagement)
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(RotateAPIKeyAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support Web API key rotation.").ToResult<ApiKey>();
            }

            return await ExecuteAsync(
                ct => _httpClient.PostAsync("app/rotateAPIKey", null, ct),
                GetJsonAsync<ApiKey>,
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult> DeleteAPIKeyAsync(CancellationToken cancellationToken = default)
        {
            var profileResult = await GetCompatibilityProfileAsync(cancellationToken: cancellationToken);
            if (!profileResult.TryGetValue(out var profile))
            {
                return profileResult.Failure.ToResult();
            }

            if (!profile.SupportsApiKeyManagement)
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(DeleteAPIKeyAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support Web API key deletion.").ToResult();
            }

            return await ExecuteAsync(ct => _httpClient.PostAsync("app/deleteAPIKey", null, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SendTestEmailAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(ct => _httpClient.PostAsync("app/sendTestEmail", null, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyList<string>>> GetDirectoryContentAsync(string directoryPath, DirectoryContentMode mode = DirectoryContentMode.All, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);

            var query = BuildDirectoryContentQuery(directoryPath, mode);

            return ExecuteAsync(
                ct => _httpClient.GetAsync("app/getDirectoryContent", query, ct),
                GetJsonListAsync<string>,
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult<IReadOnlyList<DirectoryContentEntry>>> GetDirectoryContentEntriesAsync(string directoryPath, DirectoryContentMode mode = DirectoryContentMode.All, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);

            var profileResult = await GetCompatibilityProfileAsync(cancellationToken: cancellationToken);
            if (!profileResult.TryGetValue(out var profile))
            {
                return profileResult.Failure.ToResult<IReadOnlyList<DirectoryContentEntry>>();
            }

            if (!profile.SupportsDirectoryContentMetadata)
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(GetDirectoryContentEntriesAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support directory metadata responses.").ToResult<IReadOnlyList<DirectoryContentEntry>>();
            }

            var query = BuildDirectoryContentQuery(directoryPath, mode)
                .Add("withMetadata", true);

            return await ExecuteAsync(
                ct => _httpClient.GetAsync("app/getDirectoryContent", query, ct),
                GetJsonListAsync<DirectoryContentEntry>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<string>> GetDefaultSavePathAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync("app/defaultSavePath", ct),
                (content, ct) => content.ReadAsStringAsync(ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyList<NetworkInterface>>> GetNetworkInterfacesAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync("app/networkInterfaceList", ct),
                GetJsonListAsync<NetworkInterface>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyList<string>>> GetNetworkInterfaceAddressListAsync(string @interface, CancellationToken cancellationToken = default)
        {
            var query = new QueryBuilder()
                .Add("iface", @interface);

            return ExecuteAsync(
                ct => _httpClient.GetAsync("app/networkInterfaceAddressList", query, ct),
                GetJsonListAsync<string>,
                cancellationToken: cancellationToken);
        }

        private static QueryBuilder BuildDirectoryContentQuery(string directoryPath, DirectoryContentMode mode)
        {
            return new QueryBuilder()
                .Add("dirPath", directoryPath)
                .Add("mode", mode switch
                {
                    DirectoryContentMode.Directories => "dirs",
                    DirectoryContentMode.Files => "files",
                    _ => "all"
                });
        }
    }
}
