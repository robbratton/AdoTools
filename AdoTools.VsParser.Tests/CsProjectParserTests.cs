using System;
using System.Linq;
using NUnit.Framework;
using AdoTools.VsParser.Entities.Project;
using AdoTools.VsParser.Entities.Project.Components;

namespace AdoTools.VsParser.Tests
{
    [TestFixture]
    public static class CsProjectParserTests
    {
        [TestCase(null, "path")]
        [TestCase("", "path")]
        [TestCase("  ", "path")]
        [TestCase("Content", null)]
        [TestCase("Content", "")]
        [TestCase("Content", "  ")]
        public static void ParseProjectFileContent_Throws(string content, string path)
        {
            Assert.Throws<ArgumentException>(() => CsProjectParser.ParseProjectFileContent(content, path));
        }

        private static void AssertSolutionInformationMissing(Project result)
        {
            // Information that comes from the solution will
            // be missing.
            Assert.Multiple(
                () =>
                {
                    Assert.That(result.NameInSolution, Is.Null);
                    Assert.That(result.PathRelativeToSolution, Is.Null);
                    Assert.That(result.TypeIds, Is.Empty);
                    Assert.That(result.Id, Is.Null);
                });
        }

        [Test]
        public static void ParseProjectFileContent_ReturnsExpectedResult_New()
        {
            // Arrange 

            const string filename = "new.csproj.txt";

            var content = Helpers.ReadEmbeddedResourceFile(filename);

            // Act
            var result = CsProjectParser.ParseProjectFileContent(content, System.IO.Path.GetFileNameWithoutExtension(filename));

            // Assert
            Assert.That(result, Is.Not.Null);

            Assert.Multiple(
                () =>
                {
                    Assert.That(
                        result.AssemblyInformation.FirstOrDefault(
                                x => x.Key.Equals("AssemblyName", StringComparison.CurrentCultureIgnoreCase))
                            .Value,
                        Is.Null);
                    Assert.That(result.HasMessages, Is.False);
                    Assert.That(result.ProjectFileFormat, Is.EqualTo(ProjectFileFormat.Vs2017OrLater));
                    Assert.That(result.ProjectOutputType, Is.EqualTo(ProjectOutputType.Library));
                    Assert.That(result.Sdk, Is.EqualTo("Microsoft.NET.Sdk"));

                    Helpers.AssertHasCountOf(result.AssemblyReferences, 6);
                    Helpers.AssertHasCountOf(result.Configurations, 2);
                    Helpers.AssertHasCountOf(result.Frameworks, 2);
                    Assert.That(result.Frameworks.FirstOrDefault(), Is.EqualTo("net461"));
                    // ReSharper disable once StringLiteralTypo
                    Assert.That(result.Frameworks.LastOrDefault(), Is.EqualTo("netcoreapp2.0"));
                    Helpers.AssertHasCountOf(result.PackageReferences, 8);
                    Helpers.AssertHasCountOf(result.ProjectReferences, 0);

                    AssertSolutionInformationMissing(result);
                });

            // todo Do more asserts here.
        }

        [Test]
        public static void ParseProjectFileContent_ReturnsExpectedResult_Old()
        {
            const string filename = "old.csproj.txt";

            var content = Helpers.ReadEmbeddedResourceFile(filename);

            var result = CsProjectParser.ParseProjectFileContent(content, System.IO.Path.GetFileNameWithoutExtension(filename));

            Assert.That(result, Is.Not.Null);

            Assert.Multiple(
                () =>
                {
                    Assert.That(
                        result.AssemblyInformation.FirstOrDefault(
                                x => x.Key.Equals("AssemblyName", StringComparison.CurrentCultureIgnoreCase))
                            .Value,
                        Is.EqualTo("UPMC.Web.WcfRestHttp"));
                    Assert.That(result.HasMessages, Is.False);
                    Assert.That(result.ProjectFileFormat, Is.EqualTo(ProjectFileFormat.Vs2015OrEarlier));
                    Assert.That(result.ProjectOutputType, Is.EqualTo(ProjectOutputType.Library));
                    Assert.That(result.Sdk, Is.Null);

                    Helpers.AssertHasCountOf(result.AssemblyReferences, 60);
                    Helpers.AssertHasCountOf(result.Configurations, 2);
                    Helpers.AssertHasCountOf(result.Frameworks, 1);
                    Assert.That(result.Frameworks.FirstOrDefault(), Is.EqualTo("net461"));
                    Helpers.AssertHasCountOf(result.PackageReferences, 0);
                    Helpers.AssertHasCountOf(result.ProjectReferences, 8);

                    AssertSolutionInformationMissing(result);
                });

            // todo Do more asserts here.
        }
    }
}