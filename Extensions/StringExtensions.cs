using System;

namespace Rustic.Extensions
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        public static bool SameAs(this string source, string checkString)
        {
            return (string.IsNullOrEmpty(source) && string.IsNullOrEmpty(checkString)) ||
                string.Equals(source, checkString, StringComparison.OrdinalIgnoreCase);
        }
    }
}
