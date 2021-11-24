using System.Xml.Serialization;

namespace ThingsMobile.Models;

/// <summary>
/// Basic response
/// </summary>
[Serializable]
[XmlRoot("result")]
public class BasicResponse : BaseResponseModel
{
    ///
    [XmlElement("message")]
    public string? Message { get; set; }
}
