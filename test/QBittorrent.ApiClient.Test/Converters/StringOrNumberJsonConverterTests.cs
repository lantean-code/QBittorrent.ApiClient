using System.Text;
using System.Text.Json;
using AwesomeAssertions;
using QBittorrent.ApiClient.Converters;

namespace QBittorrent.ApiClient.Test.Converters
{
    public sealed class StringOrNumberJsonConverterTests
    {
        private readonly JsonSerializerOptions _options;
        private readonly StringOrNumberJsonConverter _target;

        public StringOrNumberJsonConverterTests()
        {
            _target = new StringOrNumberJsonConverter();
            _options = new JsonSerializerOptions();
            _options.Converters.Add(_target);
        }

        [Theory]
        [InlineData("null", null)]
        [InlineData("\"value\"", "value")]
        [InlineData("123", "123")]
        public void GIVEN_SupportedToken_WHEN_Read_THEN_ShouldReturnStringValue(string json, string? expected)
        {
            var result = JsonSerializer.Deserialize<string?>(json, _options);

            result.Should().Be(expected);
        }

        [Fact]
        public void GIVEN_DirectNullToken_WHEN_Read_THEN_ShouldReturnNull()
        {
            var bytes = Encoding.UTF8.GetBytes("null");
            var reader = new Utf8JsonReader(bytes);
            reader.Read();

            var result = _target.Read(ref reader, typeof(string), _options);

            result.Should().BeNull();
        }

        [Theory]
        [InlineData("true")]
        [InlineData("1.5")]
        public void GIVEN_UnsupportedToken_WHEN_Read_THEN_ShouldThrowJsonException(string json)
        {
            var action = () => JsonSerializer.Deserialize<string?>(json, _options);

            action.Should().Throw<JsonException>();
        }

        [Theory]
        [InlineData(null, "null")]
        [InlineData("value", "\"value\"")]
        public void GIVEN_Value_WHEN_Write_THEN_ShouldWriteJsonString(string? value, string expected)
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            _target.Write(writer, value, _options);
            writer.Flush();

            Encoding.UTF8.GetString(stream.ToArray()).Should().Be(expected);
        }
    }
}
