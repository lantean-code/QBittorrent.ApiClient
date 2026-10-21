using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies the encryption used for SMTP notifications.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<SmtpEncryptionType>))]
    public enum SmtpEncryptionType
    {
        /// <summary>
        /// Uses an unencrypted SMTP connection.
        /// </summary>
        None = 0,

        /// <summary>
        /// Upgrades the SMTP connection by using STARTTLS.
        /// </summary>
        STARTTLS = 1,

        /// <summary>
        /// Uses SMTP over TLS from connection establishment.
        /// </summary>
        SMTPS = 2
    }
}
