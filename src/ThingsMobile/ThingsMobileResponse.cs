using System.Net;

namespace ThingsMobile
{
    /// <summary>
    /// Things Mobile API Response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ThingsMobileResponse<T> where T : BaseThingsMobileResponse
    {
        /// <summary>
        /// The resource extracted from the response body
        /// </summary>
        public T Resource { get; set; }

        /// <summary>
        /// The error extracted from the response body
        /// </summary>
        public ThingsMobileErrorResponse ErrorResponse { get; set; }

        /// <summary>
        /// Status code response from the API
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Indicates whether a request has succeeded
        /// </summary>
        public bool IsSuccessful { get; set; }
    }
}
