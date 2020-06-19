namespace Settings
{
    public static class TargetFrameRateSettings
    {
        public const int Highest = 60;

        public const int Lowest = 20;

        public static int Average
        {
            get
            {
                return (Lowest + Highest) / 2;
            }
        }
    }
}
