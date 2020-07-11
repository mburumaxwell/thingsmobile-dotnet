using System;
using System.Xml.Serialization;

namespace ThingsMobile.Models
{
    /// <summary>
    /// Details for a sim plan
    /// </summary>
    [Serializable]
    public class SimPlan
    {
        /// <summary>
        /// Unique identifier for the sim plan
        /// </summary>
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// Name of the plan
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Indicates if auto recharge is enabled
        /// </summary>
        [XmlElement("simAutorechargeEnabled")]
        public bool AutorechargeEnabled { get; set; }

        /// <summary>
        /// The threshold for credit recharge
        /// </summary>
        [XmlElement("simAutorechargeCreditThreshold")]
        public double AutorechargeCreditThreshold { get; set; }

        /// <summary>
        /// Autorecharge amount
        /// </summary>
        [XmlElement("simAutorechargeAmount")]
        public double AutorechargeAmount { get; set; }
    }
}
