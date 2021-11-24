using System.Xml.Serialization;

namespace ThingsMobile.Models
{
    /// <summary>
    /// The base response model
    /// </summary>
    [Serializable]
    [XmlRoot("result")]
    public class BaseResponseModel
    {
        /// <summary>
        /// Boolean indicating success of the request
        /// </summary>
        [XmlElement("done")]
        public bool IsSuccess { get; set; }
    }
}
