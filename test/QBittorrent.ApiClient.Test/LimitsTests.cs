using AwesomeAssertions;

namespace QBittorrent.ApiClient.Test
{
    public sealed class LimitsTests
    {
        [Fact]
        public void GIVEN_ShareLimitConstants_WHEN_ReadingValues_THEN_ShouldMatchQbittorrentSentinels()
        {
            Limits.UseGlobalShareRatioLimit.Should().Be(-2);
            Limits.NoShareRatioLimit.Should().Be(-1);
            Limits.UseGlobalSeedingTimeLimit.Should().Be(-2);
            Limits.NoSeedingTimeLimit.Should().Be(-1);
            Limits.UseGlobalInactiveSeedingTimeLimit.Should().Be(-2);
            Limits.NoInactiveSeedingTimeLimit.Should().Be(-1);
        }

        [Fact]
        public void GIVEN_TransferRateLimitConstant_WHEN_ReadingValue_THEN_ShouldMatchQbittorrentSentinel()
        {
            Limits.NoTransferRateLimit.Should().Be(0);
        }
    }
}
