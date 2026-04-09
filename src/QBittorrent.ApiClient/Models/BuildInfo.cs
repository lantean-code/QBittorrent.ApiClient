using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents qBittorrent build and dependency version information.
    /// </summary>
    public record BuildInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildInfo" /> class.
        /// </summary>
        [JsonConstructor]
        public BuildInfo(
            string qTVersion,
            string libTorrentVersion,
            string boostVersion,
            string openSSLVersion,
            string zlibVersion,
            int bitness,
            BuildPlatform platform)
        {
            QTVersion = qTVersion;
            LibTorrentVersion = libTorrentVersion;
            BoostVersion = boostVersion;
            OpenSSLVersion = openSSLVersion;
            ZLibVersion = zlibVersion;
            Bitness = bitness;
            Platform = platform;
        }

        /// <summary>
        /// Gets the qt version.
        /// </summary>
        [JsonPropertyName("qt")]
        public string QTVersion { get; }

        /// <summary>
        /// Gets the lib torrent version.
        /// </summary>
        [JsonPropertyName("libtorrent")]
        public string LibTorrentVersion { get; }

        /// <summary>
        /// Gets the boost version.
        /// </summary>
        [JsonPropertyName("boost")]
        public string BoostVersion { get; }

        /// <summary>
        /// Gets the open ssl version.
        /// </summary>
        [JsonPropertyName("openssl")]
        public string OpenSSLVersion { get; }

        /// <summary>
        /// Gets the z lib version.
        /// </summary>
        [JsonPropertyName("zlib")]
        public string ZLibVersion { get; }

        /// <summary>
        /// Gets the bitness.
        /// </summary>
        [JsonPropertyName("bitness")]
        public int Bitness { get; }

        /// <summary>
        /// Gets the platform qBittorrent was built for.
        /// </summary>
        [JsonPropertyName("platform")]
        public BuildPlatform Platform { get; }
    }
}
