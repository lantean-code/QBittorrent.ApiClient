using System.Net;
using AwesomeAssertions;

namespace QBittorrent.ApiClient.Test
{
    internal static class ApiResultTestExtensions
    {
        internal static ApiResult ShouldSucceed(this ApiResult result)
        {
            result.Status.Should().Be(ApiResultStatus.Success);
            result.IsSuccess.Should().BeTrue();
            result.IsPending.Should().BeFalse();
            result.IsFailure.Should().BeFalse();
            result.Failure.Should().BeNull();
            return result;
        }

        internal static ApiResult<T> ShouldSucceed<T>(this ApiResult<T> result)
            where T : notnull
        {
            result.Status.Should().Be(ApiResultStatus.Success);
            result.IsSuccess.Should().BeTrue();
            result.IsPending.Should().BeFalse();
            result.IsFailure.Should().BeFalse();
            result.Failure.Should().BeNull();
            return result;
        }

        internal static ApiResult<TSuccess, TPending> ShouldSucceed<TSuccess, TPending>(this ApiResult<TSuccess, TPending> result)
            where TSuccess : notnull
            where TPending : notnull
        {
            result.Status.Should().Be(ApiResultStatus.Success);
            result.IsSuccess.Should().BeTrue();
            result.IsPending.Should().BeFalse();
            result.IsFailure.Should().BeFalse();
            result.Failure.Should().BeNull();
            return result;
        }

        internal static ApiResult ShouldBePending(this ApiResult result)
        {
            result.Status.Should().Be(ApiResultStatus.Pending);
            result.IsSuccess.Should().BeFalse();
            result.IsPending.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            result.Failure.Should().BeNull();
            return result;
        }

        internal static ApiResult<T> ShouldBePending<T>(this ApiResult<T> result)
            where T : notnull
        {
            result.Status.Should().Be(ApiResultStatus.Pending);
            result.IsSuccess.Should().BeFalse();
            result.IsPending.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            result.Failure.Should().BeNull();
            return result;
        }

        internal static ApiResult<TSuccess, TPending> ShouldBePending<TSuccess, TPending>(this ApiResult<TSuccess, TPending> result)
            where TSuccess : notnull
            where TPending : notnull
        {
            result.Status.Should().Be(ApiResultStatus.Pending);
            result.IsSuccess.Should().BeFalse();
            result.IsPending.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            result.Failure.Should().BeNull();
            return result;
        }

        internal static T GetValueOrThrow<T>(this ApiResult<T> result)
            where T : notnull
        {
            result.ShouldSucceed();
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException("Expected a successful result.");
            }

            return result.Value;
        }

        internal static T GetPendingValueOrThrow<T>(this ApiResult<T> result)
            where T : notnull
        {
            result.ShouldBePending();
            if (!result.IsPending)
            {
                throw new InvalidOperationException("Expected a pending result.");
            }

            return result.PendingValue;
        }

        internal static TSuccess GetSuccessValueOrThrow<TSuccess, TPending>(this ApiResult<TSuccess, TPending> result)
            where TSuccess : notnull
            where TPending : notnull
        {
            result.ShouldSucceed();
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException("Expected a successful result.");
            }

            return result.Value;
        }

        internal static TPending GetPendingValueOrThrow<TSuccess, TPending>(this ApiResult<TSuccess, TPending> result)
            where TSuccess : notnull
            where TPending : notnull
        {
            result.ShouldBePending();
            if (!result.IsPending)
            {
                throw new InvalidOperationException("Expected a pending result.");
            }

            return result.PendingValue;
        }

        internal static ApiFailure ShouldFailWith(
            this ApiResult result,
            ApiFailureKind? kind = null,
            HttpStatusCode? statusCode = null,
            string? userMessage = null)
        {
            result.Status.Should().Be(ApiResultStatus.Failure);
            result.IsSuccess.Should().BeFalse();
            result.IsPending.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Failure.Should().NotBeNull();

            var failure = result.Failure;
            AssertFailure(failure, kind, statusCode, userMessage);
            return failure;
        }

        internal static ApiFailure ShouldFailWith<T>(
            this ApiResult<T> result,
            ApiFailureKind? kind = null,
            HttpStatusCode? statusCode = null,
            string? userMessage = null)
            where T : notnull
        {
            result.Status.Should().Be(ApiResultStatus.Failure);
            result.IsFailure.Should().BeTrue();
            result.Failure.Should().NotBeNull();

            var failure = result.Failure;
            AssertFailure(failure, kind, statusCode, userMessage);
            return failure;
        }

        internal static ApiFailure ShouldFailWith<TSuccess, TPending>(
            this ApiResult<TSuccess, TPending> result,
            ApiFailureKind? kind = null,
            HttpStatusCode? statusCode = null,
            string? userMessage = null)
            where TSuccess : notnull
            where TPending : notnull
        {
            result.Status.Should().Be(ApiResultStatus.Failure);
            result.IsFailure.Should().BeTrue();
            result.Failure.Should().NotBeNull();

            var failure = result.Failure;
            AssertFailure(failure, kind, statusCode, userMessage);
            return failure;
        }

        internal static async Task ShouldThrowUninitializedCompatibilityExceptionAsync(this Func<Task> action)
        {
            var exception = await action.Should().ThrowAsync<InvalidOperationException>();
            exception.Which.Message.Should().Be("ApiClient.InitializeAsync or ApiClient.Initialize must complete successfully before using compatibility-gated operations.");
        }

        internal static ApiFailure ShouldFailWithCompatibilityInitializationFailure(
            this ApiResult result,
            string detail,
            string? responseBody)
        {
            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "Unable to determine the qBittorrent Web API version.");

            failure.StatusCode.Should().BeNull();
            failure.Operation.Should().Be(nameof(ApiClient.InitializeAsync));
            failure.Detail.Should().Be(detail);
            failure.ResponseBody.Should().Be(responseBody);
            return failure;
        }

        private static void AssertFailure(
            ApiFailure failure,
            ApiFailureKind? kind,
            HttpStatusCode? statusCode,
            string? userMessage)
        {
            if (kind is not null)
            {
                failure.Kind.Should().Be(kind.Value);
            }

            if (statusCode is not null)
            {
                failure.StatusCode.Should().Be(statusCode.Value);
            }

            if (userMessage is not null)
            {
                failure.UserMessage.Should().Be(userMessage);
            }
        }
    }
}
