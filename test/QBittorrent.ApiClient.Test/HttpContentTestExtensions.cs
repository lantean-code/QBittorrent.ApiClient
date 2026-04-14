using System.Net.Http;

namespace QBittorrent.ApiClient.Test
{
    internal static class HttpContentTestExtensions
    {
        public static async Task<string?> ReadAsStringOrNullAsync(this HttpContent? content, CancellationToken cancellationToken)
        {
            if (content is null)
            {
                return null;
            }

            return await content.ReadAsStringAsync(cancellationToken);
        }

        public static async Task<string?> ReadAsUnescapedStringOrNullAsync(this HttpContent? content, CancellationToken cancellationToken)
        {
            var value = await content.ReadAsStringOrNullAsync(cancellationToken);
            if (value is null)
            {
                return null;
            }

            return Uri.UnescapeDataString(value);
        }
    }
}
