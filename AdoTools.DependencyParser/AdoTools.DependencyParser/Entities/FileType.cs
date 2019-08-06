using System;

namespace Upmc.DevTools.Dependency.Parser.Entities
{
    /// <summary>
    ///     The type of file being parsed
    /// </summary>
    public enum FileType
    {
        /// <summary>
        ///     Unknown or unsupported file type
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     Visual Studio Solution (*.sln)
        /// </summary>
        Solution,

        /// <summary>
        ///     C Sharp Project (*.csproj)
        /// </summary>
        CsProject,

        /// <summary>
        ///     SQL Server Project (*.sqlproj)
        /// </summary>
        SqlProject,

        /// <summary>
        ///     App or Web Config (app.config, web.config, DataConfiguration.config, etc.)
        /// </summary>
        AppWebDataConfig,

        /// <summary>
        ///     JSON App Config (appsettings.json)
        /// </summary>
        AppsettingsJson,

        /// <summary>
        ///     NuGet Packages.config
        /// </summary>
        PackageConfig,

        /// <summary>
        ///     assemblyInfo.cs
        /// </summary>
        AssemblyInfo
    }
}