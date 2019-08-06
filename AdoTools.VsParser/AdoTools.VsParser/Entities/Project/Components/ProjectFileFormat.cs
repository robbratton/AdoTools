using System;

namespace Upmc.DevTools.VsParser.Entities.Project.Components
{
    /// <summary>
    ///     The Visual Studio version of a CSharp project file
    /// </summary>
    public enum ProjectFileFormat
    {
        /// <summary>
        ///     Unknown Visual Studio Version
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        Unknown = 0,

        /// <summary>
        ///     Visual Studio 2015 or Earlier
        /// </summary>
        Vs2015OrEarlier,

        /// <summary>
        ///     Visual Studio 2017 or Later
        /// </summary>
        Vs2017OrLater
    }
}