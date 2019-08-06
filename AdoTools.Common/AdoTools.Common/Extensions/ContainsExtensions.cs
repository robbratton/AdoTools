using System;
// ReSharper disable UnusedMember.Global

namespace Upmc.DevTools.Common.Extensions
{
    /// <summary>
    ///     Contains Extensions
    /// </summary>
    public static class ContainsExtensions
    {
        /// <summary>
        ///     Checks if a substring is contained in the current string.
        /// </summary>
        /// <param name="text">The current string.</param>
        /// <param name="value">The value to be located.</param>
        /// <param name="stringComparison">
        ///     <see cref="StringComparison" />
        /// </param>
        /// <returns></returns>
        public static bool ContainsInsensitive(
            this string text,
            string value,
            StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            return text.IndexOf(value, stringComparison) >= 0;
        }
    }
}