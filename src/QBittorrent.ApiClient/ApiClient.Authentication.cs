using System.Net;

namespace QBittorrent.ApiClient
{
    internal partial class ApiClient
    {
        public Task<ApiResult<bool>> CheckAuthStateAsync(CancellationToken cancellationToken = default)
        {
            static async Task<ApiResult<bool>> handleAuthStateResponse(HttpResponseMessage response, string operation, CancellationToken currentCancellationToken)
            {
                using (response)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return ApiResult.CreateSuccess(true);
                    }

                    if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                    {
                        return ApiResult.CreateSuccess(false);
                    }

                    var failure = await TryCreateFailureAsync(operation, response, currentCancellationToken);
                    return failure!.ToResult<bool>();
                }
            }

            return ExecuteAsync(
                ct => _httpClient.GetAsync("app/version", ct),
                handleAuthStateResponse,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedBuilder()
                .Add("username", username)
                .Add("password", password)
                .ToFormUrlEncodedContent();

            static async Task<ApiResult> handleLoginResponse(HttpResponseMessage response, string operation, CancellationToken currentCancellationToken)
            {
                using (response)
                {
                    var failure = await TryCreateFailureAsync(operation, response, currentCancellationToken, createLoginFailure);
                    if (failure is not null)
                    {
                        return failure.ToResult();
                    }

                    var responseContent = await response.Content.ReadAsStringAsync(currentCancellationToken);
                    if (responseContent == "Fails.")
                    {
                        return createLoginFailure(HttpStatusCode.BadRequest, responseContent)!.ToResult();
                    }

                    return ApiResult.CreateSuccess();
                }

                ApiFailure? createLoginFailure(HttpStatusCode statusCode, string? responseBody)
                {
                    return statusCode switch
                    {
                        HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized => new ApiFailure
                        {
                            Kind = ApiFailureKind.AuthenticationRejected,
                            Operation = operation,
                            StatusCode = statusCode,
                            UserMessage = "Invalid username or password.",
                            Detail = responseBody,
                            Reason = LoginFailureReason.InvalidCredentials,
                            ResponseBody = responseBody,
                        },
                        HttpStatusCode.Forbidden => new ApiFailure
                        {
                            Kind = ApiFailureKind.AccessDenied,
                            Operation = operation,
                            StatusCode = statusCode,
                            UserMessage = responseBody ?? "The client has been temporarily banned from logging in.",
                            Detail = responseBody,
                            Reason = LoginFailureReason.BannedClient,
                            ResponseBody = responseBody,
                        },
                        _ => null
                    };
                }
            }

            return ExecuteAsync(
                ct => _httpClient.PostAsync("auth/login", content, ct),
                handleLoginResponse,
                cancellationToken: cancellationToken);
        }

        public Task<ApiResult> LogoutAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(ct => _httpClient.PostAsync("auth/logout", null, ct), cancellationToken: cancellationToken);
        }
    }
}
