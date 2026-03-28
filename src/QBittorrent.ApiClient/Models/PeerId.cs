namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a peer host and port pair.
    /// </summary>
    public readonly struct PeerId(string host, int port)
    {
        /// <summary>
        /// Gets the host.
        /// </summary>
        public string Host { get; } = host;

        /// <summary>
        /// Gets the port.
        /// </summary>
        public int Port { get; } = port;

        /// <summary>
        /// Returns the peer identifier in <c>host:port</c> format.
        /// </summary>
        /// <returns>The formatted peer identifier.</returns>
        public override string ToString()
        {
            return $"{Host}:{Port}";
        }
    }
}
