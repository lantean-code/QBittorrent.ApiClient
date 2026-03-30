using AwesomeAssertions;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Test.Models
{
    public class SaveLocationTests
    {
        [Fact]
        public void GIVEN_IntegerZero_WHEN_Create_THEN_ShouldReturnWatchedFolder()
        {
            var result = SaveLocation.Create(0);

            result.Kind.Should().Be(SaveLocationKind.WatchedFolder);
            result.SavePath.Should().BeNull();
        }

        [Fact]
        public void GIVEN_IntegerOne_WHEN_Create_THEN_ShouldReturnDefaultFolder()
        {
            var result = SaveLocation.Create(1);

            result.Kind.Should().Be(SaveLocationKind.DefaultFolder);
            result.SavePath.Should().BeNull();
        }

        [Fact]
        public void GIVEN_ObjectIntegerZero_WHEN_Create_THEN_ShouldReturnWatchedFolder()
        {
            var result = SaveLocation.Create((object)0);

            result.Kind.Should().Be(SaveLocationKind.WatchedFolder);
            result.SavePath.Should().BeNull();
        }

        [Fact]
        public void GIVEN_ObjectStringOne_WHEN_Create_THEN_ShouldReturnDefaultFolder()
        {
            var result = SaveLocation.Create((object)"1");

            result.Kind.Should().Be(SaveLocationKind.DefaultFolder);
            result.SavePath.Should().BeNull();
        }

        [Fact]
        public void GIVEN_StringPath_WHEN_Create_THEN_ShouldReturnCustomPath()
        {
            var result = SaveLocation.Create("/downloads");

            result.Kind.Should().Be(SaveLocationKind.CustomPath);
            result.SavePath.Should().Be("/downloads");
        }

        [Fact]
        public void GIVEN_WatchedFolderInputs_WHEN_Create_THEN_ShouldReturnCachedInstance()
        {
            var integerResult = SaveLocation.Create(0);
            var stringResult = SaveLocation.Create("0");

            ReferenceEquals(integerResult, stringResult).Should().BeTrue();
        }

        [Fact]
        public void GIVEN_DefaultFolderInputs_WHEN_Create_THEN_ShouldReturnCachedInstance()
        {
            var integerResult = SaveLocation.Create(1);
            var stringResult = SaveLocation.Create("1");

            ReferenceEquals(integerResult, stringResult).Should().BeTrue();
        }

        [Fact]
        public void GIVEN_EquivalentCustomPaths_WHEN_Comparing_THEN_ShouldBeEqual()
        {
            var first = SaveLocation.Create("/downloads");
            var second = SaveLocation.Create("/downloads");

            first.Should().Be(second);
        }

        [Fact]
        public void GIVEN_DifferentKinds_WHEN_Comparing_THEN_ShouldNotBeEqual()
        {
            var watchedFolder = SaveLocation.Create(0);
            var customPath = SaveLocation.Create("/downloads");

            watchedFolder.Should().NotBe(customPath);
        }

        [Fact]
        public void GIVEN_UnsupportedObject_WHEN_Create_THEN_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => SaveLocation.Create((object)true);

            var exception = act.Should().Throw<ArgumentOutOfRangeException>();
            exception.Which.ParamName.Should().Be("value");
        }

        [Fact]
        public void GIVEN_InvalidInteger_WHEN_Create_THEN_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => SaveLocation.Create(2);

            var exception = act.Should().Throw<ArgumentOutOfRangeException>();
            exception.Which.ParamName.Should().Be("value");
        }

        [Fact]
        public void GIVEN_NullString_WHEN_Create_THEN_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => SaveLocation.Create((string?)null);

            var exception = act.Should().Throw<ArgumentOutOfRangeException>();
            exception.Which.ParamName.Should().Be("value");
        }

        [Fact]
        public void GIVEN_WhitespaceString_WHEN_Create_THEN_ShouldThrowArgumentException()
        {
            var act = () => SaveLocation.Create(" ");

            var exception = act.Should().Throw<ArgumentException>();
            exception.Which.ParamName.Should().Be("value");
        }

        [Fact]
        public void GIVEN_WatchedFolder_WHEN_ToValue_THEN_ShouldReturnZero()
        {
            var result = SaveLocation.Create(0).ToValue();

            result.Should().Be(0);
        }

        [Fact]
        public void GIVEN_DefaultFolder_WHEN_ToValue_THEN_ShouldReturnOne()
        {
            var result = SaveLocation.Create(1).ToValue();

            result.Should().Be(1);
        }

        [Fact]
        public void GIVEN_CustomPath_WHEN_ToValue_THEN_ShouldReturnPath()
        {
            var result = SaveLocation.Create("/data").ToValue();

            result.Should().Be("/data");
        }

        [Fact]
        public void GIVEN_WatchedFolder_WHEN_ToString_THEN_ShouldReturnZero()
        {
            var result = SaveLocation.Create(0).ToString();

            result.Should().Be("0");
        }

        [Fact]
        public void GIVEN_DefaultFolder_WHEN_ToString_THEN_ShouldReturnOne()
        {
            var result = SaveLocation.Create(1).ToString();

            result.Should().Be("1");
        }

        [Fact]
        public void GIVEN_CustomPath_WHEN_ToString_THEN_ShouldReturnPath()
        {
            var result = SaveLocation.Create("/downloads").ToString();

            result.Should().Be("/downloads");
        }
    }
}
