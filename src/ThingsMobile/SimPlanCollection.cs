using System;
using System.Xml.Serialization;

namespace ThingsMobile
{
    /// <summary>
    /// Sim plans
    /// </summary>
    [Serializable]
    [XmlRoot("result")]
    public class SimPlanCollection : BaseThingsMobileResponse
    {
        /// <summary>
        /// Sim plans
        /// </summary>
        [XmlArray("plans")]
        [XmlArrayItem("plan", typeof(SimPlan))]
        public SimPlan[] SimPlans { get; set; }
    }
}
