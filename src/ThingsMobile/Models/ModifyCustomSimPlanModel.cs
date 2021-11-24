namespace ThingsMobile.Models;

/// <summary>
/// Details required to modify a custom sim plan
/// </summary>
public class ModifyCustomSimPlanModel : AddCustomSimPlanModel
{
    /// <summary>
    /// Unique identifier for the custom sim plan
    /// </summary>
    public string? Id { get; set; }
}
