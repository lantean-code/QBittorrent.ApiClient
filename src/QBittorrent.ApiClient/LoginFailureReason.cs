namespace QBittorrent.ApiClient
{
    /// <summary>
    /// Identifies the action-specific reason a login request failed.
    /// </summary>
    public enum LoginFailureReason
    {
        /// <summary>
        /// The supplied credentials were invalid.
        /// </summary>
        InvalidCredentials,

        /// <summary>
        /// The client IP has been banned after repeated failed login attempts.
        /// </summary>
        BannedClient
    }
}
