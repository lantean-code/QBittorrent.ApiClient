namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a qBittorrent save-location selection.
    /// </summary>
    public class SaveLocation
    {
        /// <summary>
        /// Gets or sets a value indicating whether the save location is the watched folder.
        /// </summary>
        public bool IsWatchedFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the save location is the default folder.
        /// </summary>
        public bool IsDefaultFolder { get; set; }

        /// <summary>
        /// Gets or sets the save path.
        /// </summary>
        public string? SavePath { get; set; }

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
                return new SaveLocation
                {
                    IsWatchedFolder = true
                };
            }
            else if (value == 1)
            {
                return new SaveLocation
                {
                    IsDefaultFolder = true
                };
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

            if (value == "0")
            {
                return new SaveLocation
                {
                    IsWatchedFolder = true
                };
            }
            else if (value == "1")
            {
                return new SaveLocation
                {
                    IsDefaultFolder = true
                };
            }
            else
            {
                return new SaveLocation
                {
                    SavePath = value
                };
            }
        }

        /// <summary>
        /// Converts the current save-location state into the qBittorrent Web API value.
        /// </summary>
        /// <returns>The serialized save-location value.</returns>
        public object ToValue()
        {
            if (IsWatchedFolder)
            {
                return 0;
            }
            else if (IsDefaultFolder)
            {
                return 1;
            }
            else if (SavePath is not null)
            {
                return SavePath;
            }

            throw new InvalidOperationException("Invalid value.");
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
