using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient
{
    internal partial class ApiClient
    {
        public Task<ApiResult<GlobalTransferInfo>> GetGlobalTransferInfoAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync("transfer/info", ct),
                GetJsonAsync<GlobalTransferInfo>,
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
    }
}
