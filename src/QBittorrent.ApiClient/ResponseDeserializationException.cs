namespace QBittorrent.ApiClient
{
    internal sealed class ResponseDeserializationException : Exception
    {
        public ResponseDeserializationException(string targetTypeName)
            : base($"Unable to deserialize response as {targetTypeName}")
        {
        }
    }
}
