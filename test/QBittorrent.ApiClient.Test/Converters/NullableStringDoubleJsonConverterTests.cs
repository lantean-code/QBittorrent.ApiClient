using System.Text;
using System.Text.Json;
using AwesomeAssertions;
using QBittorrent.ApiClient.Converters;

namespace QBittorrent.ApiClient.Test.Converters
{
    public sealed class NullableStringDoubleJsonConverterTests
    {
        private readonly NullableStringDoubleJsonConverter _target;
        private readonly JsonSerializerOptions _options;

        public NullableStringDoubleJsonConverterTests()
        {
            _target = new NullableStringDoubleJsonConverter();
            _options = new JsonSerializerOptions();
            _options.Converters.Add(_target);
        }

        [Fact]
        public void GIVEN_NullToken_WHEN_Read_THEN_ShouldReturnNull()
        {
            var value = JsonSerializer.Deserialize<double?>("null", _options);

            value.Should().BeNull();
        }

        [Fact]
        public void GIVEN_DirectNullTokenReader_WHEN_Read_THEN_ShouldReturnNull()
        {
            var reader = CreateReader("null");

            var value = _target.Read(ref reader, typeof(double?), _options);

            value.Should().BeNull();
        }

        [Fact]
        public void GIVEN_StringDash_WHEN_Read_THEN_ShouldReturnNull()
        {
            var value = JsonSerializer.Deserialize<double?>("\"-\"", _options);

            value.Should().BeNull();
        }

        [Fact]
        public void GIVEN_WhitespaceString_WHEN_Read_THEN_ShouldReturnNull()
        {
            var value = JsonSerializer.Deserialize<double?>("\"   \"", _options);

            value.Should().BeNull();
        }

        [Fact]
        public void GIVEN_InvalidString_WHEN_Read_THEN_ShouldReturnNull()
        {
            var value = JsonSerializer.Deserialize<double?>("\"invalid-value\"", _options);

            value.Should().BeNull();
        }

        [Fact]
        public void GIVEN_StringNumberWithThousands_WHEN_Read_THEN_ShouldReturnValue()
        {
            var value = JsonSerializer.Deserialize<double?>("\"1,234.5\"", _options);

            value.Should().Be(1234.5);
        }

        [Fact]
        public void GIVEN_NumberToken_WHEN_Read_THEN_ShouldReturnValue()
        {
            var value = JsonSerializer.Deserialize<double?>("123.5", _options);

            value.Should().Be(123.5);
        }

        [Fact]
        public void GIVEN_StringNumber_WHEN_Read_THEN_ShouldReturnValue()
        {
            var value = JsonSerializer.Deserialize<double?>("\"123.5\"", _options);

            value.Should().Be(123.5);
        }

        [Fact]
        public void GIVEN_PreciseStringNumber_WHEN_Read_THEN_ShouldPreserveDoublePrecision()
        {
            var value = JsonSerializer.Deserialize<double?>("\"1.23456789012345\"", _options);

            value.Should().Be(1.23456789012345);
        }

        [Fact]
        public void GIVEN_UnsupportedToken_WHEN_Read_THEN_ShouldReturnNull()
        {
            var value = JsonSerializer.Deserialize<double?>("true", _options);

            value.Should().BeNull();
        }

        [Fact]
        public void GIVEN_NullValue_WHEN_Write_THEN_ShouldEmitNull()
        {
            var json = Write(null);

            json.Should().Be("null");
        }

        [Fact]
        public void GIVEN_Value_WHEN_Write_THEN_ShouldEmitJsonNumber()
        {
            var json = Write(1.25);

            json.Should().Be("1.25");
        }

        private string Write(double? value)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new Utf8JsonWriter(memoryStream);
            _target.Write(writer, value, _options);
            writer.Flush();
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        private static Utf8JsonReader CreateReader(string json)
        {
            var bytes = Encoding.UTF8.GetBytes(json);
            var reader = new Utf8JsonReader(bytes);
            reader.Read();
            return reader;
        }
    }
}
