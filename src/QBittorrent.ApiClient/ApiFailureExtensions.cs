namespace QBittorrent.ApiClient
{
    /// <summary>
    /// Provides helpers for converting API failures into result values.
    /// </summary>
    public static class ApiFailureExtensions
    {
        /// <summary>
        /// Converts an API failure into a non-generic failed result.
        /// </summary>
        /// <param name="failure">The failure to convert.</param>
        /// <returns>The failed result.</returns>
        public static ApiResult ToResult(this ApiFailure failure)
        {
            return ApiResult.CreateFailure(failure);
        }

        /// <summary>
        /// Converts an API failure into a generic failed result.
        /// </summary>
        /// <typeparam name="T">The result value type.</typeparam>
        /// <param name="failure">The failure to convert.</param>
        /// <returns>The failed result.</returns>
        public static ApiResult<T> ToResult<T>(this ApiFailure failure)
            where T : notnull
        {
            return ApiResult.CreateFailure<T>(failure);
        }

        /// <summary>
        /// Converts an API failure into a dual-payload failed result.
        /// </summary>
        /// <typeparam name="TSuccess">The success payload type.</typeparam>
        /// <typeparam name="TPending">The pending payload type.</typeparam>
        /// <param name="failure">The failure to convert.</param>
        /// <returns>The failed result.</returns>
        public static ApiResult<TSuccess, TPending> ToResult<TSuccess, TPending>(this ApiFailure failure)
            where TSuccess : notnull
            where TPending : notnull
        {
            return ApiResult.CreateFailure<TSuccess, TPending>(failure);
        }
    }
}
