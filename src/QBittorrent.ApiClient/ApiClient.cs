using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient
{
    internal partial class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ApiClientCompatibilityProfileCache _compatibilityProfileCache;

        internal ApiClient(HttpClient httpClient, ApiClientCompatibilityProfileCache? compatibilityProfileCache = null)
        {
            _httpClient = httpClient;
            _compatibilityProfileCache = compatibilityProfileCache ?? new ApiClientCompatibilityProfileCache();
        }

        private Task<ApiResult> ExecuteAsync(
            Func<CancellationToken, Task<HttpResponseMessage>> sendRequest,
            [CallerMemberName] string operation = "",
            CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                sendRequest,
                (response, currentOperation, currentCancellationToken) => CreateResultAsync(response, currentOperation, currentCancellationToken),
                operation,
                cancellationToken);
        }

        private async Task<ApiResult> ExecuteAsync(
            Func<CancellationToken, Task<HttpResponseMessage>> sendRequest,
            Func<HttpResponseMessage, string, CancellationToken, Task<ApiResult>> handleResponse,
            [CallerMemberName] string operation = "",
            CancellationToken cancellationToken = default)
        {
            var sendResult = await SendAsync(operation, sendRequest, cancellationToken);
            if (sendResult.IsFailure)
            {
                return sendResult.Failure.ToResult();
            }

            if (!sendResult.TryGetValue(out var response))
            {
                throw new InvalidOperationException("Expected a completed send result.");
            }

            return await handleResponse(response, operation, cancellationToken);
        }

        private async Task<ApiResult<T>> ExecuteAsync<T>(
            Func<CancellationToken, Task<HttpResponseMessage>> sendRequest,
            Func<HttpResponseMessage, string, CancellationToken, Task<ApiResult<T>>> handleResponse,
            [CallerMemberName] string operation = "",
            CancellationToken cancellationToken = default)
            where T : notnull
        {
            var sendResult = await SendAsync(operation, sendRequest, cancellationToken);
            if (sendResult.IsFailure)
            {
                return sendResult.Failure.ToResult<T>();
            }

            if (!sendResult.TryGetValue(out var response))
            {
                throw new InvalidOperationException("Expected a completed send result.");
            }

            return await handleResponse(response, operation, cancellationToken);
        }

        private async Task<ApiResult<TSuccess, TPending>> ExecuteAsync<TSuccess, TPending>(
            Func<CancellationToken, Task<HttpResponseMessage>> sendRequest,
            Func<HttpResponseMessage, string, CancellationToken, Task<ApiResult<TSuccess, TPending>>> handleResponse,
            [CallerMemberName] string operation = "",
            CancellationToken cancellationToken = default)
            where TSuccess : notnull
            where TPending : notnull
        {
            var sendResult = await SendAsync(operation, sendRequest, cancellationToken);
            if (sendResult.IsFailure)
            {
                return sendResult.Failure.ToResult<TSuccess, TPending>();
            }

            if (!sendResult.TryGetValue(out var response))
            {
                throw new InvalidOperationException("Expected a completed send result.");
            }

            return await handleResponse(response, operation, cancellationToken);
        }

        private Task<ApiResult<T>> ExecuteAsync<T>(
            Func<CancellationToken, Task<HttpResponseMessage>> sendRequest,
            Func<HttpContent, CancellationToken, Task<T>> readValue,
            [CallerMemberName] string operation = "",
            CancellationToken cancellationToken = default)
            where T : notnull
        {
            return ExecuteAsync(
                sendRequest,
                (response, currentOperation, currentCancellationToken) => CreateResultAsync(currentOperation, response, readValue, currentCancellationToken),
                operation,
                cancellationToken);
        }

        private async Task<ApiResult<HttpResponseMessage>> SendAsync(string operation, Func<CancellationToken, Task<HttpResponseMessage>> sendRequest, CancellationToken cancellationToken)
        {
            try
            {
                return ApiResult.CreateSuccess(await sendRequest(cancellationToken));
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (HttpRequestException exception)
            {
                return CreateNoResponseFailure(operation, exception).ToResult<HttpResponseMessage>();
            }
            catch (InvalidOperationException exception) when (_httpClient.BaseAddress is null)
            {
                return CreateConfigurationFailure(operation, "HttpClient BaseAddress must be configured.", exception).ToResult<HttpResponseMessage>();
            }
            catch (TaskCanceledException exception)
            {
                return CreateTimeoutFailure(operation, exception).ToResult<HttpResponseMessage>();
            }
        }

        private async Task<ApiResult<ApiClientCompatibilityProfile>> GetCompatibilityProfileAsync(
            [CallerMemberName] string operation = "",
            CancellationToken cancellationToken = default)
        {
            var cacheKey = GetCompatibilityProfileCacheKey();

            return await _compatibilityProfileCache.GetOrAddAsync(
                cacheKey,
                ct => ResolveCompatibilityProfileAsync(operation, ct),
                cancellationToken);
        }

        private async Task<ApiResult> RefreshCompatibilityCoreAsync(CancellationToken cancellationToken = default)
        {
            var cacheKey = GetCompatibilityProfileCacheKey();

            return await _compatibilityProfileCache.RefreshAsync(
                cacheKey,
                ct => ResolveCompatibilityProfileAsync(nameof(RefreshCompatibilityAsync), ct),
                cancellationToken);
        }

        private async Task<ApiResult<ApiClientCompatibilityProfile>> ResolveCompatibilityProfileAsync(string operation, CancellationToken cancellationToken)
        {
            var apiVersionResult = await GetRawApiVersionAsync(cancellationToken);
            if (apiVersionResult.IsFailure)
            {
                return apiVersionResult.Failure.ToResult<ApiClientCompatibilityProfile>();
            }

            if (!apiVersionResult.TryGetValue(out var rawApiVersion))
            {
                throw new InvalidOperationException("Expected a completed API version result.");
            }

            rawApiVersion = NormalizeResponseBody(rawApiVersion);
            if ((rawApiVersion is null) || !Version.TryParse(rawApiVersion, out var parsedApiVersion))
            {
                return CreateCompatibilityResolutionFailure(operation, rawApiVersion).ToResult<ApiClientCompatibilityProfile>();
            }

            return ApiResult.CreateSuccess(new ApiClientCompatibilityProfile(parsedApiVersion));
        }

        private async Task HydrateCompatibilityProfileAsync(string? rawApiVersion, CancellationToken cancellationToken = default)
        {
            if (!TryCreateCompatibilityProfile(rawApiVersion, out var profile))
            {
                return;
            }

            await _compatibilityProfileCache.TryHydrateAsync(GetCompatibilityProfileCacheKey(), profile, cancellationToken);
        }

        private string GetCompatibilityProfileCacheKey()
        {
            return _httpClient.BaseAddress?.AbsoluteUri ?? string.Empty;
        }

        private static bool TryCreateCompatibilityProfile(string? rawApiVersion, [NotNullWhen(true)] out ApiClientCompatibilityProfile? profile)
        {
            rawApiVersion = NormalizeResponseBody(rawApiVersion);
            if ((rawApiVersion is null) || !Version.TryParse(rawApiVersion, out var parsedApiVersion))
            {
                profile = null;
                return false;
            }

            profile = new ApiClientCompatibilityProfile(parsedApiVersion);
            return true;
        }

        private static async Task<ApiResult> CreateResultAsync(
            HttpResponseMessage response,
            string operation,
            CancellationToken cancellationToken,
            Func<HttpStatusCode, string?, ApiFailure?>? createFailure = null)
        {
            using (response)
            {
                var failure = await TryCreateFailureAsync(operation, response, cancellationToken, createFailure);
                return failure is null
                    ? ApiResult.CreateSuccess()
                    : failure.ToResult();
            }
        }

        private static async Task<ApiResult<T>> CreateResultAsync<T>(
            string operation,
            HttpResponseMessage response,
            Func<HttpContent, CancellationToken, Task<T>> readValue,
            CancellationToken cancellationToken,
            Func<HttpStatusCode, string?, ApiFailure?>? createFailure = null)
            where T : notnull
        {
            using (response)
            {
                var failure = await TryCreateFailureAsync(operation, response, cancellationToken, createFailure);
                if (failure is not null)
                {
                    return failure.ToResult<T>();
                }

                return await TryReadValueAsync(operation, response.Content, readValue, cancellationToken);
            }
        }

        private static async Task<ApiResult<T>> CreatePendingResultAsync<T>(
            string operation,
            HttpResponseMessage response,
            Func<HttpContent, CancellationToken, Task<T>> readPendingValue,
            CancellationToken cancellationToken)
            where T : notnull
        {
            using (response)
            {
                var readResult = await TryReadValueAsync(operation, response.Content, readPendingValue, cancellationToken);
                if (readResult.IsFailure)
                {
                    return readResult.Failure.ToResult<T>();
                }

                if (!readResult.TryGetValue(out var pendingValue))
                {
                    throw new InvalidOperationException("Expected a completed pending-value result.");
                }

                return ApiResult.CreatePending(pendingValue);
            }
        }

        private static async Task<ApiResult<TSuccess, TPending>> CreatePendingResultAsync<TSuccess, TPending>(
            string operation,
            HttpResponseMessage response,
            Func<HttpContent, CancellationToken, Task<TPending>> readPendingValue,
            CancellationToken cancellationToken)
            where TSuccess : notnull
            where TPending : notnull
        {
            using (response)
            {
                var readResult = await TryReadValueAsync(operation, response.Content, readPendingValue, cancellationToken);
                if (readResult.IsFailure)
                {
                    return readResult.Failure.ToResult<TSuccess, TPending>();
                }

                if (!readResult.TryGetValue(out var pendingValue))
                {
                    throw new InvalidOperationException("Expected a completed pending-value result.");
                }

                return ApiResult.CreatePending<TSuccess, TPending>(pendingValue);
            }
        }

        private static async Task<ApiResult<T>> TryReadValueAsync<T>(
            string operation,
            HttpContent content,
            Func<HttpContent, CancellationToken, Task<T>> readValue,
            CancellationToken cancellationToken)
            where T : notnull
        {
            try
            {
                return ApiResult.CreateSuccess(await readValue(content, cancellationToken));
            }
            catch (JsonException exception)
            {
                return CreateUnexpectedResponseFailure(operation, exception).ToResult<T>();
            }
            catch (ResponseDeserializationException exception)
            {
                return CreateUnexpectedResponseFailure(operation, exception).ToResult<T>();
            }
        }

        private static async Task<ApiFailure?> TryCreateFailureAsync(
            string operation,
            HttpResponseMessage response,
            CancellationToken cancellationToken,
            Func<HttpStatusCode, string?, ApiFailure?>? createFailure = null)
        {
            if (response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseBody = NormalizeResponseBody(await response.Content.ReadAsStringAsync(cancellationToken));
            return createFailure?.Invoke(response.StatusCode, responseBody) ?? CreateFailure(operation, response.StatusCode, responseBody);
        }

        private static ApiFailure CreateNoResponseFailure(string operation, HttpRequestException exception)
        {
            var detail = string.IsNullOrWhiteSpace(exception.Message) ? null : exception.Message;

            return new ApiFailure
            {
                Kind = ApiFailureKind.NoResponse,
                Operation = operation,
                UserMessage = detail ?? "qBittorrent client is not reachable.",
                Detail = detail,
                IsTransient = true,
                ResponseBody = detail,
            };
        }

        private static ApiFailure CreateConfigurationFailure(string operation, string userMessage, Exception? exception = null)
        {
            var detail = exception?.Message ?? userMessage;

            return new ApiFailure
            {
                Kind = ApiFailureKind.InvalidConfiguration,
                Operation = operation,
                UserMessage = userMessage,
                Detail = detail,
                ResponseBody = detail,
            };
        }

        private static ApiFailure CreateFailure(string operation, HttpStatusCode statusCode, string? responseBody)
        {
            return statusCode switch
            {
                HttpStatusCode.BadRequest => CreateBadRequestFailure(operation, responseBody, statusCode),
                HttpStatusCode.Unauthorized => CreateUnauthorizedFailure(operation, responseBody, statusCode),
                HttpStatusCode.Forbidden => CreateForbiddenFailure(operation, responseBody, statusCode),
                HttpStatusCode.NotFound => CreateNotFoundFailure(operation, responseBody, statusCode),
                HttpStatusCode.Conflict => CreateConflictFailure(operation, responseBody, statusCode),
                HttpStatusCode.UnsupportedMediaType => CreateUnsupportedDataFailure(operation, responseBody, statusCode),
                _ when (int)statusCode >= 500 => CreateServerFailure(operation, responseBody, statusCode),
                _ => new ApiFailure
                {
                    Kind = ApiFailureKind.UnexpectedResponse,
                    Operation = operation,
                    StatusCode = statusCode,
                    UserMessage = responseBody ?? $"Unexpected API response ({(int)statusCode}).",
                    Detail = responseBody,
                    ResponseBody = responseBody,
                }
            };
        }

        private static string? NormalizeResponseBody(string? responseBody)
        {
            return string.IsNullOrWhiteSpace(responseBody) ? null : responseBody;
        }

        private static ApiFailure CreateBadRequestFailure(string operation, string? responseBody, HttpStatusCode statusCode)
        {
            return new ApiFailure
            {
                Kind = ApiFailureKind.ValidationFailed,
                Operation = operation,
                StatusCode = statusCode,
                UserMessage = responseBody ?? "The request was rejected.",
                Detail = responseBody,
                ResponseBody = responseBody,
            };
        }

        private static ApiFailure CreateUnauthorizedFailure(string operation, string? responseBody, HttpStatusCode statusCode)
        {
            return new ApiFailure
            {
                Kind = ApiFailureKind.AuthenticationRequired,
                Operation = operation,
                StatusCode = statusCode,
                UserMessage = responseBody ?? "Authentication is required.",
                Detail = responseBody,
                ResponseBody = responseBody,
            };
        }

        private static ApiFailure CreateForbiddenFailure(string operation, string? responseBody, HttpStatusCode statusCode)
        {
            return new ApiFailure
            {
                Kind = ApiFailureKind.AuthenticationRequired,
                Operation = operation,
                StatusCode = statusCode,
                UserMessage = responseBody ?? "Authentication is required.",
                Detail = responseBody,
                ResponseBody = responseBody,
            };
        }

        private static ApiFailure CreateNotFoundFailure(string operation, string? responseBody, HttpStatusCode statusCode)
        {
            return new ApiFailure
            {
                Kind = ApiFailureKind.NotFound,
                Operation = operation,
                StatusCode = statusCode,
                UserMessage = responseBody ?? "The requested resource could not be found.",
                Detail = responseBody,
                ResponseBody = responseBody,
            };
        }

        private static ApiFailure CreateConflictFailure(string operation, string? responseBody, HttpStatusCode statusCode)
        {
            return new ApiFailure
            {
                Kind = ApiFailureKind.Conflict,
                Operation = operation,
                StatusCode = statusCode,
                UserMessage = responseBody ?? "The request conflicts with the current server state.",
                Detail = responseBody,
                ResponseBody = responseBody,
            };
        }

        private static ApiFailure CreateUnsupportedDataFailure(string operation, string? responseBody, HttpStatusCode statusCode)
        {
            return new ApiFailure
            {
                Kind = ApiFailureKind.UnsupportedData,
                Operation = operation,
                StatusCode = statusCode,
                UserMessage = responseBody ?? "The supplied data is not supported.",
                Detail = responseBody,
                ResponseBody = responseBody,
            };
        }

        private static ApiFailure CreateServerFailure(string operation, string? responseBody, HttpStatusCode statusCode)
        {
            return new ApiFailure
            {
                Kind = ApiFailureKind.ServerError,
                Operation = operation,
                StatusCode = statusCode,
                UserMessage = responseBody ?? "qBittorrent returned a server error.",
                Detail = responseBody,
                IsTransient = true,
                ResponseBody = responseBody,
            };
        }

        private static ApiFailure CreateTimeoutFailure(string operation, Exception exception)
        {
            return new ApiFailure
            {
                Kind = ApiFailureKind.Timeout,
                Operation = operation,
                UserMessage = "The request timed out before qBittorrent responded.",
                Detail = exception.Message,
                IsTransient = true,
            };
        }

        private static ApiFailure CreateUnexpectedResponseFailure(string operation, Exception exception)
        {
            return new ApiFailure
            {
                Kind = ApiFailureKind.UnexpectedResponse,
                Operation = operation,
                UserMessage = "qBittorrent returned an unexpected response.",
                Detail = exception.Message,
            };
        }

        private static ApiFailure CreateCompatibilityResolutionFailure(string operation, string? rawApiVersion)
        {
            var detail = rawApiVersion is null
                ? "qBittorrent did not return a Web API version."
                : $"qBittorrent returned an unsupported Web API version value: {rawApiVersion}";

            return new ApiFailure
            {
                Kind = ApiFailureKind.UnexpectedResponse,
                Operation = operation,
                UserMessage = "Unable to determine the qBittorrent Web API version.",
                Detail = detail,
                ResponseBody = rawApiVersion,
            };
        }

        private static ApiFailure CreateUnsupportedCompatibilityFailure(
            string operation,
            ApiClientCompatibilityProfile profile,
            string unsupportedMessage)
        {
            return new ApiFailure
            {
                Kind = ApiFailureKind.ValidationFailed,
                Operation = operation,
                UserMessage = unsupportedMessage,
                Detail = $"Connected qBittorrent Web API version: {profile.WebApiVersion}",
            };
        }

        private static JsonTypeInfo<T> GetJsonTypeInfo<T>()
        {
            return SerializerOptions.GetTypeInfo<T>();
        }

        private static string SerializeJson<T>(T value)
        {
            return JsonSerializer.Serialize(value, GetJsonTypeInfo<T>());
        }

        private static T? DeserializeJson<T>(string json)
        {
            return JsonSerializer.Deserialize(json, GetJsonTypeInfo<T>());
        }

        private static async Task<T> GetJsonAsync<T>(HttpContent content, CancellationToken cancellationToken)
        {
            return await content.ReadFromJsonAsync(GetJsonTypeInfo<T>(), cancellationToken) ?? throw new ResponseDeserializationException(typeof(T).Name);
        }

        private static async Task<IReadOnlyList<T>> GetJsonListAsync<T>(HttpContent content, CancellationToken cancellationToken)
        {
            var items = await GetJsonAsync<List<T>>(content, cancellationToken);

            return items.AsReadOnly();
        }

        private static async Task<IReadOnlyDictionary<TKey, TValue>> GetJsonDictionaryAsync<TKey, TValue>(HttpContent content, CancellationToken cancellationToken) where TKey : notnull
        {
            var items = await GetJsonAsync<Dictionary<TKey, TValue>>(content, cancellationToken);

            return items.AsReadOnly();
        }

        private async Task<bool> ReadBooleanFlagAsync(HttpContent content, CancellationToken cancellationToken)
        {
            var value = await content.ReadAsStringAsync(cancellationToken);
            return value switch
            {
                "1" => true,
                "0" => false,
                _ => throw new ResponseDeserializationException("Boolean"),
            };
        }

        private async Task<int> ReadInt32Async(HttpContent content, CancellationToken cancellationToken)
        {
            var value = await content.ReadAsStringAsync(cancellationToken);
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            {
                throw new ResponseDeserializationException("Int32");
            }

            return result;
        }

        private async Task<long> ReadInt64Async(HttpContent content, CancellationToken cancellationToken)
        {
            var value = await content.ReadAsStringAsync(cancellationToken);
            if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            {
                throw new ResponseDeserializationException("Int64");
            }

            return result;
        }

        private async Task<int> ReadSearchIdentifierAsync(HttpContent content, CancellationToken cancellationToken)
        {
            var payload = await GetJsonAsync<SearchStartResult>(content, cancellationToken);
            if (payload.Id is null)
            {
                throw new ResponseDeserializationException("Int32");
            }

            return payload.Id.Value;
        }
    }
}
