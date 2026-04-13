namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a qBittorrent torrent selection.
    /// </summary>
    public sealed record TorrentSelector
    {
        private static readonly TorrentSelector _allTorrents = new(true, null);

        /// <summary>
        /// Gets a value indicating whether all torrents are selected.
        /// </summary>
        public bool All { get; }

        /// <summary>
        /// Gets the selected torrent hashes.
        /// </summary>
        public IReadOnlyList<string>? Hashes { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TorrentSelector" /> class.
        /// </summary>
        /// <param name="all">Whether all torrents are selected.</param>
        /// <param name="hashes">The selected torrent hashes.</param>
        private TorrentSelector(bool all, IReadOnlyList<string>? hashes)
        {
            All = all;
            Hashes = hashes;
        }

        /// <summary>
        /// Creates a selector for all torrents.
        /// </summary>
        /// <returns>A selector that targets all torrents.</returns>
        public static TorrentSelector AllTorrents()
        {
            return _allTorrents;
        }

        /// <summary>
        /// Creates a selector for a single torrent.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <returns>A selector that targets the specified torrent.</returns>
        public static TorrentSelector FromHash(string hash)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(hash);

            return new TorrentSelector(false, [hash]);
        }

        /// <summary>
        /// Creates a selector for one or more torrents.
        /// </summary>
        /// <param name="hashes">The torrent hashes.</param>
        /// <returns>A selector that targets the specified torrents.</returns>
        public static TorrentSelector FromHashes(IEnumerable<string> hashes)
        {
            ArgumentNullException.ThrowIfNull(hashes);

            var normalizedHashes = hashes.Select(NormalizeHash).ToArray();
            if (normalizedHashes.Length == 0)
            {
                throw new ArgumentException("Specify at least one torrent hash.", nameof(hashes));
            }

            return new TorrentSelector(false, normalizedHashes);
        }

        /// <inheritdoc />
        public bool Equals(TorrentSelector? other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other is null)
            {
                return false;
            }

            if (All != other.All)
            {
                return false;
            }

            if (Hashes is null && other.Hashes is null)
            {
                return true;
            }

            if (Hashes is null || other.Hashes is null)
            {
                return false;
            }

            return Hashes.SequenceEqual(other.Hashes, StringComparer.Ordinal);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(All);

            if (Hashes is not null)
            {
                foreach (var hash in Hashes)
                {
                    hashCode.Add(hash, StringComparer.Ordinal);
                }
            }

            return hashCode.ToHashCode();
        }

        private static string NormalizeHash(string hash)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(hash);
            return hash;
        }
    }
}
