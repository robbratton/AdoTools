// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;

namespace Upmc.DevTools.VsParser.Entities.DataConfigurationFile.Components
{
    /// <inheritdoc />
    /// <summary>
    ///     Database Type
    /// </summary>
    public class DatabaseType : SettingWithEnvironmentWithEnvironmentBase
    {
        /// <summary>
        ///     The Type of Database
        ///     Example: Microsoft.Practices.EnterpriseLibrary.Data.Sql.SqlDatabase,
        ///     Microsoft.Practices.EnterpriseLibrary.Data,
        ///     Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
        /// </summary>
        public string Type { get; set; }

/* Example: <databaseType name="Sql Server" type="Microsoft.Practices.EnterpriseLibrary.Data.Sql.SqlDatabase, Microsoft.Practices.EnterpriseLibrary.Data, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null" />
         */
    }
}