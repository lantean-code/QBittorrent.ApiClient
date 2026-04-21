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
        public void GIVEN_AllApiResultVariants_WHEN_ViewedAsBase_THEN_ShouldShareCommonResultType()
        {
            ApiResultBase nonGenericResult = ApiResult.CreateSuccess();
            ApiResultBase singlePayloadResult = ApiResult.CreateSuccess(42);
            ApiResultBase dualPayloadResult = ApiResult.CreatePending<string, int>(42);

            nonGenericResult.Should().BeOfType<ApiResult>();
            singlePayloadResult.Should().BeOfType<ApiResult<int>>();
            dualPayloadResult.Should().BeOfType<ApiResult<string, int>>();
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
        public void GIVEN_FailedGenericApiResult_WHEN_TryGetValue_THEN_ShouldReturnFalse()
        {
            var failure = new ApiFailure
            {
                Kind = ApiFailureKind.ServerError,
                Operation = "Operation",
                UserMessage = "UserMessage",
            };

            var target = ApiResult.CreateFailure<int>(failure);

            var success = target.TryGetValue(out var value);

            target.Status.Should().Be(ApiResultStatus.Failure);
            target.IsFailure.Should().BeTrue();
            success.Should().BeFalse();
            value.Should().Be(default(int));
        }

        [Fact]
        public void GIVEN_Value_WHEN_CallingNonGenericSuccessOverload_THEN_ShouldReturnSuccessfulGenericResult()
        {
            var target = ApiResult.CreateSuccess(42);

            var success = target.TryGetValue(out var value);

            target.Status.Should().Be(ApiResultStatus.Success);
            target.IsSuccess.Should().BeTrue();
            success.Should().BeTrue();
            value.Should().Be(42);
            target.Failure.Should().BeNull();
        }

        [Fact]
        public void GIVEN_NonFailureStatus_WHEN_CreatingFailedSinglePayloadResult_THEN_ShouldThrowArgumentException()
        {
            var failure = new ApiFailure
            {
                Kind = ApiFailureKind.ServerError,
                Operation = "Operation",
                UserMessage = "UserMessage",
            };

            var action = () => new ApiResult<int>(ApiResultStatus.Success, 42, failure);

            action.Should().Throw<ArgumentException>()
                .WithParameterName("status")
                .WithMessage("Failure details can only be provided for failed results.*");
        }

        [Fact]
        public void GIVEN_FailureStatus_WHEN_CreatingNonFailedSinglePayloadResult_THEN_ShouldThrowArgumentException()
        {
            var action = () => new ApiResult<int>(ApiResultStatus.Failure, 42);

            action.Should().Throw<ArgumentException>()
                .WithParameterName("status")
                .WithMessage("Failed results require failure details.*");
        }

        [Fact]
        public void GIVEN_PendingStatus_WHEN_CreatingSinglePayloadResult_THEN_ShouldThrowArgumentException()
        {
            var action = () => new ApiResult<int>(ApiResultStatus.Pending, 42);

            action.Should().Throw<ArgumentException>()
                .WithParameterName("status")
                .WithMessage("Value results only support successful or failed states.*");
        }

        [Fact]
        public void GIVEN_SuccessfulDualPayloadApiResult_WHEN_TryGetSuccessValue_THEN_ShouldReturnSuccessValue()
        {
            var target = ApiResult.CreateSuccess<string, int>("Value");

            var success = target.TryGetSuccessValue(out var value);

            target.Status.Should().Be(ApiResultStatus.Success);
            target.IsSuccess.Should().BeTrue();
            success.Should().BeTrue();
            value.Should().Be("Value");
        }

        [Fact]
        public void GIVEN_SuccessfulDualPayloadApiResult_WHEN_TryGetFailure_THEN_ShouldReturnFalse()
        {
            var target = ApiResult.CreateSuccess<string, int>("Value");

            var success = target.TryGetFailure(out var failure);

            target.Status.Should().Be(ApiResultStatus.Success);
            target.IsSuccess.Should().BeTrue();
            success.Should().BeFalse();
            failure.Should().BeNull();
        }

        [Fact]
        public void GIVEN_FailedDualPayloadApiResult_WHEN_TryGetFailure_THEN_ShouldReturnFailure()
        {
            var expectedFailure = new ApiFailure
            {
                Kind = ApiFailureKind.ServerError,
                Operation = "Operation",
                UserMessage = "UserMessage",
            };

            var target = ApiResult.CreateFailure<string, int>(expectedFailure);

            var success = target.TryGetFailure(out var failure);

            target.Status.Should().Be(ApiResultStatus.Failure);
            target.IsFailure.Should().BeTrue();
            success.Should().BeTrue();
            failure.Should().BeSameAs(expectedFailure);
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

            var success = target.TryGetSuccessValue(out var value);

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
        public void GIVEN_NonFailureStatus_WHEN_CreatingFailedDualPayloadResult_THEN_ShouldThrowArgumentException()
        {
            var failure = new ApiFailure
            {
                Kind = ApiFailureKind.ServerError,
                Operation = "Operation",
                UserMessage = "UserMessage",
            };

            var action = () => new ApiResult<string, int>(ApiResultStatus.Success, "Value", 42, failure);

            action.Should().Throw<ArgumentException>()
                .WithParameterName("status")
                .WithMessage("Failure details can only be provided for failed results.*");
        }

        [Fact]
        public void GIVEN_FailureStatus_WHEN_CreatingNonFailedDualPayloadResult_THEN_ShouldThrowArgumentException()
        {
            var action = () => new ApiResult<string, int>(ApiResultStatus.Failure, "Value", 42);

            action.Should().Throw<ArgumentException>()
                .WithParameterName("status")
                .WithMessage("Failed results require failure details.*");
        }

        [Fact]
        public void GIVEN_InvalidNonFailureStatus_WHEN_CreatingDualPayloadResult_THEN_ShouldThrowArgumentException()
        {
            var action = () => new ApiResult<string, int>((ApiResultStatus)123, "Value", default);

            action.Should().Throw<ArgumentException>()
                .WithParameterName("status")
                .WithMessage("Dual-payload results only support successful, pending, or failed states.*");
        }

        [Fact]
        public void GIVEN_NullFailure_WHEN_CreatingFailedDerivedResult_THEN_ShouldThrowArgumentNullException()
        {
            var action = () => new ApiResult(ApiResultStatus.Failure, null!);

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

            var action = () => new ApiResult(ApiResultStatus.Success, failure);

            action.Should().Throw<ArgumentException>()
                .WithParameterName("status")
                .WithMessage("Failure details can only be provided for failed results.*");
        }

        [Fact]
        public void GIVEN_FailureStatus_WHEN_CreatingNonFailedDerivedResult_THEN_ShouldThrowArgumentException()
        {
            var action = () => new ApiResult(ApiResultStatus.Failure);

            action.Should().Throw<ArgumentException>()
                .WithParameterName("status")
                .WithMessage("Failed results require failure details.*");
        }
    }
}
