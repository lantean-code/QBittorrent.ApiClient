using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient
{
    internal partial class ApiClient
    {
        public Task<ApiResult<string>> AddTorrentCreationTaskAsync(TorrentCreationTaskRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (string.IsNullOrWhiteSpace(request.SourcePath))
            {
                throw new ArgumentException("SourcePath is required.", nameof(request));
            }

            var builder = new FormUrlEncodedBuilder()
                .Add("sourcePath", request.SourcePath);

            if (!string.IsNullOrWhiteSpace(request.TorrentFilePath))
            {
                builder.Add("torrentFilePath", request.TorrentFilePath);
            }
            if (request.PieceSize.HasValue)
            {
                builder.Add("pieceSize", request.PieceSize.Value);
            }
            if (request.Private.HasValue)
            {
                builder.Add("private", request.Private.Value);
            }
            if (request.StartSeeding.HasValue)
            {
                builder.Add("startSeeding", request.StartSeeding.Value);
            }
            if (!string.IsNullOrWhiteSpace(request.Comment))
            {
                builder.Add("comment", request.Comment);
            }
            if (!string.IsNullOrWhiteSpace(request.Source))
            {
                builder.Add("source", request.Source);
            }
            if (request.Trackers is not null)
            {
                builder.Add("trackers", string.Join('|', request.Trackers));
            }
            if (request.UrlSeeds is not null)
            {
                builder.Add("urlSeeds", string.Join('|', request.UrlSeeds));
            }
            if (request.Format.HasValue)
            {
                builder.Add("format", request.Format.Value switch
                {
                    TorrentFormat.V1 => "v1",
                    TorrentFormat.V2 => "v2",
                    _ => "hybrid"
                });
            }
            if (request.OptimizeAlignment.HasValue)
            {
                builder.Add("optimizeAlignment", request.OptimizeAlignment.Value);
            }
            if (request.PaddedFileSizeLimit.HasValue)
            {
                builder.Add("paddedFileSizeLimit", request.PaddedFileSizeLimit.Value);
            }

            static async Task<string> readTaskId(HttpContent content, CancellationToken currentCancellationToken)
            {
                var rawPayload = await content.ReadAsStringAsync(currentCancellationToken);
                if (string.IsNullOrWhiteSpace(rawPayload))
                {
                    return string.Empty;
                }

                try
                {
                    var payload = DeserializeJson<TorrentCreationTaskIdentifier>(rawPayload);
                    return payload?.TaskId ?? string.Empty;
                }
                catch (System.Text.Json.JsonException)
                {
                    return string.Empty;
                }
            }

            return ExecuteAsync(
                ct => _httpClient.PostAsync("torrentcreator/addTask", builder.ToFormUrlEncodedContent(), ct),
                readTaskId,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<IReadOnlyList<TorrentCreationTaskStatus>>> GetTorrentCreationTasksAsync(string? taskId = null, CancellationToken cancellationToken = default)
        {
            var query = new QueryBuilder();
            if (!string.IsNullOrWhiteSpace(taskId))
            {
                query.Add("taskID", taskId);
            }

            return ExecuteAsync(
                ct => _httpClient.GetAsync("torrentcreator/status", query, ct),
                GetJsonListAsync<TorrentCreationTaskStatus>,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult<byte[]>> GetTorrentCreationTaskFileAsync(string taskId, CancellationToken cancellationToken = default)
        {
            var query = new QueryBuilder()
                .Add("taskID", taskId);

            return ExecuteAsync(
                ct => _httpClient.GetAsync("torrentcreator/torrentFile", query, ct),
                (content, ct) => content.ReadAsByteArrayAsync(ct),
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> DeleteTorrentCreationTaskAsync(string taskId, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("taskID", taskId)
                .ToFormUrlEncodedContent();

            return ExecuteAsync(ct => _httpClient.PostAsync("torrentcreator/deleteTask", content, ct), cancellationToken: cancellationToken);
        }
    }
}
