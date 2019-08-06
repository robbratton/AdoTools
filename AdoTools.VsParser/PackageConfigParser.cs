using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AdoTools.Common;
using AdoTools.VsParser.Entities.Project.Components;

// ReSharper disable UnusedMember.Global

namespace AdoTools.VsParser
{
    /// <summary>
    ///     Processes Packages.config content.
    /// </summary>
    public static class PackageConfigParser
    {
        /// <summary>
        ///     Gets NuGet references from the content of a
        ///     packages.config file.
        /// </summary>
        /// <param name="content">
        ///     Content from a packages.config file.
        /// </param>
        /// <returns><see cref="List{T}" /> of <see cref="PackageReference" /></returns>
        public static IEnumerable<PackageReference> ParsePackagesConfigFileContent(string content)
        {
            Validators.AssertIsNotNullOrWhitespace(content, nameof(content));

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(content);

            return (from XmlElement packageLine in xmlDocument.ChildNodes[1]
                select new PackageReference
                {
                    Name = packageLine.Attributes["id"].Value,
                    Version = packageLine.Attributes["version"].Value,
                    TargetFramework = packageLine.Attributes["targetFramework"]?.Value
                }).ToList();
        }
    }
}