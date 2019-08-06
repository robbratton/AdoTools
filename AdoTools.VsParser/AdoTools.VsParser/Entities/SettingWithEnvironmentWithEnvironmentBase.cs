using System;
using System.Diagnostics.CodeAnalysis;

namespace Upmc.DevTools.VsParser.Entities
{
    /// <inheritdoc />
    /// <summary>
    ///     Base class for settings which include an environment and name.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class SettingWithEnvironmentWithEnvironmentBase : ISettingWithEnvironment
    {
        /// <inheritdoc />
        /// <summary>
        ///     The deploy environment of this setting. Usually obtained from the filename, e.g. web.QA.config is assumed to be QA.
        /// </summary>
        public DeployEnvironment Environment { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     The name of this setting.
        /// </summary>
        public string Name { get; set; }
    }
}