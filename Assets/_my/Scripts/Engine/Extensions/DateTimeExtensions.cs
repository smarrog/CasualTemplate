using System;

namespace Smr.Extensions {
    public static class DateTimeExtension {

        // Это есть в Wdk.Utils.TimeUtils, но если сослаться - получится перекрёстная ссылка
        public const long SECONDS_IN_DAY = 86400;

        public static int ToUnixDay(this long timestamp) {
            return (int) (timestamp / SECONDS_IN_DAY);
        }

        public static long ToUnixUtcTimeStamp(this DateTime dt) {
            return (long)(TimeZoneInfo.ConvertTimeToUtc(dt) - DateTime.UnixEpoch).TotalSeconds;
        }

        public static long ToUnixLocalTimeStamp(this DateTime dt) {
            return (long)(dt - DateTime.UnixEpoch).TotalSeconds;
        }

        public static DateTime UnixTimeStampToDateTime(this long timestamp) {
            return DateTime.UnixEpoch.AddSeconds(timestamp);
        }

        public static long GetTimeStampDateOnly(this long timestamp) {
            return timestamp / SECONDS_IN_DAY * SECONDS_IN_DAY;
        }
    }
}