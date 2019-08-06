using System;
using System.Text.RegularExpressions;
// ReSharper disable UnusedMember.Global

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Upmc.DevTools.Common.Entities
{
    /// <summary>
    ///     Class to Store and Process Software Versions
    /// </summary>
    public class SoftwareVersion
    {
        // Example "1.2.3.4"

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="input"></param>
        public SoftwareVersion(string input)
        {
            Validators.AssertIsNotNullOrWhitespace(input, nameof(input));

            var match = Regex.Match(input, @"^(\d+)\.(\d+)\.(\d+)(\.([^. ]+))?$");
            if (!match.Success)
            {
                throw new ArgumentException("The version string must be in the format: 9.9.9.9, 9.9.9 or 9.9.9.x.");
            }

            Major = int.Parse(match.Groups[1].Value);
            Minor = int.Parse(match.Groups[2].Value);
            Build = int.Parse(match.Groups[3].Value);
            Revision = match.Groups[5].Value;
        }

        /// <summary>
        ///     Major Version Segment
        /// </summary>
        public int Major { get; set; }

        /// <summary>
        ///     Minor Version Segment
        /// </summary>
        public int Minor { get; set; }

        /// <summary>
        ///     Build Version Segment
        /// </summary>
        public int Build { get; set; }

        /// <summary>
        ///     Revision Version Segment
        /// </summary>
        public string Revision { get; set; }

        /// <summary>
        ///     Custom ToString Implementation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var revisionOutput = "";
            if (!string.IsNullOrWhiteSpace(Revision))
            {
                revisionOutput = $".{Revision}";
            }

            return $"{Major}.{Minor}.{Build}{revisionOutput}";
        }
    }
}