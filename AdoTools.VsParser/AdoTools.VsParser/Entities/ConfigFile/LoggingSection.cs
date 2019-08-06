using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MissingXmlDoc

namespace Upmc.DevTools.VsParser.Entities.ConfigFile
{
    /// <summary>
    ///     Logging Section of AppSettingsJsonFile
    /// </summary>
    [JsonObject("LoggingSection")]
    public class LoggingSection
    {
        /// <summary>
        ///     Include Scopes
        /// </summary>
        [JsonProperty("IncludeScopes")]
        public bool IncludeScopes { get; set; }

        /// <summary>
        ///     Log Level
        /// </summary>
        [JsonProperty("LogLevel")]
        public IDictionary<string, string> LogLevel { get; set; }
    }
}