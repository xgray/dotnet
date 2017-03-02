
namespace Bench
{
    using System;

    public static partial class DateTimeUtils
    {
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTimeMillis(this DateTime value)
        {
            return (long)(value - Jan1st1970).TotalMilliseconds;
        }

        public static DateTime FromUnixTimeMillis(long epoch)
        {
            return Jan1st1970.AddMilliseconds(epoch);
        }
    }
}
