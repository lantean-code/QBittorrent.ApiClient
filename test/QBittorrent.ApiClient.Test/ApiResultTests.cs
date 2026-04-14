using AwesomeAssertions;

namespace QBittorrent.ApiClient.Test
{
    public sealed class ApiResultTests
    {
        private sealed class DerivedApiResult : ApiResult
        {
            public DerivedApiResult(ApiResultStatus status)
                : base(status)
            {
            }

            public DerivedApiResult(ApiResultStatus status, ApiFailure failure)
                : base(status, failure)
            {
            }
        }

        [Fact]
        public void GIVEN_FailedApiResult_WHEN_TryGetFailure_THEN_ShouldReturnFailure()
        {
            var expectedFailure = new ApiFailure
            {
                Kind = ApiFailureKind.ServerError,
                Operation = "Operation",
                UserMessage = "UserMessage",
            };

            var target = ApiResult.CreateFailure(expectedFailure);

            var success = target.TryGetFailure(out var failure);

            target.Status.Should().Be(ApiResultStatus.Failure);
            target.IsSuccess.Should().BeFalse();
            target.IsPending.Should().BeFalse();
            target.IsFailure.Should().BeTrue();
            success.Should().BeTrue();
            failure.Should().BeSameAs(expectedFailure);
        }

        [Fact]
        public void GIVEN_SuccessfulApiResult_WHEN_TryGetFailure_THEN_ShouldReturnFalse()
        {
            var target = ApiResult.CreateSuccess();

            var success = target.TryGetFailure(out var failure);

            target.Status.Should().Be(ApiResultStatus.Success);
            target.IsSuccess.Should().BeTrue();
            target.IsPending.Should().BeFalse();
            target.IsFailure.Should().BeFalse();
            success.Should().BeFalse();
            failure.Should().BeNull();
        }

        [Fact]
        public void GIVEN_PendingApiResult_WHEN_TryGetFailure_THEN_ShouldReturnFalse()
        {
            var target = ApiResult.CreatePending();

            var success = target.TryGetFailure(out var failure);

            target.Status.Should().Be(ApiResultStatus.Pending);
            target.IsSuccess.Should().BeFalse();
            target.IsPending.Should().BeTrue();
            target.IsFailure.Should().BeFalse();
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

            var target = ApiResult.CreateFailure<int>(expectedFailure);

            var success = target.TryGetFailure(out var failure);

            target.Status.Should().Be(ApiResultStatus.Failure);
            target.IsFailure.Should().BeTrue();
            success.Should().BeTrue();
            failure.Should().BeSameAs(expectedFailure);
        }

        [Fact]
        public void GIVEN_SuccessfulGenericApiResult_WHEN_TryGetFailure_THEN_ShouldReturnFalse()
        {
            var target = ApiResult.CreateSuccess(42);

            var success = target.TryGetFailure(out var failure);

            target.Status.Should().Be(ApiResultStatus.Success);
            target.IsSuccess.Should().BeTrue();
            success.Should().BeFalse();
            failure.Should().BeNull();
        }

        [Fact]
        public void GIVEN_PendingGenericApiResultWithValue_WHEN_TryGetPendingValue_THEN_ShouldReturnPendingValue()
        {
            var target = ApiResult.CreatePending(42);

            var success = target.TryGetPendingValue(out var value);

            target.Status.Should().Be(ApiResultStatus.Pending);
            target.IsPending.Should().BeTrue();
            success.Should().BeTrue();
            value.Should().Be(42);
        }

        [Fact]
        public void GIVEN_PendingGenericApiResult_WHEN_TryGetValue_THEN_ShouldReturnFalse()
        {
            var target = ApiResult.CreatePending(42);

            var success = target.TryGetValue(out var value);

            success.Should().BeFalse();
            value.Should().Be(default(int));
        }

        [Fact]
        public void GIVEN_SuccessfulGenericApiResult_WHEN_TryGetPendingValue_THEN_ShouldReturnFalse()
        {
            var target = ApiResult.CreateSuccess(42);

            var success = target.TryGetPendingValue(out var value);

            success.Should().BeFalse();
            value.Should().Be(default(int));
        }

        [Fact]
        public void GIVEN_Value_WHEN_CallingNonGenericSuccessOverload_THEN_ShouldReturnSuccessfulGenericResult()
        {
            var target = ApiResult.CreateSuccess(42);

            var success = target.TryGetValue(out var value);

            target.Status.Should().Be(ApiResultStatus.Success);
            success.Should().BeTrue();
            value.Should().Be(42);
            target.Failure.Should().BeNull();
        }

        [Fact]
        public void GIVEN_SuccessfulDualPayloadApiResult_WHEN_TryGetSuccessValue_THEN_ShouldReturnSuccessValue()
        {
            var target = ApiResult.CreateSuccess<string, int>("Value");

            var success = target.TryGetValue(out var value);

            target.Status.Should().Be(ApiResultStatus.Success);
            target.IsSuccess.Should().BeTrue();
            success.Should().BeTrue();
            value.Should().Be("Value");
        }

        [Fact]
        public void GIVEN_PendingDualPayloadApiResult_WHEN_TryGetPendingValue_THEN_ShouldReturnPendingValue()
        {
            var target = ApiResult.CreatePending<string, int>(42);

            var success = target.TryGetPendingValue(out var value);

            target.Status.Should().Be(ApiResultStatus.Pending);
            target.IsPending.Should().BeTrue();
            success.Should().BeTrue();
            value.Should().Be(42);
        }

        [Fact]
        public void GIVEN_PendingDualPayloadApiResult_WHEN_TryGetSuccessValue_THEN_ShouldReturnFalse()
        {
            var target = ApiResult.CreatePending<string, int>(42);

            var success = target.TryGetValue(out var value);

            success.Should().BeFalse();
            value.Should().BeNull();
        }

        [Fact]
        public void GIVEN_SuccessfulDualPayloadApiResult_WHEN_TryGetPendingValue_THEN_ShouldReturnFalse()
        {
            var target = ApiResult.CreateSuccess<string, int>("Value");

            var success = target.TryGetPendingValue(out var value);

            success.Should().BeFalse();
            value.Should().Be(default(int));
        }

        [Fact]
        public void GIVEN_NullFailure_WHEN_CreatingFailedDerivedResult_THEN_ShouldThrowArgumentNullException()
        {
            var action = () => new DerivedApiResult(ApiResultStatus.Failure, null!);

            action.Should().Throw<ArgumentNullException>()
                .WithParameterName("failure");
        }

        [Fact]
        public void GIVEN_NonFailureStatus_WHEN_CreatingFailedDerivedResult_THEN_ShouldThrowArgumentException()
        {
            var failure = new ApiFailure
            {
                Kind = ApiFailureKind.ServerError,
                Operation = "Operation",
                UserMessage = "UserMessage",
            };

            var action = () => new DerivedApiResult(ApiResultStatus.Success, failure);

            action.Should().Throw<ArgumentException>()
                .WithParameterName("status")
                .WithMessage("Failure details can only be provided for failed results.*");
        }

        [Fact]
        public void GIVEN_FailureStatus_WHEN_CreatingNonFailedDerivedResult_THEN_ShouldThrowArgumentException()
        {
            var action = () => new DerivedApiResult(ApiResultStatus.Failure);

            action.Should().Throw<ArgumentException>()
                .WithParameterName("status")
                .WithMessage("Failed results require failure details.*");
        }
    }
}
