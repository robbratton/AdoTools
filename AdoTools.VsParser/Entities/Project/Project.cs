using System;
using System.Collections.Generic;
using AdoTools.VsParser.Entities.Project.Components;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace AdoTools.VsParser.Entities.Project
{
    /// <summary>
    ///     Contains information from a Visual Studio Project File. Includes information from the parent Solution file.
    /// </summary>
    public class Project
    {
        /// <summary>
        ///     Information from the AssemblyInfo data in the the Project
        /// </summary>
        public List<KeyValuePair<string, string>> AssemblyInformation { get; set; } =
            new List<KeyValuePair<string, string>>();

        /// <summary>
        ///     References to Assemblies
        /// </summary>
        public List<AssemblyReference> AssemblyReferences { get; set; } = new List<AssemblyReference>();

        /// <summary>
        ///     Configurations used in this Project
        ///     Examples: Debug, AnyCPU; Release, AnyCPU
        /// </summary>
        public List<Tuple<string, string>> Configurations { get; set; } = new List<Tuple<string, string>>();

        /// <summary>
        ///     The .NET Frameworks in this Project
        /// </summary>
        public List<string> Frameworks { get; set; } = new List<string>();

        /// <summary>
        ///     True if the Project Contains a Reference to messages.resx
        /// </summary>
        public bool HasMessages { get; set; }

        /// <summary>
        ///     This is the project's Name from the SOLUTION FILE.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        ///     This is the project's name from the SOLUTION FILE.
        /// </summary>
        public string NameInSolution { get; set; }

        /// <summary>
        ///     References to NuGet Packages
        /// </summary>
        public List<PackageReference> PackageReferences { get; set; } = new List<PackageReference>();

        /// <summary>
        ///     This project's path from the SOLUTION FILE.
        /// </summary>
        public string PathRelativeToSolution { get; set; }

        /// <summary>
        ///     The format of this file
        /// </summary>
        public ProjectFileFormat? ProjectFileFormat { get; set; }

        /// <summary>
        ///     The output type
        /// </summary>
        public ProjectOutputType ProjectOutputType { get; set; }

        /// <summary>
        ///     References to other projects
        /// </summary>
        public List<string> ProjectReferences { get; set; } = new List<string>();

        /// <summary>
        ///     The SDK name
        /// </summary>
        public string Sdk { get; set; }

        /// <summary>
        ///     This is the project's list of TypeIds from the
        ///     SOLUTION FILE.
        /// </summary>
        public List<Guid> TypeIds { get; set; } = new List<Guid>();

        // todo ? public IEnumerable<OtherInclude>
        // OtherIncludes { get; set; }
    }
}