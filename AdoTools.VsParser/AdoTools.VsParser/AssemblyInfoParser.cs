using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Upmc.DevTools.Common;

// ReSharper disable UnusedMember.Global

namespace Upmc.DevTools.VsParser
{
    /// <summary>
    ///     Processes AssemblyInfo.cs content.
    /// </summary>
    public static class AssemblyInfoParser
    {
        /// <summary>
        ///     Captures name/value pairs.
        /// </summary>
        private static readonly Regex AssemblyInfoLine = new Regex(
            @"^\[assembly: (.+)\(""(.+)""\)\]$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        ///     Gets the assembly values from the content of the
        ///     assemblyInfo.cs file.
        /// </summary>
        /// <param name="content">
        ///     Content from an assemblyInfo.cs file.
        /// </param>
        /// <returns>Collection of name and value strings</returns>
        public static IEnumerable<KeyValuePair<string, string>> ParseFileContent(string content)
        {
            Validators.AssertIsNotNullOrWhitespace(content, nameof(content));

            // Example [assembly: AssemblyCompany("MyProject")]

            var lines = Regex.Split(content, "\r\n|\r|\n");
            var output = lines
                .Select(line => AssemblyInfoLine.Match(line))
                .Where(match => match.Success && match.Groups.Count == 3)
                .Select(match => new KeyValuePair<string, string>(match.Groups[1].Value, match.Groups[2].Value));

            return output.ToList();
        }
    }
}