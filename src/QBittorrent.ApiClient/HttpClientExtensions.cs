namespace QBittorrent.ApiClient
{
    internal static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostAsync(this HttpClient httpClient, string requestUrl, FormUrlEncodedBuilder builder, CancellationToken cancellationToken = default)
        {
            return httpClient.PostAsync(requestUrl, builder.ToFormUrlEncodedContent(), cancellationToken);
        }

        public static Task<HttpResponseMessage> GetAsync(this HttpClient httpClient, string requestUrl, QueryBuilder builder, CancellationToken cancellationToken = default)
        {
            return httpClient.GetAsync($"{requestUrl}{builder.ToQueryString()}", cancellationToken);
        }
    }
}
