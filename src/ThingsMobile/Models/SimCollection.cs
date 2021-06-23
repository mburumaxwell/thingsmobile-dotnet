using System;
using System.Xml.Serialization;

namespace ThingsMobile.Models
{
    /// <summary>
    /// A collection of SIMs
    /// </summary>
    [Serializable]
    [XmlRoot("result")]
    public class SimCollection : BaseResponseModel
    {
        /// <summary>
        /// A collection of SIMs
        /// </summary>
        [XmlArray("sims")]
        [XmlArrayItem("sim", typeof(Sim))]
        public Sim[]? Sims { get; set; }
    }
}
