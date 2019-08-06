using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AdoTools.VsParser.Tests")]

namespace AdoTools.VsParser.Entities
{
    /// <summary>
    ///     Methods to help with using the DeployEnvironment enumeration
    /// </summary>
    public static class DeployEnvironmentHelper
    {
        /// <summary>
        ///     Maps a string environment to <see cref="DeployEnvironment" />
        /// </summary>
        /// <param name="value"></param>
        /// <param name="throwIfNotMatched">
        ///     If true, throws an ArgumentException if the value
        ///     cannot be matched.
        /// </param>
        /// <returns>
        ///     <see cref="DeployEnvironment" />
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        public static DeployEnvironment Map(string value, bool throwIfNotMatched = false)
        {
            DeployEnvironment output;

            if (string.IsNullOrWhiteSpace(value))
            {
                if (throwIfNotMatched)
                {
                    throw new ArgumentException("value must not be null or whitespace.", nameof(value));
                }

                output = DeployEnvironment.None;
            }
            else
            {
                if (!Enum.TryParse(value, true, out output))
                {
                    switch (value.ToLower())
                    {
                        case "debug":
                        case "release":
                            output = DeployEnvironment.None;

                            break;

                        case "dev":
                            output = DeployEnvironment.Development;

                            break;

                        case "prod":
                            output = DeployEnvironment.Production;

                            break;

                        default:

                            if (throwIfNotMatched)
                            {
                                throw new ArgumentException($"Unknown string: \"{value}\"", nameof(value));
                            }
                            else
                            {
                                output = DeployEnvironment.None;
                            }

                            break;
                    }
                }
            }

            return output;
        }
    }
}