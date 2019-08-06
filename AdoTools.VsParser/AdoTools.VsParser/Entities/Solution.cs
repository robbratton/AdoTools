using System;
using System.Collections.Generic;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable CollectionNeverQueried.Global

// ReSharper disable UnusedMember.Global

namespace Upmc.DevTools.VsParser.Entities
{
    /// <summary>
    ///     Contains parsed data about a Visual Studio solution (*.sln)
    /// </summary>
    public class Solution
    {
        /// <summary>
        ///     The file's format version.
        /// </summary>
        public string FormatVersion { get; set; }

        /// <summary>
        ///     The minimum Visual Studio version required to
        ///     process this file.
        /// </summary>
        public string MinimumVisualStudioVersion { get; set; }

        /// <summary>
        ///     A collection of projects contained in this solution.
        ///     WARNING: The details of the projects are very
        ///     sparse until each has been processed by
        ///     the project parser.
        /// </summary>
        public List<Project.Project> Projects { get; } = new List<Project.Project>();

        /// <summary>
        ///     The version of Visual Studio which created or
        ///     edited this file.
        /// </summary>
        public string VisualStudioVersion { get; set; }

        /// <summary>
        ///     Information from the GitVersion file.
        /// </summary>
        public GitVersion GitVersion { get; set; }

        // todo Configurations for each project? 
        // todo SCC?
    }
}