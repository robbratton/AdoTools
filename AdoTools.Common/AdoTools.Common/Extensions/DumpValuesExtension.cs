using System;

// ReSharper disable UnusedMember.Global

namespace Upmc.DevTools.Common.Extensions
{
    /// <summary>
    ///     Dump Values Extensions
    /// </summary>
    public static class DumpValuesExtension
    {
        /// <summary>
        ///     Dump fields for a class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The value to be dumped</param>
        /// <param name="indent">Initial indent level</param>
        /// <param name="maxDepth">Maximum Depth</param>
        /// <param name="indentString">String to be used to indent</param>
        /// <param name="handlers">Leave null to use the default handlers.</param>
        public static string DumpValues<T>(
            this T input,
            int indent,
            int maxDepth = 10,
            string indentString = "  ",
            IVariableDumpHandlers handlers = null
        ) where T : class
        {
            var myHandlers = handlers ?? new VariableDumpHandlers(maxDepth, indentString);

            var output = input == null
                ? "(null)"
                : myHandlers.HandleItem(input, indent);

            return output;
        }
    }
}