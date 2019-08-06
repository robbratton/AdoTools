using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AdoTools.DependencyParser.Entities;
using AdoTools.VsParser.Entities;
using AdoTools.VsParser.Entities.Project;
using AdoTools.VsParser.Entities.Project.Components;
using AdoTools.Ado.Entities;

namespace AdoTools.DependencyParser.Tests
{
    public static class Helpers
    {
        public static SolutionMetadata CreateSolutionMetadata(
            string name,
            bool sourceInformationIsNull,
            bool solutionIsNull,
            out SourceInformation sourceInformation,
            out Solution solution)
        {
            sourceInformation = null;

            if (!sourceInformationIsNull)
            {
                sourceInformation = CreateTestSourceInformation();
            }

            solution = null;

            if (!solutionIsNull)
            {
                solution = CreateTestSolution();
            }

            var output = new SolutionMetadata(name, sourceInformation, solution);

            return output;
        }

        public static Project CreateTestProject()
        {
            return new Project
            {
                AssemblyInformation =
                    new List<KeyValuePair<string, string>> {new KeyValuePair<string, string>("x", "y")},
                AssemblyReferences = new List<AssemblyReference> {new AssemblyReference()},
                Configurations = new List<Tuple<string, string>> {new Tuple<string, string>("x", "y")},
                // ReSharper disable once StringLiteralTypo
                Frameworks = new List<string> {"net461", "netcoreapp2.0"},
                PackageReferences = new List<PackageReference> {new PackageReference()},
                ProjectReferences = new List<string> {"ref1"},
                HasMessages = true,
                ProjectFileFormat = ProjectFileFormat.Vs2017OrLater,
                ProjectOutputType = ProjectOutputType.Library,
                Sdk = "Sdk"
            };
        }

        public static Solution CreateTestSolution()
        {
            var output = new Solution
            {
                FormatVersion = "Format Version",
                MinimumVisualStudioVersion = "Min VS Version",
                VisualStudioVersion = "VS Version"
            };
            output.Projects.AddRange(new List<Project> {CreateTestProject()});

            return output;
        }

        public static SourceInformation CreateTestSourceInformation()
        {
            return new SourceInformation
            {
                IsDirectory = false,
                SourcePath = @"c:\source_path\" + Guid.NewGuid() + ".csproj",
                SourceType = SourceType.Filesystem
            };
        }

        public static string ReadEmbeddedResourceFile(string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
            {
                throw new ArgumentException("Value must not be null or whitespace.", nameof(resourceName));
            }

            string output;

            var assembly = Assembly.GetExecutingAssembly();

            var resourcePath = @"AdoTools.DependencyParser.Tests.TestFiles." + resourceName + ".txt";

            using (var stream = assembly.GetManifestResourceStream(resourcePath))
                //using (var stream = assembly.GetManifestResourceStream(typeof(Helpers), resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"Embedded resource file {resourcePath} was not found.");

                    //throw new FileNotFoundException($"Embedded resource file {resourceName} was not found.");
                }

                using (var reader = new StreamReader(stream))
                {
                    output = reader.ReadToEnd();
                }
            }

            return output;
        }
    }
}