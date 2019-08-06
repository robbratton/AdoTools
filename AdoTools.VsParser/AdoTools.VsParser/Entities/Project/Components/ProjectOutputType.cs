using System;

namespace Upmc.DevTools.VsParser.Entities.Project.Components
{
    /// <summary>
    ///     The output which will be generated when building a project.
    /// </summary>
    public enum ProjectOutputType
    {
        /// <summary>
        ///     Class Library
        /// </summary>
        Library = 0,

        /// <summary>
        ///     Windows Classic Executable
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        Exe = 1,

        /// <summary>
        ///     Windows Classic Executable
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        WinExe = 2,

        /// <summary>
        ///     Placeholder for items which can't be determined.
        /// </summary>
        Placeholder = 3
    }
}