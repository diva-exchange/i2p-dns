using System.Text.RegularExpressions;

namespace diva_dns.Util
{
    public class InputValidation
    {
        private static Regex _nsV34Matcher = new Regex(@"^([A-Za-z_-]{4,15}:){1,4}[A-Za-z0-9_-]{1,64}$");
        private static Regex _b32Matcher = new Regex(@"^[a-z0-9]{52}$");
        private InputValidation() { }

        public static bool IsProperNsV34(string ns) => _nsV34Matcher.IsMatch(ns);

        public static bool IsB32String(string b32) => _b32Matcher.IsMatch(b32);
    }
}
