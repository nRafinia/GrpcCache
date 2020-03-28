namespace nCache.Client
{
    public enum CacheTime
    {
        /// <summary>
        /// 5 Minutes
        /// </summary>
        Tiny = 5,

        /// <summary>
        /// 30 Minutes
        /// </summary>
        Little = 30,

        /// <summary>
        /// 1 Hour
        /// </summary>
        VeryLow = 60,

        /// <summary>
        /// 2 Hours
        /// </summary>
        Low = 120,

        /// <summary>
        /// 4 Hours
        /// </summary>
        Normal = 240,

        /// <summary>
        /// 8 Hours
        /// </summary>
        High = 480,

        /// <summary>
        /// 12 Hours
        /// </summary>
        VeryHigh = 720,

        /// <summary>
        /// 24 Hours
        /// </summary>
        Day = 1440,

        /// <summary>
        /// 1 Minute
        /// </summary>
        Minute1 = 1
    }
}