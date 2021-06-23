using System;

namespace ThingsMobile
{
    /// <summary>
    /// Options for configuring the <see cref="ThingsMobileClient"/>
    /// </summary>
    public class ThingsMobileClientOptions
    {
        /// <summary>
        /// The username for the ThingsMobile account
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The token used to access the ThingsMobile APIs
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The endpoint to use for requests.
        /// Defaults to <c>https://api.thingsmobile.com/</c>.
        /// For test purposes, set this value to <c>https://test.thingsmobile.com/</c>
        /// </summary>
        public Uri Endpoint { get; set; } = new Uri("https://api.thingsmobile.com/");
    }
}
