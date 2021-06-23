namespace ThingsMobile.Models
{
    /// <summary>
    /// A class containing details required to set sim thresholds
    /// </summary>
    public class SimThreshold
    {
        /// <summary>
        /// Msisdn for the sim card
        /// </summary>
        public string? MSISDN { get; set; }

        /// <summary>
        /// Daily traffic threshold in MB
        /// </summary>
        public double DailyLimit { get; set; }

        /// <summary>
        /// Indicates whether to block the sim when the threshold is reached/exceeded
        /// </summary>
        public bool BlockSimDaily { get; set; }

        /// <summary>
        /// Monthly traffic threshold in MB
        /// </summary>
        public double MonthlyLimit { get; set; }

        /// <summary>
        /// Indicates whether to block the sim after the monthly traffic threshold is reached
        /// </summary>
        public bool BlockSimMonthly { get; set; }

        /// <summary>
        /// Total traffic threshold in MB
        /// </summary>
        public double TotalLimit { get; set; }

        /// <summary>
        /// Indicates whether to block the sim when the total threshold is reached
        /// </summary>
        public bool BlockSimTotal { get; set; }
    }
}
