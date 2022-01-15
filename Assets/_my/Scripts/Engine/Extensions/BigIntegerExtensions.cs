using System.Numerics;

namespace Smr.Extensions {
    public static class BigIntegerExtensions {
        private static readonly string[] ABBREVIATED_SUFFIXES = {
            "",
            "K",
            "M",
            "B",
            "T",
            "Q",
            "Qi",
            "Sx",
            "Sp",
            "Oc",
            "No",
            "Dc",
            "Und",
            "Duo",
            "Tre",
            "AA",
            "BB",
            "CC"
        };
        
        public static string ToAbbreviatedString(this BigInteger value) {
            if (value < 1000) {
                return value.ToString();
            }
            
            var suffixIndex = 0;
            var decimalValue = (decimal)value;

            while (decimalValue >= 1000 && suffixIndex < ABBREVIATED_SUFFIXES.Length - 1) {
                decimalValue /= 1000;
                suffixIndex++;
            }

            return $"{decimalValue:0.##}{ABBREVIATED_SUFFIXES[suffixIndex]}";
        }

        public static BigInteger FromString(string value) {
            return string.IsNullOrEmpty(value) ? BigInteger.Zero : BigInteger.Parse(value);
        }
    }
}