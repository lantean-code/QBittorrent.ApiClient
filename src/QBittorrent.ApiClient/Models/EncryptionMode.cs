namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies how qBittorrent should handle encrypted peer connections.
    /// </summary>
    public enum EncryptionMode
    {
        /// <summary>
        /// Allows both encrypted and unencrypted peer connections.
        /// </summary>
        AllowEncryption = 0,

        /// <summary>
        /// Requires encrypted peer connections.
        /// </summary>
        RequireEncryption = 1,

        /// <summary>
        /// Disables encrypted peer connections.
        /// </summary>
        DisableEncryption = 2
    }
}
