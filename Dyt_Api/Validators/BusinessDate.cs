namespace Dyt_Api.Validators
{
    public static class BusinessDate
    {
        // Turkey uses UTC+3 all year. Keeps "today" from lagging behind local midnight.
        private static readonly TimeSpan BusinessOffset = TimeSpan.FromHours(3);

        public static DateOnly Today => FromUtc(DateTime.UtcNow);

        public static DateOnly FromUtc(DateTime utcNow)
        {
            return DateOnly.FromDateTime(utcNow + BusinessOffset);
        }
    }
}
