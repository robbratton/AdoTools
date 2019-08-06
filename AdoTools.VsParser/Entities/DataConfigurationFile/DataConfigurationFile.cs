using System;
using System.Collections.Generic;
using AdoTools.VsParser.Entities.DataConfigurationFile.Components;

// ReSharper disable UnusedMember.Global

namespace AdoTools.VsParser.Entities.DataConfigurationFile
{
    /// <summary>
    ///     These items are in the DataConfiguration.config file
    ///     in the enterpriseLibrary.databaseSettings section.
    /// </summary>
    public class DataConfigurationFile
    {
        /// <summary>
        ///     Collection of ConnectionStrings
        /// </summary>
        public List<ConnectionStringSetting> ConnectionStrings { get; set; } = new List<ConnectionStringSetting>();

        /// <summary>
        ///     Collection of Database Instances
        /// </summary>
        public List<DatabaseInstance> DatabaseInstances { get; set; } = new List<DatabaseInstance>();

        /// <summary>
        ///     Collection of Database Types
        /// </summary>
        public List<DatabaseType> DatabaseTypes { get; set; }

        /// <summary>
        ///     Deploy Environment for this File
        /// </summary>
        public DeployEnvironment Environment { get; set; }
    }
}