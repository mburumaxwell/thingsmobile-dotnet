using System.Xml.Serialization;

namespace ThingsMobile.Models;

/// <summary>
/// A collection of CDRs
/// </summary>
[Serializable]
[XmlRoot("result")]
public class CdrPaginated : BaseResponseModel
{
    /// <summary>
    /// Call detail records
    /// </summary>
    [XmlArray("cdrsPaginated")]
    [XmlArrayItem("cdrPaginated", typeof(CallDetailRecord))]
    public CallDetailRecord[]? CallDetailRecords { get; set; }
}