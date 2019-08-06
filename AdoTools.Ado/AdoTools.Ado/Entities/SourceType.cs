using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Upmc.DevTools.VsTs.Tests")]

namespace Upmc.DevTools.VsTs.Entities
{
    /// <summary>
    ///     Type of Source System
    /// </summary>
    public enum SourceType
    {
        /// <summary>
        ///     Not applicable
        /// </summary>
        None = 0,

        /// <summary>
        ///     TFS Version Control
        /// </summary>
        TfsVc,

        /// <summary>
        ///     Git repository
        /// </summary>
        TfsGit,

        /// <summary>
        ///     Local Filesystem
        /// </summary>
        Filesystem
    }
}