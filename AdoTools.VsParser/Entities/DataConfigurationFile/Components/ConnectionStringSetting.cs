using System;
using System.Xml.Serialization;

namespace AdoTools.VsParser.Entities.DataConfigurationFile.Components
{
    /// <inheritdoc />
    /// <summary>
    ///     Information for connection strings in app.config, web.config, DataConfiguration.config, etc.
    /// </summary>
    [Serializable]
    public class ConnectionStringSetting : SettingWithEnvironmentWithEnvironmentBase
    {
        /// <summary>
        ///     Database server name
        /// </summary>
        [XmlAttribute]
        public string Server { get; set; }

        /// <summary>
        ///     Database name
        /// </summary>
        [XmlAttribute]
        public string Database { get; set; }

        /// <summary>
        ///     Database UserID
        /// </summary>
        [XmlAttribute]
        public string UserId { get; set; }

        /// <summary>
        ///     Database Password
        /// </summary>
        [XmlAttribute]
        public string Password { get; set; }

        /// <summary>
        ///     Application Name
        /// </summary>
        [XmlAttribute]
        public string ApplicationName { get; set; }
    }
}