namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies the download state of a torrent piece.
    /// </summary>
    public enum PieceState
    {
        /// <summary>
        /// Indicates that the piece has not been downloaded.
        /// </summary>
        NotDownloaded = 0,

        /// <summary>
        /// Indicates that the piece is currently being downloaded.
        /// </summary>
        Downloading = 1,

        /// <summary>
        /// Indicates that the piece has been downloaded.
        /// </summary>
        Downloaded = 2,
    }
}
