using System.Text.Json;
using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents the nested <c>info</c> section of torrent metadata returned by qBittorrent.
    /// </summary>
    public record TorrentMetadataInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TorrentMetadataInfo" /> class.
        /// </summary>
        /// <param name="name">The torrent name.</param>
        /// <param name="length">The total torrent length in bytes.</param>
        /// <param name="pieceLength">The piece length in bytes.</param>
        /// <param name="piecesNum">The number of pieces.</param>
        /// <param name="private">Whether the torrent is private.</param>
        /// <param name="files">The files declared by the torrent metadata.</param>
        [JsonConstructor]
        public TorrentMetadataInfo(
            string? name,
            long length,
            int pieceLength,
            int piecesNum,
            bool @private,
            IReadOnlyList<TorrentMetadataFile>? files)
        {
            Name = name ?? throw new JsonException("The torrent metadata info payload did not include a valid name.");
            Length = length;
            PieceLength = pieceLength;
            PiecesNum = piecesNum;
            Private = @private;
            Files = files ?? [];
        }

        /// <summary>
        /// Gets the torrent name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        /// Gets the total torrent length in bytes.
        /// </summary>
        [JsonPropertyName("length")]
        public long Length { get; }

        /// <summary>
        /// Gets the piece length in bytes.
        /// </summary>
        [JsonPropertyName("piece_length")]
        public int PieceLength { get; }

        /// <summary>
        /// Gets the number of pieces.
        /// </summary>
        [JsonPropertyName("pieces_num")]
        public int PiecesNum { get; }

        /// <summary>
        /// Gets a value indicating whether the torrent is private.
        /// </summary>
        [JsonPropertyName("private")]
        public bool Private { get; }

        /// <summary>
        /// Gets the files declared by the torrent metadata.
        /// </summary>
        [JsonPropertyName("files")]
        public IReadOnlyList<TorrentMetadataFile> Files { get; }
    }
}
