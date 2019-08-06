using System;
using System.Collections.Generic;

// ReSharper disable UnusedMember.Global

namespace AdoTools.VsParser.Entities
{
    /// <summary>
    ///     Describes the content of the AssemblyInfo.cs file or
    ///     the assembly info section in a CS Project file. If
    ///     these settings are in an AssemblyInfo.cs file and the
    ///     CSPRoj is in the new format, there must be entries in
    ///     the CS Project file like this
    ///     &lt;GenerateAssemblyTitleAttribute&gt;false&lt;/GenerateAssemblyTitleAttribute&gt;
    ///     The assemblyInfo.cs file is a CSharp source file with
    ///     attributes starting with assembly:. Example
    ///     [assembly: AssemblyCulture("")]
    /// </summary>
    public class AssemblyInfoFile
    {
        /// <summary>
        ///     Example: UPMC
        /// </summary>
        public KeyValuePair<string, string> Company { get; set; }

        /// <summary>
        ///     True if this Assembly should be Visible to COM
        /// </summary>
        public KeyValuePair<string, bool> ComVisible { get; set; }

        /// <summary>
        ///     Configuration
        /// </summary>
        public KeyValuePair<string, string> Configuration { get; set; }

        /// <summary>
        ///     Example: Copyright (C) UPMC 2017
        /// </summary>
        public KeyValuePair<string, string> Copyright { get; set; }

        /// <summary>
        ///     Culture
        /// </summary>
        public KeyValuePair<string, string> Culture { get; set; }

        /// <summary>
        ///     Description
        /// </summary>
        public KeyValuePair<string, string> Description { get; set; }

        /// <summary>
        ///     Example: 1.0.0.0
        /// </summary>
        public KeyValuePair<string, string> FileVersion { get; set; }

        /// <summary>
        ///     The GUID for this Assembly
        /// </summary>
        public KeyValuePair<string, Guid> Guid { get; set; }

        /// <summary>
        ///     Example: Upmc.Web.WcfRestHttp
        /// </summary>
        public KeyValuePair<string, string> Product { get; set; }

        /// <summary>
        ///     Example: Upmc.Web.WcfRestHttp
        /// </summary>
        public KeyValuePair<string, string> Title { get; set; }

        /// <summary>
        ///     The Trademark for this Assembly
        /// </summary>
        public KeyValuePair<string, string> TradeMark { get; set; }

        /// <summary>
        ///     Example: 1.0.0.0
        /// </summary>
        public KeyValuePair<string, string> Version { get; set; }
    }
}