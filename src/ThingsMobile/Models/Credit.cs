using System;
using System.Xml.Serialization;

namespace ThingsMobile.Models
{
    /// <summary>
    /// Details of the remaining credit and how it has been recharged
    /// </summary>
    [Serializable]
    [XmlRoot("result")]
    public class Credit : BaseThingsMobileResponse
    {
        /// <summary>
        /// The recharge history
        /// </summary>
        [XmlArray("creditHistory")]
        [XmlArrayItem("historyRow", typeof(RechargeHistory))]
        public RechargeHistory[] History { get; set; }

        /// <summary>
        /// Credit left
        /// </summary>
        [XmlElement("amount")]
        public double Amount { get; set; }

        /// <summary>
        /// The currency kind (e.g USD, EUR etc)
        /// </summary>
        [XmlElement("currency")]
        public string Currency { get; set; }
    }
}
