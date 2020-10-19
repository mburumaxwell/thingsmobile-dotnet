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
        /// The base URL for the ThingsMobile APIs
        /// </summary>
        public Uri BaseUrl { get; set; } = new Uri("https://api.thingsmobile.com/");
    }
}
