using AwesomeAssertions;

namespace QBittorrent.ApiClient.Test
{
    public sealed class LimitsTests
    {
        [Fact]
        public void GIVEN_ShareLimitConstants_WHEN_ReadingValues_THEN_ShouldMatchQbittorrentSentinels()
        {
            Limits.UseGlobalShareLimit.Should().Be(-2);
            Limits.NoShareLimit.Should().Be(-1);
        }

        [Fact]
        public void GIVEN_TransferRateLimitConstant_WHEN_ReadingValue_THEN_ShouldMatchQbittorrentSentinel()
        {
            Limits.NoTransferRateLimit.Should().Be(0);
        }
    }
}
