using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a peer connected to a torrent.
    /// </summary>
    public record Peer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Peer" /> class.
        /// </summary>
        [JsonConstructor]
        public Peer(
            string? client,
            PeerConnectionType? connection,
            string? country,
            string? countryCode,
            int? downloadSpeed,
            long? downloaded,
            string? files,
            string? flags,
            string? flagsDescription,
            string? hostName,
            string? iPAddress,
            string? i2pDestination,
            string? clientId,
            int? port,
            double? progress,
            double? relevance,
            int? uploadSpeed,
            long? uploaded)
        {
            Client = client;
            Connection = connection;
            Country = country;
            CountryCode = countryCode;
            DownloadSpeed = downloadSpeed;
            Downloaded = downloaded;
            Files = files;
            Flags = flags;
            FlagsDescription = flagsDescription;
            HostName = hostName;
            IPAddress = iPAddress;
            I2pDestination = i2pDestination;
            ClientId = clientId;
            Port = port;
            Progress = progress;
            Relevance = relevance;
            UploadSpeed = uploadSpeed;
            Uploaded = uploaded;
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        [JsonPropertyName("client")]
        public string? Client { get; }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        [JsonPropertyName("connection")]
        public PeerConnectionType? Connection { get; }

        /// <summary>
        /// Gets the country.
        /// </summary>
        [JsonPropertyName("country")]
        public string? Country { get; }

        /// <summary>
        /// Gets the country code.
        /// </summary>
        [JsonPropertyName("country_code")]
        public string? CountryCode { get; }

        /// <summary>
        /// Gets the current download speed in bytes per second.
        /// </summary>
        [JsonPropertyName("dl_speed")]
        public int? DownloadSpeed { get; }

        /// <summary>
        /// Gets the downloaded byte count.
        /// </summary>
        [JsonPropertyName("downloaded")]
        public long? Downloaded { get; }

        /// <summary>
        /// Gets the files.
        /// </summary>
        [JsonPropertyName("files")]
        public string? Files { get; }

        /// <summary>
        /// Gets the flags.
        /// </summary>
        [JsonPropertyName("flags")]
        public string? Flags { get; }

        /// <summary>
        /// Gets the flags description.
        /// </summary>
        [JsonPropertyName("flags_desc")]
        public string? FlagsDescription { get; }

        /// <summary>
        /// Gets the resolved peer host name when available.
        /// </summary>
        [JsonPropertyName("host_name")]
        public string? HostName { get; }

        /// <summary>
        /// Gets the ip address.
        /// </summary>
        [JsonPropertyName("ip")]
        public string? IPAddress { get; }

        /// <summary>
        /// Gets the I2P destination.
        /// </summary>
        [JsonPropertyName("i2p_dest")]
        public string? I2pDestination { get; }

        /// <summary>
        /// Gets the client ID.
        /// </summary>
        [JsonPropertyName("peer_id_client")]
        public string? ClientId { get; }

        /// <summary>
        /// Gets the peer port number.
        /// </summary>
        [JsonPropertyName("port")]
        public int? Port { get; }

        /// <summary>
        /// Gets the peer completion fraction from 0.0 to 1.0.
        /// </summary>
        [JsonPropertyName("progress")]
        public double? Progress { get; }

        /// <summary>
        /// Gets the peer relevance fraction from 0.0 to 1.0.
        /// </summary>
        [JsonPropertyName("relevance")]
        public double? Relevance { get; }

        /// <summary>
        /// Gets the current upload speed in bytes per second.
        /// </summary>
        [JsonPropertyName("up_speed")]
        public int? UploadSpeed { get; }

        /// <summary>
        /// Gets the uploaded byte count.
        /// </summary>
        [JsonPropertyName("uploaded")]
        public long? Uploaded { get; }
    }
}
