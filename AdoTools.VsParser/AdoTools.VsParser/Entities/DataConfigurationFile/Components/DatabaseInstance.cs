// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

using System;

namespace Upmc.DevTools.VsParser.Entities.DataConfigurationFile.Components
{
    /// <inheritdoc />
    /// <summary>
    ///     Information about a Database Instance used by
    ///     Enterprise Library
    /// </summary>
    public class DatabaseInstance : SettingWithEnvironmentWithEnvironmentBase
    {
        /// <summary>
        ///     Connection String Name
        ///     Example: Enterprise
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        ///     Database Instance Type
        ///     Example: Sql Server
        /// </summary>
        public string Type { get; set; }

        /*
        Example: <instance name="Enterprise" type="Sql Server" connectionString="Enterprise" />
        */
    }
}