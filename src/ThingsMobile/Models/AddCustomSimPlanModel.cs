namespace ThingsMobile.Models
{
    /// <summary>
    /// Details required to add a custom sim plan
    /// </summary>
    public class AddCustomSimPlanModel
    {
        /// <summary>
        /// Name of the sim plan
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Sim auto recharge status
        /// </summary>
        public bool EnableAutoRecharge { get; set; }

        /// <summary>
        /// Amouht to auto recharge the sim by (MB)
        /// </summary>
        public double AutoRechargeAmount { get; set; }

        /// <summary>
        /// The threshold to recharge the sim by (MB)
        /// </summary>
        public double AutoRechargeThreshold { get; set; }
    }
}
