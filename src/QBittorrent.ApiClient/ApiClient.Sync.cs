using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient
{
    internal partial class ApiClient
    {
        public Task<ApiResult<MainData>> GetMainDataAsync(int requestId, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync($"sync/maindata?rid={requestId}", ct),
                GetJsonAsync<MainData>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<TorrentPeers>> GetTorrentPeersDataAsync(string hash, int requestId, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                ct => _httpClient.GetAsync($"sync/torrentPeers?hash={hash}&rid={requestId}", ct),
                GetJsonAsync<TorrentPeers>,
                cancellationToken: cancellationToken);
        }
    }
}
