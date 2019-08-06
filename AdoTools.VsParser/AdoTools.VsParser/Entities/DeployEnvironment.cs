using System;

// ReSharper disable UnusedMember.Global

namespace Upmc.DevTools.VsParser.Entities
{
    /// <summary>
    ///     The environment where the item will be deployed
    /// </summary>
    public enum DeployEnvironment
    {
        /// <summary>
        ///     Unknown or None
        /// </summary>
        None = 0,

        /// <summary>
        ///     Development Deploy Environment. Generally a Debug build.
        /// </summary>
        Development,

        /// <summary>
        ///     Lab Deploy Environment. Generally a Release build.
        /// </summary>
        Lab,

        /// <summary>
        ///     QA Deploy Environment. Generally a Release build.
        /// </summary>
        Qa,

        /// <summary>
        ///     Production Support Deploy Environment. Generally a Release build.
        /// </summary>
        ProdSupport,

        /// <summary>
        ///     Training Deploy Environment. Generally a Release build.
        /// </summary>
        Training,

        /// <summary>
        ///     Production Deploy Environment. Generally a Release build.
        /// </summary>
        Production
    }
}