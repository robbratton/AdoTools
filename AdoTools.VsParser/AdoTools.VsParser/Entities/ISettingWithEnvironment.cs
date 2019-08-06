// ReSharper disable UnusedMemberInSuper.Global

using System;

// ReSharper disable UnusedMember.Global

namespace Upmc.DevTools.VsParser.Entities
{
    /// <summary>
    ///     Interface for Settings which Include the Deploy
    ///     Environment and Name
    /// </summary>
    public interface ISettingWithEnvironment
    {
        /// <summary>
        ///     Deploy Environment
        /// </summary>
        DeployEnvironment Environment { get; set; }

        /// <summary>
        ///     Setting Name
        /// </summary>
        string Name { get; set; }
    }
}