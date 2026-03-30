using AwesomeAssertions;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Test.Models
{
    public sealed class TorrentSelectorTests
    {
        [Fact]
        public void GIVEN_AllTorrents_WHEN_CalledMultipleTimes_THEN_ShouldReturnCachedSelector()
        {
            var first = TorrentSelector.AllTorrents();
            var second = TorrentSelector.AllTorrents();

            first.Should().BeSameAs(second);
            first.All.Should().BeTrue();
            first.Hashes.Should().BeNull();
        }

        [Fact]
        public void GIVEN_Hash_WHEN_FromHash_THEN_ShouldCreateSingleHashSelector()
        {
            var result = TorrentSelector.FromHash("hash");

            result.All.Should().BeFalse();
            result.Hashes.Should().Equal("hash");
        }

        [Fact]
        public void GIVEN_Hashes_WHEN_FromHashes_THEN_ShouldCreateMultiHashSelector()
        {
            var result = TorrentSelector.FromHashes(["hash1", "hash2"]);

            result.All.Should().BeFalse();
            result.Hashes.Should().Equal("hash1", "hash2");
        }

        [Fact]
        public void GIVEN_NullHash_WHEN_FromHash_THEN_ShouldThrowArgumentException()
        {
            var action = () => TorrentSelector.FromHash(null!);

            var exception = action.Should().Throw<ArgumentNullException>();
            exception.Which.ParamName.Should().Be("hash");
        }

        [Fact]
        public void GIVEN_WhitespaceHash_WHEN_FromHash_THEN_ShouldThrowArgumentException()
        {
            var action = () => TorrentSelector.FromHash(" ");

            var exception = action.Should().Throw<ArgumentException>();
            exception.Which.ParamName.Should().Be("hash");
        }

        [Fact]
        public void GIVEN_NullHashes_WHEN_FromHashes_THEN_ShouldThrowArgumentNullException()
        {
            var action = () => TorrentSelector.FromHashes(null!);

            var exception = action.Should().Throw<ArgumentNullException>();
            exception.Which.ParamName.Should().Be("hashes");
        }

        [Fact]
        public void GIVEN_EmptyHashes_WHEN_FromHashes_THEN_ShouldThrowArgumentException()
        {
            var action = () => TorrentSelector.FromHashes([]);

            var exception = action.Should().Throw<ArgumentException>();
            exception.Which.ParamName.Should().Be("hashes");
        }

        [Fact]
        public void GIVEN_HashesContainingWhitespace_WHEN_FromHashes_THEN_ShouldThrowArgumentException()
        {
            var action = () => TorrentSelector.FromHashes(["hash", " "]);

            var exception = action.Should().Throw<ArgumentException>();
            exception.Which.ParamName.Should().Be("hash");
        }
    }
}
