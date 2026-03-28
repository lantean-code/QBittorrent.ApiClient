using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient
{
    internal partial class ApiClient
    {
        public Task<ApiResult<IReadOnlyList<Log>>> GetLogAsync(
            bool? normal = null,
            bool? info = null,
            bool? warning = null,
            bool? critical = null,
            int? lastKnownId = null,
            CancellationToken cancellationToken = default)
        {
            var query = new QueryBuilder();
            if (normal is not null)
            {
                query.Add("normal", normal.Value);
            }
            if (info is not null)
            {
                query.Add("info", info.Value);
            }
            if (warning is not null)
            {
                query.Add("warning", warning.Value);
            }
            if (critical is not null)
            {
                query.Add("critical", critical.Value);
            }
            if (lastKnownId is not null)
            {
                query.Add("last_known_id", lastKnownId.Value);
            }

            return ExecuteAsync(
                ct => _httpClient.GetAsync("log/main", query, ct),
                GetJsonListAsync<Log>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyList<PeerLog>>> GetPeerLogAsync(int? lastKnownId = null, CancellationToken cancellationToken = default)
        {
            var query = new QueryBuilder();
            if (lastKnownId is not null)
            {
                query.Add("last_known_id", lastKnownId.Value);
            }

            return ExecuteAsync(
                ct => _httpClient.GetAsync("log/peers", query, ct),
                GetJsonListAsync<PeerLog>,
                cancellationToken: cancellationToken);
        }
    }
}
