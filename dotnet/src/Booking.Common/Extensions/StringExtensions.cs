using System;

namespace Booking.Common.Extensions
{
    public static class StringExtensions
    {
        public static string NullIfEmpty(this string s)
        {
            return (String.IsNullOrEmpty(s) ? null : s);
        }
        public static string NullIfWhiteSpace(this string s)
        {
            return (String.IsNullOrWhiteSpace(s) ? null : s);
        }
    }
}