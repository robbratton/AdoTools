// ReSharper disable MemberCanBePrivate.Global ReSharper
// disable AutoPropertyCanBeMadeGetOnly.Global ReSharper
// disable UnusedAutoPropertyAccessor.Global

using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace AdoTools.VsParser.Entities.ConfigFile.Components
{
    /// <inheritdoc />
    /// <summary>
    ///     A single setting element
    /// </summary>
    public class ApplicationSetting : SettingWithEnvironmentWithEnvironmentBase
    {
        /// <inheritdoc />
        public ApplicationSetting(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <inheritdoc />
        public ApplicationSetting(string name, string value, string serializeAs)
        {
            Name = name;
            Value = value;
            SerializeAs = serializeAs;
        }

        /// <summary>
        ///     The format for Serializing this Setting
        /// </summary>
        public string SerializeAs { get; set; }

        /// <summary>
        ///     The Setting's Value
        /// </summary>
        public string Value { get; set; }
    }
}