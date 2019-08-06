using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("AdoTools.Ado")]
[assembly:
    AssemblyDescription(
        "Provides access to various Azure DevOps services including Git, TFS VC, Builds, Policy, NuGet")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Robert E. Bratton")]
[assembly: AssemblyProduct("AdoTools.Ado")]
[assembly: AssemblyCopyright("Copyright © Robert E. Bratton 2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("05189637-5bd5-45ea-9afa-c2b5204a7bc5")]

[assembly: InternalsVisibleTo("AdoTools.Ado.Tests")]

// Version information will be derived from the GitVersion.yml file and other information by the GitVersion.exe application when this code is built on the Azure DevOps build server.