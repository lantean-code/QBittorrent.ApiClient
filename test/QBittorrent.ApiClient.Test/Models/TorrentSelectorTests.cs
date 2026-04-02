using AwesomeAssertions;
using QBittorrent.ApiClient.Models;
using System.Runtime.CompilerServices;

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

        [Fact]
        public void GIVEN_SameInstance_WHEN_Equals_THEN_ShouldReturnTrue()
        {
            var target = TorrentSelector.FromHash("hash");

            var result = target.Equals(target);

            result.Should().BeTrue();
        }

        [Fact]
        public void GIVEN_NullOther_WHEN_Equals_THEN_ShouldReturnFalse()
        {
            var target = TorrentSelector.FromHash("hash");

            var result = target.Equals(null);

            result.Should().BeFalse();
        }

        [Fact]
        public void GIVEN_DifferentAllValues_WHEN_Equals_THEN_ShouldReturnFalse()
        {
            var target = TorrentSelector.AllTorrents();

            var result = target.Equals(TorrentSelector.FromHash("hash"));

            result.Should().BeFalse();
        }

        [Fact]
        public void GIVEN_DistinctAllSelectors_WHEN_Equals_THEN_ShouldReturnTrue()
        {
            var target = TorrentSelector.AllTorrents();
            var other = CreateTorrentSelector(all: true, hashes: null);

            ReferenceEquals(target, other).Should().BeFalse();

            var result = target.Equals(other);

            result.Should().BeTrue();
        }

        [Fact]
        public void GIVEN_SameAllButOnlyOneNullHashes_WHEN_Equals_THEN_ShouldReturnFalse()
        {
            var target = CreateTorrentSelector(all: false, hashes: null);

            var result = target.Equals(TorrentSelector.FromHash("hash"));

            result.Should().BeFalse();
        }

        [Fact]
        public void GIVEN_EquivalentHashes_WHEN_Equals_THEN_ShouldReturnTrue()
        {
            var target = TorrentSelector.FromHashes(["hash1", "hash2"]);
            var other = TorrentSelector.FromHashes(["hash1", "hash2"]);

            var result = target.Equals(other);

            result.Should().BeTrue();
        }

        [Fact]
        public void GIVEN_DifferentHashes_WHEN_Equals_THEN_ShouldReturnFalse()
        {
            var target = TorrentSelector.FromHashes(["hash1", "hash2"]);
            var other = TorrentSelector.FromHashes(["hash1", "hash3"]);

            var result = target.Equals(other);

            result.Should().BeFalse();
        }

        [Fact]
        public void GIVEN_EquivalentSelectors_WHEN_GetHashCode_THEN_ShouldReturnSameHashCode()
        {
            var left = TorrentSelector.FromHashes(["hash1", "hash2"]);
            var right = TorrentSelector.FromHashes(["hash1", "hash2"]);

            var leftHashCode = left.GetHashCode();
            var rightHashCode = right.GetHashCode();

            leftHashCode.Should().Be(rightHashCode);
        }

        [Fact]
        public void GIVEN_AllSelectors_WHEN_GetHashCode_THEN_ShouldReturnSameHashCode()
        {
            var left = TorrentSelector.AllTorrents();
            var right = CreateTorrentSelector(all: true, hashes: null);

            var leftHashCode = left.GetHashCode();
            var rightHashCode = right.GetHashCode();

            leftHashCode.Should().Be(rightHashCode);
        }

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        private static extern TorrentSelector CreateTorrentSelector(bool all, IReadOnlyList<string>? hashes);
    }
}
