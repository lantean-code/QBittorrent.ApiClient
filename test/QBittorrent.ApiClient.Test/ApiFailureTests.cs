using AwesomeAssertions;

namespace QBittorrent.ApiClient.Test
{
    public sealed class ApiFailureTests
    {
        [Fact]
        public void GIVEN_MatchingReasonType_WHEN_TryGetReason_THEN_ShouldReturnTypedReason()
        {
            var target = new ApiFailure
            {
                Kind = ApiFailureKind.AuthenticationRejected,
                Operation = "Operation",
                UserMessage = "UserMessage",
                Reason = LoginFailureReason.InvalidCredentials,
            };

            var success = target.TryGetReason<LoginFailureReason>(out var reason);

            success.Should().BeTrue();
            reason.Should().Be(LoginFailureReason.InvalidCredentials);
        }

        [Fact]
        public void GIVEN_NonMatchingReasonType_WHEN_TryGetReason_THEN_ShouldReturnFalse()
        {
            var target = new ApiFailure
            {
                Kind = ApiFailureKind.AuthenticationRejected,
                Operation = "Operation",
                UserMessage = "UserMessage",
                Reason = LoginFailureReason.InvalidCredentials,
            };

            var success = target.TryGetReason<SearchFailureReason>(out var reason);

            success.Should().BeFalse();
            reason.Should().BeNull();
        }

        [Fact]
        public void GIVEN_Failure_WHEN_ToResult_THEN_ShouldReturnFailedNonGenericResult()
        {
            var target = new ApiFailure
            {
                Kind = ApiFailureKind.AuthenticationRejected,
                Operation = "Operation",
                UserMessage = "UserMessage",
            };

            var result = target.ToResult();

            result.IsSuccess.Should().BeFalse();
            result.Failure.Should().BeSameAs(target);
        }

        [Fact]
        public void GIVEN_Failure_WHEN_ToGenericResult_THEN_ShouldReturnFailedGenericResult()
        {
            var target = new ApiFailure
            {
                Kind = ApiFailureKind.AuthenticationRejected,
                Operation = "Operation",
                UserMessage = "UserMessage",
            };

            var result = target.ToResult<int>();

            result.IsSuccess.Should().BeFalse();
            result.Failure.Should().BeSameAs(target);
        }
    }
}
