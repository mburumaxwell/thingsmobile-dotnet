using System;
using System.Xml.Serialization;
using ThingsMobile.Models;

namespace ThingsMobile
{
    /// <summary>
    /// Error response from things mobile APIs
    /// </summary>
    [Serializable]
    [XmlRoot("result")]
    public class ThingsMobileErrorResponse : BaseThingsMobileResponse
    {
        /// <summary>
        /// Numeric error code on Things Mobile API
        /// </summary>
        [XmlElement("errorCode")]
        public string Code { get; set; }

        /// <summary>
        /// Error description on the Things Mobile API
        /// </summary>
        [XmlElement("errorMessage")]
        public string Description { get; set; }
    }
}
