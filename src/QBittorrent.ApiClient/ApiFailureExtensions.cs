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
            return ApiResult.FailureResult(failure);
        }

        /// <summary>
        /// Converts an API failure into a generic failed result.
        /// </summary>
        /// <typeparam name="T">The result value type.</typeparam>
        /// <param name="failure">The failure to convert.</param>
        /// <returns>The failed result.</returns>
        public static ApiResult<T> ToResult<T>(this ApiFailure failure)
        {
            return ApiResult<T>.FailureResult(failure);
        }
    }
}
