using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Upmc.DevTools.VsParser.Entities.Project.Components
{
    /// <inheritdoc />
    /// <summary>
    ///     For parsing NuGet package reference entries in CS
    ///     Project files.
    /// </summary>
    [Serializable]
    public class PackageReference : SettingWithEnvironmentWithEnvironmentBase
    {
        /// <summary>
        ///     The target framework for this package, e.g. net45
        /// </summary>
        public string TargetFramework { get; set; }

        /// <summary>
        ///     Version of the package, e.g. 4.7.99
        /// </summary>
        public string Version { get; set; }

        /* Example:
        <PackageReference Include="DocumentFormat.OpenXml" Version="2.7.2" />
        */
    }
}