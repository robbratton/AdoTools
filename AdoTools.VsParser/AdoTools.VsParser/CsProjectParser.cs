using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Upmc.DevTools.Common;
using Upmc.DevTools.VsParser.Entities.Project;
using Upmc.DevTools.VsParser.Entities.Project.Components;
using static System.Globalization.CultureInfo;

// ReSharper disable UnusedMember.Global

// ReSharper disable MemberCanBePrivate.Global

namespace Upmc.DevTools.VsParser
{
    /// <summary>
    ///     Processes *.csproj file content.
    /// </summary>
    public static class CsProjectParser
    {
        /// <summary>
        ///     Update the Project Meta Data from the content
        ///     from a CS Project file in the old or new format.
        /// </summary>
        /// <param name="content">File content</param>
        /// <param name="path"></param>
        /// <returns>
        ///     <see cref="Project" /> if the content was parsed,
        ///     <c>null</c> otherwise.
        /// </returns>
        public static Project ParseProjectFileContent(string content, string path)
        {
            Validators.AssertIsNotNullOrWhitespace(content, nameof(content));
            Validators.AssertIsNotNullOrWhitespace(path, nameof(path));

            var output = new Project();

            var xDocument = XDocument.Parse(content);

            output.AssemblyInformation = GetAssemblyInformation(xDocument);
            output.AssemblyReferences = GetAssemblyReferences(xDocument);
            output.Configurations = GetConfigurations(xDocument);
            output.Frameworks = GetFrameworks(xDocument); // todo Add default if it's missing?
            output.HasMessages = GetMessages(content);
            output.PackageReferences = GetPackageReferences(xDocument);
            output.ProjectFileFormat = GetProjectFileFormat(xDocument);
            output.ProjectOutputType = GetProjectOutputType(xDocument, path); // todo Add default if it's missing?
            output.ProjectReferences = GetProjectReferences(xDocument);
            output.Sdk = GetSdk(xDocument);

            return output;
        }

        internal static List<AssemblyReference> GetAssemblyReferences(XContainer xDocument)
        {
            var references = xDocument.Descendants()
                .Where(r => r.Name.LocalName.Equals("Reference", StringComparison.CurrentCultureIgnoreCase));

            var output = ParseAssemblyReferences(references);

            return output;
        }

        internal static List<Tuple<string, string>> GetConfigurations(XContainer xDocument)
        {
            var output = new List<Tuple<string, string>>();

            // Find property groups which have the Condition attribute
            var propertyGroups = xDocument.Descendants()
                .Where(
                    r => r.Name.LocalName.Equals("PropertyGroup", StringComparison.CurrentCultureIgnoreCase)
                         && r.HasAttributes
                         && r.Attribute("Condition") != null
                );

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var propertyGroup in propertyGroups)
            {
                var conditionAttribute = propertyGroup.Attribute("Condition");

                if (conditionAttribute != null)
                {
                    var match = Regex.Match(conditionAttribute.Value, @"== ?'([^|']+)\|([^|']+)'");

                    if (match.Success && match.Groups.Count == 3)
                    {
                        var configuration = match.Groups[1].Value;
                        var platform = match.Groups[2].Value;

                        output.Add(new Tuple<string, string>(configuration, platform));
                    }
                }
            }

            return output;
        }

        internal static List<string> GetFrameworks(XContainer xDocument)
        {
            var frameworkFromDoc1 = xDocument.Descendants()
                .Where(
                    x => Regex.IsMatch(x.Name.LocalName, "^TargetFramework(Version)?s?$", RegexOptions.IgnoreCase)
                         && !string.IsNullOrWhiteSpace(x.Value))
                .Select(z => z.Value)
                .Distinct();

            var output = MapFrameworks(frameworkFromDoc1).ToList();

            return output;
        }

        internal static bool GetMessages(string content)
        {
            return InvariantCulture.CompareInfo.IndexOf(
                       content,
                       "<Folder Include=\"Resources\" />",
                       CompareOptions.IgnoreCase)
                   >= 0
                   || InvariantCulture.CompareInfo.IndexOf(content, "Messages.resx", CompareOptions.IgnoreCase) >= 0;
        }

        internal static List<PackageReference> GetPackageReferences(XContainer xDocument)
        {
            var references = xDocument.Descendants()
                .Where(r => r.Name.LocalName.Equals("PackageReference", StringComparison.CurrentCultureIgnoreCase));

            return ParsePackageReferences(references);
        }

        internal static ProjectFileFormat? GetProjectFileFormat(XDocument xDocument)
        {
            var sdk = GetSdk(xDocument);

            ProjectFileFormat? output = string.IsNullOrWhiteSpace(sdk)
                ? ProjectFileFormat.Vs2015OrEarlier
                : ProjectFileFormat.Vs2017OrLater;

            return output;
        }

        internal static ProjectOutputType GetProjectOutputType(XContainer xDocument, string filename)
        {
            ProjectOutputType output;

            if (filename.EndsWith(".csproj", StringComparison.CurrentCultureIgnoreCase))
            {
                var outputTypes = xDocument.Descendants()
                    .Where(x => x.Name.LocalName.Equals("OutputType", StringComparison.CurrentCultureIgnoreCase))
                    .Select(z => z.Value)
                    .Distinct()
                    .ToArray();

                if (outputTypes.Length > 0)
                {
                    if (!Enum.TryParse(outputTypes[0], out output))
                    {
                        throw new InvalidOperationException($"Could not parse {outputTypes[0]} as an outputType.");
                    }
                }
                else
                {
                    // Library was assumed because no other
                    // OutputType was defined.
                    output = ProjectOutputType.Library;
                }
            }
            else
            {
                output = ProjectOutputType.Placeholder;
            }

            return output;
        }

        internal static List<string> GetProjectReferences(XContainer xDocument)
        {
            var references =
                from r in xDocument.Descendants()
                where r.Name.LocalName.Equals("ProjectReference", StringComparison.CurrentCultureIgnoreCase)
                      && r.HasAttributes
                      && r.HasElements
                select r;

            return ParseProjectReferences(references);
        }

        internal static string GetSdk(XDocument xDocument)
        {
            var output = xDocument.Root?.Attribute("Sdk")?.Value;

            return output;
        }

        internal static IEnumerable<string> MapFrameworks(IEnumerable<string> frameworks)
        {
            // framework version Input format: "v4.6.1;v4.7"
            // or "net461;net47" output format: "net461;net47"

            var output = new List<string>();

            foreach (var frameworkString in frameworks)
            {
                foreach (var framework in frameworkString.Split(';'))
                {
                    if (framework.StartsWith("v", StringComparison.CurrentCultureIgnoreCase))
                    {
                        output.Add("net" + frameworkString.Replace("v", "").Replace(".", ""));
                    }
                    else
                    {
                        output.Add(framework);
                    }
                }
            }

            return output;
        }

        internal static List<KeyValuePair<string, string>> ParseAssemblyInformationItems(IEnumerable<XElement> items)
        {
            return items.Select(
                    reference => new KeyValuePair<string, string>(reference.Name.LocalName, reference.Value)
                )
                .ToList();
        }

        internal static List<AssemblyReference> ParseAssemblyReferences(IEnumerable<XElement> assemblyReferences)
        {
            var output = new List<AssemblyReference>();

            foreach (var reference in assemblyReferences)
            {
                var newItem = new AssemblyReference();

                var include = reference.Attribute("Include")?.Value;

                if (!string.IsNullOrWhiteSpace(include))
                {
                    var includeParts = Regex.Split(include, ", +");

                    foreach (var part in includeParts)
                    {
                        var partSplit = Regex.Split(part, " *= *");

                        if (partSplit.Length == 1)
                        {
                            newItem.Name = part;
                        }
                        else
                        {
                            switch (partSplit[0])
                            {
                                case "Version":
                                    newItem.Version = partSplit[1];

                                    break;

                                case "Culture":
                                    newItem.Culture = partSplit[1];

                                    break;

                                case "PublicKeyToken":
                                    newItem.PublicKeyToken = partSplit[1];

                                    break;

                                case "processorArchitecture":
                                    newItem.ProcessorArchitecture =
                                        (ProcessorArchitecture) Enum.Parse(
                                            typeof(ProcessorArchitecture),
                                            partSplit[1],
                                            true);

                                    break;

                                default:

                                    throw new InvalidOperationException("Cannot process assembly item " + partSplit[0]);
                            }
                        }
                    }
                }

                newItem.HintPath = reference.Descendants()
                    .FirstOrDefault(x => x.Name.LocalName.Equals("HintPath", StringComparison.CurrentCultureIgnoreCase))
                    ?.Value;

                var value = reference.Descendants()
                    .FirstOrDefault(
                        x => x.Name.LocalName.Equals("SpecificVersion", StringComparison.CurrentCultureIgnoreCase))
                    ?.Value;
                newItem.SpecificVersion = !string.IsNullOrWhiteSpace(value)
                    ? bool.Parse(value)
                    : (bool?) null;

                output.Add(newItem);
            }

            return output;
        }

        internal static List<PackageReference> ParsePackageReferences(IEnumerable<XElement> references)
        {
            var output = new List<PackageReference>();

            foreach (var item in references)
            {
                var newItem = new PackageReference
                {
                    Name = item.Attribute("Include")?.Value
                };

                var versionAttribute = item.Attribute("Version");

                if (versionAttribute != null)
                {
                    newItem.Version = versionAttribute.Value;
                }

                if (item.HasElements)
                {
                    var versionElement = item.Elements()
                        .FirstOrDefault(
                            x => x.Name.LocalName.Equals("Version", StringComparison.CurrentCultureIgnoreCase));

                    if (versionElement != null)
                    {
                        newItem.Version = versionElement.Value;
                    }
                }

                output.Add(newItem);
            }

            return output;
        }

        internal static List<string> ParseProjectReferences(IEnumerable<XElement> projectReferences)
        {
            var temp = projectReferences
                .Select(reference => reference.Attribute("Include")?.Value);

            return temp.ToList();
        }

        private static List<KeyValuePair<string, string>> GetAssemblyInformation(XContainer xDocument)
        {
            var items = xDocument.Descendants()
                .Where(r => r.Name.LocalName.StartsWith("Assembly", StringComparison.CurrentCultureIgnoreCase));

            var output = ParseAssemblyInformationItems(items);

            return output;
        }
    }
}