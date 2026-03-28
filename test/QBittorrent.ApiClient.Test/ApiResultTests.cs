using AwesomeAssertions;

namespace QBittorrent.ApiClient.Test
{
    public sealed class ApiResultTests
    {
        [Fact]
        public void GIVEN_FailedApiResult_WHEN_TryGetFailure_THEN_ShouldReturnFailure()
        {
            var expectedFailure = new ApiFailure
            {
                Kind = ApiFailureKind.ServerError,
                Operation = "Operation",
                UserMessage = "UserMessage",
            };

            var target = ApiResult.FailureResult(expectedFailure);

            var success = target.TryGetFailure(out var failure);

            success.Should().BeTrue();
            failure.Should().BeSameAs(expectedFailure);
        }

        [Fact]
        public void GIVEN_SuccessfulApiResult_WHEN_TryGetFailure_THEN_ShouldReturnFalse()
        {
            var target = ApiResult.Success();

            var success = target.TryGetFailure(out var failure);

            success.Should().BeFalse();
            failure.Should().BeNull();
        }

        [Fact]
        public void GIVEN_FailedGenericApiResult_WHEN_TryGetFailure_THEN_ShouldReturnFailure()
        {
            var expectedFailure = new ApiFailure
            {
                Kind = ApiFailureKind.ServerError,
                Operation = "Operation",
                UserMessage = "UserMessage",
            };

            var target = ApiResult<int>.FailureResult(expectedFailure);

            var success = target.TryGetFailure(out var failure);

            success.Should().BeTrue();
            failure.Should().BeSameAs(expectedFailure);
        }

        [Fact]
        public void GIVEN_SuccessfulGenericApiResult_WHEN_TryGetFailure_THEN_ShouldReturnFalse()
        {
            var target = ApiResult<int>.Success(42);

            var success = target.TryGetFailure(out var failure);

            success.Should().BeFalse();
            failure.Should().BeNull();
        }
    }
}
