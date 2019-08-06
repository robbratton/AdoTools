using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MissingXmlDoc

namespace Upmc.DevTools.VsParser.Entities.ConfigFile
{
    /// <summary>
    ///     General Model for App Settings Files
    /// </summary>
    [JsonObject]
    public class AppSettingsJsonFile
    {
        /// <summary>
        ///     Application Settings
        /// </summary>
        [JsonProperty("AppSettings")]
        public IDictionary<string, string> AppSettings { get; set; }

        /// <summary>
        ///     Connection Strings
        /// </summary>
        [JsonProperty("ConnectionStrings")]
        public IDictionary<string, string> ConnectionStrings { get; set; }

        /// <summary>
        ///     Core Monitoring Settings
        /// </summary>
        [JsonProperty("CoreMonitoringSettingsConfigurationElement")]
        public IDictionary<string, string> CoreMonitoringSettingsConfiguration { get; set; }

        /// <summary>
        ///     Core Persistence Settings
        /// </summary>
        [JsonProperty("CorePersistenceSettingsConfigurationElement")]
        public IDictionary<string, string> CorePersistenceSettingsConfiguration { get; set; }

        /// <summary>
        ///     Core Service Bus Settings
        /// </summary>
        [JsonProperty("CoreServiceBusConfigurationSettingsSection")]
        public IDictionary<string, string> CoreServiceBusConfiguration { get; set; }

        /// <summary>
        ///     Logging Settings
        /// </summary>
        [JsonProperty("Logging")]
        public LoggingSection Logging { get; set; }

        /// <summary>
        ///     Open Auth 2 Settings
        /// </summary>
        [JsonProperty("OAuth2Settings")]
        public IDictionary<string, string> OAuth2 { get; set; }
    }
}