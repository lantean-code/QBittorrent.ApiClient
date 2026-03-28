using QBittorrent.ApiClient.Models;
using System.Text.Json;

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
                var serializedKeys = JsonSerializer.Serialize(normalizedKeys, _options);
                contentBuilder.Add("keys", serializedKeys);
            }

            async Task<IReadOnlyDictionary<string, JsonElement>> ReadClientData(HttpContent content, CancellationToken currentCancellationToken)
            {
                return await GetJsonAsync<Dictionary<string, JsonElement>>(content, currentCancellationToken);
            }

            return await ExecuteAsync(
                ct => _httpClient.PostAsync("clientdata/load", contentBuilder.ToFormUrlEncodedContent(), ct),
                ReadClientData,
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult> StoreClientDataAsync(IReadOnlyDictionary<string, object?> data, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(data);

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

            var serializedData = JsonSerializer.Serialize(data, _options);
            var content = new FormUrlEncodedBuilder()
                .Add("data", serializedData)
                .ToFormUrlEncodedContent();

            return await ExecuteAsync(ct => _httpClient.PostAsync("clientdata/store", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult<BuildInfo>> GetBuildInfoAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync("app/buildInfo", ct),
                GetJsonAsync<BuildInfo>,
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

            var json = JsonSerializer.Serialize(preferences, _options);

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
            var json = JsonSerializer.Serialize(cookies, _options);

            var content = new FormUrlEncodedBuilder()
                .Add("cookies", json)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("app/setCookies", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SendTestEmailAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(ct => _httpClient.PostAsync("app/sendTestEmail", null, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyList<string>>> GetDirectoryContentAsync(string directoryPath, DirectoryContentMode mode = DirectoryContentMode.All, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);

            var query = new QueryBuilder()
                .Add("dirPath", directoryPath)
                .Add("mode", mode switch
                {
                    DirectoryContentMode.Directories => "dirs",
                    DirectoryContentMode.Files => "files",
                    _ => "all"
                });

            return ExecuteAsync(
                ct => _httpClient.GetAsync("app/getDirectoryContent", query, ct),
                GetJsonListAsync<string>,
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
    }
}
