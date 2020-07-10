using System;
using System.Xml.Serialization;

namespace ThingsMobile
{
    /// <summary>
    /// Successful response
    /// </summary>
    [Serializable]
    [XmlRoot("result")]
    public class BaseThingsMobileResponse
    {
        /// <summary>
        /// Boolean indicating success of the request
        /// </summary>
        [XmlElement("done")]
        public bool IsSuccess { get; set; }
    }
}
