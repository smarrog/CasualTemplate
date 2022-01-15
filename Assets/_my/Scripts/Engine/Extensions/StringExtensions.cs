using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;

namespace Smr.Extensions {
    public static class StringExtension {
        [ContractAnnotation("s:null => false")]
        public static bool IsFilled(this string s) => !string.IsNullOrEmpty(s);

        [ContractAnnotation("s:null => true")]
        public static bool IsEmpty(this string s) => string.IsNullOrEmpty(s);

        /// <summary>
        /// Calculates md5 digest of a string.
        /// </summary>
        /// <returns>The md5 digest.</returns>
        /// <param name="s">input string</param>
        public static string GetMd5(this string s) {
            if (string.IsNullOrEmpty(s)) {
                return string.Empty;
            }

            var data = new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(s));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++) {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public static string GetNotNull(this string s) {
            return s ?? String.Empty;
        }
        public static long GetHashCodeLong(this string s) {
            string s1 = s.Substring(0, s.Length / 2);
            string s2 = s.Substring(s.Length / 2);

            long result = ((long)s1.GetHashCode()) << 0x20 | (uint)s2.GetHashCode();
            return result;
        }

        public static string RemoveSpaces(this string s) {
            return string.IsNullOrEmpty(s) ? string.Empty : s.Replace("\t", "").Replace(" ", "");
        }

        public static string RemoveLinebreaks(this string s) {
            return string.IsNullOrEmpty(s) ? string.Empty : s.Replace("\n", "").Replace("\r", "");
        }

        public static string LastPathElement(this string s) {
            return s.Replace('\\', '/').Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
        }

        public static string ToAltPath(this string s) => s.Replace('\\', '/');
        public static string ToRegularPath(this string s) => s.Replace('/', '\\');

        public static (string left, string right) SplitByDot(this string s) {
            int dotIndex = s.IndexOf('.');
            if (dotIndex == -1) {
                return (s, string.Empty);
            } else {
                return (s[..dotIndex], s[(dotIndex + 1)..]);
            }
        }

        public static void AlphaNumericSort<T>(this List<T> keyedList, Func<T, string> keyGetter) {
            keyedList.Sort((o1, o2) => AlphaNumericComparer(keyGetter(o1), keyGetter(o2)));
        }

        public static void AlphaNumericSort(this List<string> stringsList) {
            stringsList.Sort(AlphaNumericComparer);
        }

        public static bool IsEqualToIgnoreCase(this string lhs, string rhs) {
            return string.Equals(lhs, rhs, StringComparison.CurrentCultureIgnoreCase);
        }

        public static int AlphaNumericComparer(string s1, string s2) {
            int idx = GetIndexOfDifferentChars(s1, s2);
            if (idx < 0) {
                return s1.Length.CompareTo(s2.Length);
            }

            if (s1[idx].IsNumber() && s2[idx].IsNumber()) {
                return CompareNumbersFromStrings(s1, s2, idx);
            }

            return string.Compare(s1, s2, StringComparison.InvariantCulture);
        }

        private static int GetIndexOfDifferentChars(string s1, string s2) {
            int minLen = Math.Min(s1.Length, s2.Length);

            for (int i = 0; i < minLen; i++) {
                if (s1[i] != s2[i]) {
                    return i;
                }
            }
            return -1;
        }

        private static int CompareNumbersFromStrings(string s1, string s2, int startIdx) {
            int numStart = startIdx;
            while (numStart > 0 && s1[numStart - 1].IsNumber()) {
                numStart--;
            }

            int num1End = startIdx;
            while (num1End < s1.Length - 1 && s1[num1End + 1].IsNumber()) {
                num1End++;
            }

            int num2End = startIdx;
            while (num2End < s2.Length - 1 && s2[num2End + 1].IsNumber()) {
                num2End++;
            }

            string numStr1 = s1.Substring(numStart, num1End - numStart + 1);
            string numStr2 = s2.Substring(numStart, num2End - numStart + 1);

            int.TryParse(numStr1, out int num1);
            int.TryParse(numStr2, out int num2);

            return num1.CompareTo(num2);
        }


        public static bool IsNumber(this char ch) => ch >= '0' && ch <= '9';

        public static bool StartsWithAny(this string str, string[] prefixes) {
            for (int i = 0; i < prefixes.Length; i++) {
                if (str.StartsWith(prefixes[i], StringComparison.Ordinal)) {
                    return true;
                }
            }
            return false;
        }

        public static string ToSnakeCase(this string text) {
            if (text == null) {
                throw new ArgumentNullException(nameof(text));
            }
            if (text.Length < 2) {
                return text;
            }
            var sb = new StringBuilder();
            sb.Append(char.ToLowerInvariant(text[0]));
            for (int i = 1; i < text.Length; ++i) {
                char c = text[i];
                if (char.IsUpper(c)) {
                    sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                } else {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static T ToEnum<T>(this string value, T defaultValue) where T : struct {
            if (string.IsNullOrEmpty(value)) {
                return defaultValue;
            }

            return Enum.TryParse(value, true, out T result) ? result : defaultValue;
        }

        public static bool ContainsPartial(this string text, string parted) {
            if (!text.IsFilled() || !parted.IsFilled()) {
                return false;
            }

            var pi = 0;
            var pc = char.ToLowerInvariant(parted[pi]);
            foreach (var t in text) {
                if (char.ToLowerInvariant(t) != pc) {
                    continue;
                }

                pi++;
                if (pi == parted.Length) {
                    return true;
                }
                pc = char.ToLowerInvariant(parted[pi]);
            }
            return false;
        }

        public static bool ApplyAsFilter(this string text, string target) {
            return string.IsNullOrEmpty(text) || target.ToLower().Contains(text.ToLower());
        }
    }
}
