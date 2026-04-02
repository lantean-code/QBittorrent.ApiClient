namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies which libtorrent disk I/O backend qBittorrent should use.
    /// </summary>
    public enum DiskIoType
    {
        /// <summary>
        /// Uses libtorrent's default disk I/O implementation.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Uses memory-mapped files.
        /// </summary>
        MemoryMappedFiles = 1,

        /// <summary>
        /// Uses the POSIX-compliant disk I/O backend.
        /// </summary>
        PosixCompliant = 2,

        /// <summary>
        /// Uses the simple pread/pwrite backend.
        /// </summary>
        SimplePreadPwrite = 3
    }
}
