using System.Xml.Serialization;

namespace ThingsMobile.Models;

/// <summary>
/// Recharge history for the account
/// </summary>
[Serializable]
public class RechargeHistory
{
    /// <summary>
    /// Amount recharged
    /// </summary>
    [XmlElement("amount")]
    public double Amount { get; set; }

    /// <summary>
    /// Date when the recharge was done
    /// </summary>
    [XmlElement("dateLoad")]
    public string? LoadingDateString { get; set; }

    /// <summary>
    /// Date when the recharge was done
    /// </summary>
    [XmlIgnore]
    public DateTime? LoadingDate => string.IsNullOrWhiteSpace(LoadingDateString) ? null : DateTime.Parse(LoadingDateString);

    /// <summary>
    /// MSISDN for the sim card
    /// </summary>
    [XmlElement("msisdn")]
    public string? MSISDN { get; set; }

    /// <summary>
    /// Description of the recharge
    /// </summary>
    [XmlElement("opDescription")]
    public string? Description { get; set; }
}
