using System;
using System.Reflection;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace AdoTools.VsParser.Entities.Project.Components
{
    /// <inheritdoc />
    /// <summary>
    ///     For parsing assembly reference entries in CS Project
    ///     files.
    ///     <example>
    ///         &lt;Reference
    ///         Include="Infragistics2.Win.UltraWinSchedule.v11.1,
    ///         Version=11.1.20111.2111, Culture=neutral,
    ///         PublicKeyToken=7dd5c3163f2cd0cb,
    ///         processorArchitecture=MSIL"&gt;
    ///         &lt;HintPath&gt;..\..\..\..\ThirdParty\Infragistics2.Win.UltraWinSchedule.v11.1.dll&lt;/HintPath&gt;
    ///         &lt;SpecificVersion&gt;False&lt;/SpecificVersion&gt;
    ///         &lt;/Reference&gt;
    ///     </example>
    /// </summary>
    public class AssemblyReference : SettingWithEnvironmentWithEnvironmentBase
    {
        /// <summary>
        ///     Culture Configuration
        ///     <example>neutral</example>
        /// </summary>
        public string Culture { get; set; }

        /// <summary>
        ///     The Hint Path to the Location of the Assembly
        ///     File
        ///     <example>..\..\..\..\ThirdParty\Infragistics2.Win.UltraWinSchedule.v11.1.dll</example>
        /// </summary>
        public string HintPath { get; set; }

        /// <summary>
        ///     The Processor Architecture Targeted
        ///     <example>MSIL</example>
        /// </summary>
        public ProcessorArchitecture ProcessorArchitecture { get; set; }

        /// <summary>
        ///     The Public Key Token of the Referenced Assembly
        ///     <example>7dd5c3163f2cd0cb</example>
        /// </summary>
        public string PublicKeyToken { get; set; }

        /// <summary>
        ///     True if the Reference is for a Specific Version
        /// </summary>
        public bool? SpecificVersion { get; set; }

        /// <summary>
        ///     Version of the Referenced Assembly
        /// </summary>
        public string Version { get; set; }
    }
}