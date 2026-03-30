namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a qBittorrent save-location selection.
    /// </summary>
    public sealed record SaveLocation
    {
        private static readonly SaveLocation _watchedFolder = new SaveLocation(SaveLocationKind.WatchedFolder, null);
        private static readonly SaveLocation _defaultFolder = new SaveLocation(SaveLocationKind.DefaultFolder, null);

        /// <summary>
        /// Gets the save-location kind.
        /// </summary>
        public SaveLocationKind Kind { get; }

        /// <summary>
        /// Gets the save path.
        /// </summary>
        public string? SavePath { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveLocation" /> class.
        /// </summary>
        /// <param name="kind">The save-location kind.</param>
        /// <param name="savePath">The explicit save path.</param>
        private SaveLocation(SaveLocationKind kind, string? savePath)
        {
            Kind = kind;
            SavePath = savePath;
        }

        /// <summary>
        /// Creates a <see cref="SaveLocation" /> from a qBittorrent save-location value.
        /// </summary>
        /// <param name="value">The qBittorrent save-location value.</param>
        /// <returns>The parsed save-location value.</returns>
        public static SaveLocation Create(object? value)
        {
            if (value is int intValue)
            {
                return Create(intValue);
            }
            else if (value is string stringValue)
            {
                return Create(stringValue);
            }

            throw new ArgumentOutOfRangeException(nameof(value));
        }

        /// <summary>
        /// Creates a <see cref="SaveLocation" /> from an integer qBittorrent save-location value.
        /// </summary>
        /// <param name="value">The qBittorrent save-location value.</param>
        /// <returns>The parsed save-location value.</returns>
        public static SaveLocation Create(int value)
        {
            if (value == 0)
            {
                return _watchedFolder;
            }
            else if (value == 1)
            {
                return _defaultFolder;
            }

            throw new ArgumentOutOfRangeException(nameof(value));
        }

        /// <summary>
        /// Creates a <see cref="SaveLocation" /> from a string qBittorrent save-location value.
        /// </summary>
        /// <param name="value">The qBittorrent save-location value.</param>
        /// <returns>The parsed save-location value.</returns>
        public static SaveLocation Create(string? value)
        {
            if (value is null)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("A custom save location must include a path.", nameof(value));
            }

            if (value == "0")
            {
                return _watchedFolder;
            }
            else if (value == "1")
            {
                return _defaultFolder;
            }
            else
            {
                return new SaveLocation(SaveLocationKind.CustomPath, value);
            }
        }

        /// <summary>
        /// Converts the current save-location state into the qBittorrent Web API value.
        /// </summary>
        /// <returns>The serialized save-location value.</returns>
        public object ToValue()
        {
            if (Kind == SaveLocationKind.WatchedFolder)
            {
                return 0;
            }

            if (Kind == SaveLocationKind.DefaultFolder)
            {
                return 1;
            }

            return SavePath!;
        }

        /// <summary>
        /// Returns the serialized qBittorrent save-location value as a string.
        /// </summary>
        /// <returns>The serialized save-location string.</returns>
        public override string? ToString()
        {
            return ToValue().ToString();
        }
    }
}
