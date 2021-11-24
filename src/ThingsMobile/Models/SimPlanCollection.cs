﻿using System.Xml.Serialization;

namespace ThingsMobile.Models;

/// <summary>
/// Sim plans
/// </summary>
[Serializable]
[XmlRoot("result")]
public class SimPlanCollection : BaseResponseModel
{
    /// <summary>
    /// Sim plans
    /// </summary>
    [XmlArray("plans")]
    [XmlArrayItem("plan", typeof(SimPlan))]
    public SimPlan[]? SimPlans { get; set; }
}
