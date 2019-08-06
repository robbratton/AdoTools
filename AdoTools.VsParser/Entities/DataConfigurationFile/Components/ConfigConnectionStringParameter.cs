// ReSharper disable UnusedMember.Global

using System;

namespace AdoTools.VsParser.Entities.DataConfigurationFile.Components
{
    /// <inheritdoc />
    /// <summary>
    ///     Connection String Parameter from a Configuration file
    ///     used by Enterprise Library
    /// </summary>
    public class ConfigConnectionStringParameter : SettingWithEnvironmentWithEnvironmentBase
    {
        /// <summary>
        ///     True if this Item is Case Sensitive.
        /// </summary>
        public bool IsSensitive { get; set; }

        /// <summary>
        ///     The parameter's Value
        /// </summary>
        public string Value { get; set; }
    }
}