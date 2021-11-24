﻿using System.Xml.Serialization;

namespace ThingsMobile.Models;

/// <summary>
/// Call detail record (CDR)
/// </summary>
[Serializable]
public class CallDetailRecord
{
    /// <summary>
    /// Country where the record was made
    /// </summary>
    [XmlElement(ElementName = "cdrCountry")]
    public string? Country { get; set; }

    /// <summary>
    /// Time when the call started
    /// </summary>
    [XmlElement(ElementName = "cdrDateStart")]
    public string? StartDateString { get; set; }

    /// <summary>
    /// Time when the call started
    /// </summary>
    [XmlIgnore]
    public DateTime? StartDate => string.IsNullOrWhiteSpace(StartDateString) ? null : DateTime.Parse(StartDateString);

    /// <summary>
    /// Time when the call ended
    /// </summary>
    [XmlElement(ElementName = "cdrDateStop")]
    public string? EndDateString { get; set; }

    /// <summary>
    /// Time when the call ended
    /// </summary>
    [XmlIgnore]
    public DateTime? EndDate => string.IsNullOrWhiteSpace(EndDateString) ? null : DateTime.Parse(EndDateString);

    /// <summary>
    /// International mobile subscriber identity used to identify the user of a cellular network
    /// </summary>
    [XmlElement(ElementName = "cdrImsi")]
    public string? Imsi { get; set; }

    /// <summary>
    /// MSISDN of the user
    /// </summary>
    [XmlElement(ElementName = "cdrMsisdn")]
    public string? Msisdn { get; set; }

    /// <summary>
    /// Network used
    /// </summary>
    [XmlElement(ElementName = "cdrNetwork")]
    public string? Network { get; set; }

    /// <summary>
    /// Operator used
    /// </summary>
    [XmlElement(ElementName = "cdrOperator")]
    public string? Operator { get; set; }

    /// <summary>
    /// Traffic in bytes
    /// </summary>
    [XmlElement(ElementName = "cdrTraffic")]
    public double Traffic { get; set; }
}
