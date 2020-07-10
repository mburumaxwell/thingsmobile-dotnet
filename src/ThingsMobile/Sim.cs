using System;
using System.Xml.Serialization;

namespace ThingsMobile
{
    /// <summary>
    /// Represents a SIM card
    /// </summary>
    [Serializable]
    public class Sim
    {
        /// <summary>
        /// Call detail records
        /// </summary>
        [XmlArray("cdrs")]
        [XmlArrayItem("cdr", typeof(CallDetailRecord))]
        public CallDetailRecord[] CallDetailRecords { get; set; }

        /// <summary>
        /// Date when the SIM card was activated
        /// </summary>
        [XmlElement("activationDate")]
        public string ActivationDateString { get; set; }

        /// <summary>
        /// Date when the SIM card was activated
        /// </summary>
        [XmlIgnore]
        public DateTime? ActivationDate => !string.IsNullOrWhiteSpace(ActivationDateString) ? DateTime.Parse(ActivationDateString) : (DateTime?)null;

        /// <summary>
        /// Balance left on the SIM card
        /// </summary>
        [XmlElement("balance")]
        public double Balance { get; set; }

        /// <summary>
        /// Indicates whether a SIM should be blocked after expiration date
        /// </summary>
        [XmlElement("blockSimAfterExpirationDate")]
        public bool BlockSimAfterExpirationDate { get; set; }

        /// <summary>
        /// Indicates if a sim is blocked daily
        /// </summary>
        [XmlElement("blockSimDaily")]
        public bool BlockSimDaily { get; set; }

        /// <summary>
        /// Indicates if a sim is blocked monthly
        /// </summary>
        [XmlElement("blockSimMonthly")]
        public bool BlockSimMonthly { get; set; }

        /// <summary>
        /// Indicates if a sim is blocked
        /// </summary>
        [XmlElement("blockSimTotal")]
        public bool BlockSimTotal { get; set; }

        /// <summary>
        /// Daily traffic in bytes
        /// </summary>
        [XmlElement("dailyTraffic")]
        public int DailyTraffic { get; set; }

        /// <summary>
        /// Daily traffic threshold in bytes
        /// </summary>
        [XmlElement("dailyTrafficThreshold")]
        public int DailyTrafficThreshold { get; set; }

        /// <summary>
        /// Expiration date for the SIM
        /// </summary>
        [XmlElement("expirationDate")]
        public string ExpirationDateString { get; set; }

        /// <summary>
        /// Expiration date for the SIM
        /// </summary>
        [XmlIgnore]
        public DateTime? ExpirationDate => !string.IsNullOrWhiteSpace(ExpirationDateString) ? (DateTime?)DateTime.Parse(ExpirationDateString) : null;

        /// <summary>
        /// Date when the SIM card was last connected     
        /// </summary>
        [XmlElement("lastConnectionDate")]
        public string LastConnectionDateString { get; set; }

        /// <summary>
        /// Date when the SIM card was last connected     
        /// </summary>
        [XmlIgnore]
        public DateTime? LastConnectionDate => !string.IsNullOrWhiteSpace(LastConnectionDateString) ? (DateTime?)DateTime.Parse(LastConnectionDateString) : null;

        /// <summary>
        /// The barcode number for the sim card
        /// </summary>
        [XmlElement("iccid")]
        public string ICCID { get; set; }

        /// <summary>
        /// Monthly traffic in bytes
        /// </summary>
        [XmlElement("monthlyTraffic")]
        public int MonthlyTraffic { get; set; }

        /// <summary>
        /// Monthly traffic threshold in bytes
        /// </summary>
        [XmlElement("monthlyTrafficThreshold")]
        public int MonthlyTrafficThreshold { get; set; }

        /// <summary>
        /// MSISDN of the SIM card
        /// </summary>
        [XmlElement("msisdn")]
        public string Msisdn { get; set; }

        /// <summary>
        /// Unique name used to identify the sim
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// The unique identifier for the plan the sim is associated with
        /// </summary>
        [XmlElement("plan")]
        public string Plan { get; set; }

        /// <summary>
        /// Whether the device is active or inactive
        /// </summary>
        [XmlElement("status")]
        public string StatusString { get; set; }

        /// <summary>
        /// Whether the device is active or inactive
        /// </summary>
        [XmlIgnore]
        public SimStatus Status => string.Equals("active", StatusString) ? SimStatus.Active : SimStatus.Inactive;

        /// <summary>
        /// A grouping tag for the sim
        /// </summary>
        [XmlElement("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Total traffic in bytes
        /// </summary>
        [XmlElement("totalTraffic")]
        public int TotalTraffic { get; set; }

        /// <summary>
        /// Total traffic threshold in bytes
        /// </summary>
        [XmlElement("totalTrafficThreshold")]
        public int TotalTrafficThreshold { get; set; }
    }
}
