using System;
using System.Collections.Generic;
using Upmc.DevTools.VsParser.Entities.ConfigFile.Components;

// ReSharper disable UnusedMember.Global

namespace Upmc.DevTools.VsParser.Entities.ConfigFile
{
    /// <summary>
    ///     Contains the information specific to a web.config file.
    /// </summary>
    public class WebConfigFile
    {
        /// <summary>
        ///     Contains the application settings.
        /// </summary>
        public List<ApplicationSetting> AppSettings { get; set; }

        /// <summary>
        ///     Deploy Environment for this File
        /// </summary>
        public DeployEnvironment Environment { get; set; }
    }
}