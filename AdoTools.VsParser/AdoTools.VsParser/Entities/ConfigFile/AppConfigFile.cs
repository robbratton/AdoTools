using System;
using System.Collections.Generic;
using Upmc.DevTools.VsParser.Entities.ConfigFile.Components;

// ReSharper disable UnusedMember.Global

namespace Upmc.DevTools.VsParser.Entities.ConfigFile
{
    /// <summary>
    ///     Contents of an app.config file.
    /// </summary>
    public class AppConfigFile
    {
        /// <summary>
        ///     Represents a single group of settings
        /// </summary>
        public List<ApplicationSetting> ApplicationSettings { get; set; }

        /// <summary>
        ///     Deploy Environment for this File
        /// </summary>
        public DeployEnvironment Environment { get; set; }
    }
}