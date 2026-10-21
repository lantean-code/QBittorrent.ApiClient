using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient
{
    internal partial class ApiClient
    {
        public Task<ApiResult<GlobalTransferStatistics>> GetGlobalTransferStatisticsAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync("transfer/info", ct),
                GetJsonAsync<GlobalTransferStatistics>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<bool>> GetAlternativeSpeedLimitsStateAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync("transfer/speedLimitsMode", ct),
                ReadBooleanFlagAsync,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetAlternativeSpeedLimitsStateAsync(bool enabled, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("mode", enabled ? 1 : 0)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("transfer/setSpeedLimitsMode", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> ToggleAlternativeSpeedLimitsAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(ct => _httpClient.PostAsync("transfer/toggleSpeedLimitsMode", null, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult<long>> GetGlobalDownloadLimitAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync("transfer/downloadLimit", ct),
                ReadInt64Async,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetGlobalDownloadLimitAsync(long limit, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("limit", limit)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("transfer/setDownloadLimit", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult<long>> GetGlobalUploadLimitAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync("transfer/uploadLimit", ct),
                ReadInt64Async,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> SetGlobalUploadLimitAsync(long limit, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("limit", limit)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("transfer/setUploadLimit", content, ct), cancellationToken: cancellationToken);
        }

        public Task<ApiResult> BanPeersAsync(IEnumerable<PeerId> peers, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .AddPipeSeparated("peers", peers)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("transfer/banPeers", content, ct), cancellationToken: cancellationToken);
        }

        public async Task<ApiResult<SpeedLimits>> GetSpeedLimitsAsync(CancellationToken cancellationToken = default)
        {
            var profile = CompatibilityProfile;
            if (!profile.SupportsSpeedLimitBatchOperations)
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(GetSpeedLimitsAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support retrieving all speed limits.").ToResult<SpeedLimits>();
            }

            return await ExecuteAsync(
                ct => _httpClient.GetAsync("transfer/getSpeedLimits", ct),
                GetJsonAsync<SpeedLimits>,
                cancellationToken: cancellationToken);
        }

        public async Task<ApiResult> SetSpeedLimitsAsync(SpeedLimits speedLimits, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(speedLimits);

            var profile = CompatibilityProfile;
            if (!profile.SupportsSpeedLimitBatchOperations)
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(SetSpeedLimitsAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support setting all speed limits.").ToResult();
            }

            var content = new FormUrlEncodedBuilder()
                .Add("up_limit", speedLimits.UploadLimit)
                .Add("dl_limit", speedLimits.DownloadLimit)
                .Add("alt_up_limit", speedLimits.AlternativeUploadLimit)
                .Add("alt_dl_limit", speedLimits.AlternativeDownloadLimit)
                .ToFormUrlEncodedContent();

            return await ExecuteAsync(ct => _httpClient.PostAsync("transfer/setSpeedLimits", content, ct), cancellationToken: cancellationToken);
        }

        public async Task<ApiResult> PauseSessionAsync(CancellationToken cancellationToken = default)
        {
            var profile = CompatibilityProfile;
            if (!profile.SupportsSessionPauseResume)
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(PauseSessionAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support pausing the session.").ToResult();
            }

            return await ExecuteAsync(ct => _httpClient.PostAsync("transfer/pauseSession", null, ct), cancellationToken: cancellationToken);
        }

        public async Task<ApiResult> ResumeSessionAsync(CancellationToken cancellationToken = default)
        {
            var profile = CompatibilityProfile;
            if (!profile.SupportsSessionPauseResume)
            {
                return CreateUnsupportedCompatibilityFailure(
                    nameof(ResumeSessionAsync),
                    profile,
                    $"qBittorrent Web API {profile.WebApiVersion} does not support resuming the session.").ToResult();
            }

            return await ExecuteAsync(ct => _httpClient.PostAsync("transfer/resumeSession", null, ct), cancellationToken: cancellationToken);
        }
    }
}
