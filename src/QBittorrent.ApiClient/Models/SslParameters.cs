using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents SSL parameters associated with a torrent.
    /// </summary>
    public record SslParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SslParameters" /> class.
        /// </summary>
        [JsonConstructor]
        public SslParameters(string? certificate, string? privateKey, string? dhParams)
        {
            Certificate = certificate;
            PrivateKey = privateKey;
            DhParams = dhParams;
        }

        /// <summary>
        /// Gets the certificate.
        /// </summary>
        [JsonPropertyName("ssl_certificate")]
        public string? Certificate { get; }

        /// <summary>
        /// Gets the private key.
        /// </summary>
        [JsonPropertyName("ssl_private_key")]
        public string? PrivateKey { get; }

        /// <summary>
        /// Gets the DH params.
        /// </summary>
        [JsonPropertyName("ssl_dh_params")]
        public string? DhParams { get; }
    }
}
