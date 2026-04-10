using System.Net;
using AwesomeAssertions;

namespace QBittorrent.ApiClient.Test
{
    internal static class ApiResultTestExtensions
    {
        internal static ApiResult ShouldSucceed(this ApiResult result)
        {
            result.IsSuccess.Should().BeTrue();
            result.Failure.Should().BeNull();
            return result;
        }

        internal static ApiResult<T> ShouldSucceed<T>(this ApiResult<T> result)
        {
            result.IsSuccess.Should().BeTrue();
            result.Failure.Should().BeNull();
            return result;
        }

        internal static T GetValueOrThrow<T>(this ApiResult<T> result)
        {
            result.IsSuccess.Should().BeTrue();
            result.Failure.Should().BeNull();
            return result.Value!;
        }

        internal static ApiFailure GetFailureOrThrow(this ApiResult result)
        {
            result.IsSuccess.Should().BeFalse();
            result.Failure.Should().NotBeNull();
            return result.Failure!;
        }

        internal static ApiFailure GetFailureOrThrow<T>(this ApiResult<T> result)
        {
            result.IsSuccess.Should().BeFalse();
            result.Failure.Should().NotBeNull();
            return result.Failure!;
        }

        internal static ApiFailure ShouldFailWith(
            this ApiResult result,
            ApiFailureKind? kind = null,
            HttpStatusCode? statusCode = null,
            string? userMessage = null)
        {
            var failure = result.GetFailureOrThrow();
            AssertFailure(failure, kind, statusCode, userMessage);
            return failure;
        }

        internal static ApiFailure ShouldFailWith<T>(
            this ApiResult<T> result,
            ApiFailureKind? kind = null,
            HttpStatusCode? statusCode = null,
            string? userMessage = null)
        {
            var failure = result.GetFailureOrThrow();
            AssertFailure(failure, kind, statusCode, userMessage);
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
