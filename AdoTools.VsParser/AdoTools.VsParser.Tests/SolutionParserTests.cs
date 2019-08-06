using System;
using System.Linq;
using NUnit.Framework;
using Upmc.DevTools.VsParser.Entities;

namespace Upmc.DevTools.VsParser.Tests
{
    [TestFixture]
    public class SolutionParserTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public static void ParseSolutionFileContent_Throws(string content)
        {
            var parser = new SolutionParser();

            Assert.Throws<ArgumentException>(() => parser.ParseSolutionFileContent(content));
        }

        [Test]
        public void ParseSolutionFileContent_ReturnsExpectedResult()
        {
            // Arrange
            var parser = new SolutionParser();

            const string filename = "sample.sln.txt";

            var content = Helpers.ReadEmbeddedResourceFile(filename);

            // Act
            var result = parser.ParseSolutionFileContent(content);

            result.GitVersion = new GitVersion {NextVersion = "1.2.3"};

            // Assert
            Assert.That(result, Is.Not.Null, "result");

            Assert.Multiple(() =>
            {
                Assert.That(result.Projects, Is.Not.Null, "Projects");
                Assert.That(result.Projects.Count, Is.EqualTo(2), "Projects.Count");

                var project1 = result.Projects[0];
                Assert.That(
                    project1.TypeIds.First(),
                    Is.EqualTo(new Guid("9A19103F-16F7-4668-BE54-9A1E7A4F7556")),
                    "project1.TypeIds.First()");
                Assert.That(project1.Id, Is.EqualTo(new Guid("A7BACD69-F323-4416-B5D7-B85D7F5CA44C")), "project1.Id");
                Assert.That(project1.NameInSolution, Is.EqualTo("Upmc.DevTools.CsProject"), "project1.NameInSolution");
                Assert.That(
                    project1.PathRelativeToSolution,
                    Is.EqualTo(@"Upmc.DevTools.CsProject\Upmc.DevTools.CsProject.csproj"),
                    "project1.PathRelativeToSolution");

                var project2 = result.Projects[1];
                Assert.That(
                    project2.TypeIds.First(),
                    Is.EqualTo(new Guid("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC")),
                    "project2.TypeIds.First()");
                Assert.That(project2.Id, Is.EqualTo(new Guid("FED7C5D5-46D0-4C69-965C-ECEDCAFD4B3B")), "project2.Id");
                Assert.That(
                    project2.NameInSolution,
                    Is.EqualTo("Upmc.DevTools.CsProject.Tests"),
                    "project2.NameInSolution");
                Assert.That(
                    project2.PathRelativeToSolution,
                    Is.EqualTo(@"Upmc.DevTools.CsProject.Tests\Upmc.DevTools.CsProject.Tests.csproj"),
                    project2.PathRelativeToSolution);

                Assert.That(result.FormatVersion, Is.EqualTo("12.00"), "FormatVersion");

                Assert.That(
                    result.MinimumVisualStudioVersion,
                    Is.EqualTo("10.0.40219.1"),
                    "MinimumVisualStudioVersion");

                Assert.That(result.VisualStudioVersion, Is.EqualTo("15.0.27004.2009"), "VisualStudioVersion");

                Assert.That(result.GitVersion.NextVersion, Is.EqualTo("1.2.3"), "result.GitVersion.NextVersion");
            });
        }
    }
}