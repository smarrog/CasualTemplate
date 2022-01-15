namespace Smr.Extensions {
    public static class TimeExtensions {
        // TODO вынести это в TimeUtils с нормальными настройкаи
        public static string ToTimeShortFormat(this int seconds, bool hideIfZero = true) {
            if (seconds < 60) {
                if (hideIfZero) {
                    return $"{seconds}s";
                }
                return $"0m {seconds}s";
            }

            if (seconds < 86400) {
                var hours = seconds / 3600;
                var minutes = seconds % 3600 / 60;
                if (hideIfZero) {
                    if (minutes == 0) {
                        return $"{hours}h";
                    }
                    if (hours == 0) {
                        return $"{minutes}m";
                    }
                }
                return $"{hours}h {minutes}m";
            }

            var days = seconds / 86400;
            return $"{days}d";
        }
        
        public static string ToTime(this int seconds, Localization.Localization localization, bool hideIfZero = true) {
            switch (seconds) {
                case < 60 when hideIfZero:
                    return SecondsToString(seconds, localization);
                case < 60:
                    return $"{MinutesToString(0, localization)} {SecondsToString(seconds, localization)}";
                case < 86400: {
                    var hours = seconds / 3600;
                    var minutes = seconds % 3600 / 60;
                    if (hideIfZero) {
                        if (minutes == 0) {
                            return HoursToString(hours, localization);
                        }
                        if (hours == 0) {
                            return MinutesToString(minutes, localization);
                        }
                    }
                    return $"{HoursToString(hours, localization)} {MinutesToString(minutes, localization)}";
                }
            }

            var days = seconds / 86400;
            return DaysToString(days, localization);
        }
        
        
        public static string DaysToString(int value, Localization.Localization localization) {
            string ending;
            if (localization == Localization.Localization.Russian) {
                ending = (value % 100) switch {
                    1 => "день",
                    2 or 3 or 4 => "дня",
                    _ => "дней"
                };
            } else {
                ending = value switch {
                    1 => "day",
                    _ => "days"
                };
            }
            return $"{value} {ending}";
        }
        
        public static string HoursToString(int value, Localization.Localization localization) {
            string ending;
            if (localization == Localization.Localization.Russian) {
                ending = (value % 100) switch {
                    1 => "час",
                    2 or 3 or 4 => "часа",
                    _ => "часов"
                };
            } else {
                ending = value switch {
                    1 => "hour",
                    _ => "hours"
                };
            }
            return $"{value} {ending}";
        }
        
        public static string MinutesToString(int value, Localization.Localization localization) {
            string ending;
            if (localization == Localization.Localization.Russian) {
                ending = (value % 100) switch {
                    1 => "минута",
                    2 or 3 or 4 => "минуты",
                    _ => "минут"
                };
            } else {
                ending = value switch {
                    1 => "minute",
                    _ => "minutes"
                };
            }
            return $"{value} {ending}";
        }
        
        public static string SecondsToString(float value, Localization.Localization localization) {
            var intValue = (int)value;
            string ending;
            if (localization == Localization.Localization.Russian) {
                ending = (intValue % 100) switch {
                    1 => "секунда",
                    2 or 3 or 4 => "секунды",
                    _ => "секунд"
                };
            } else {
                ending = intValue switch {
                    1 => "second",
                    _ => "seconds"
                };
            }
            return $"{value:0.#} {ending}";
        }
    }
}