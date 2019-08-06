using System;
using NUnit.Framework;
using Upmc.DevTools.Dependency.Parser.Entities;
using Upmc.DevTools.VsTs.Entities;

namespace Upmc.DevTools.Dependency.Parser.Tests
{
    [TestFixture]
    public class ToolsTests : TestBase
    {
        [TestCase(@"c:\wrong\end", @"\", true, @"c:\wrong\end\")]
        // ReSharper disable StringLiteralTypo
        [TestCase(@"c:\right\end\", @"\", true, @"c:\right\end\")]
        // ReSharper restore StringLiteralTypo
        [TestCase(@"/wrong/end", @"/", true, @"/wrong/end/")]
        [TestCase(@"/right/end/", @"/", true, @"/right/end/")]
        [TestCase(@"c:\file.txt", @"\", false, @"c:\file.txt")]
        [TestCase(@"/file.txt", @"/", false, @"/file.txt")]
        public static void FixPathEnding_ReturnsExpectedResult(
            string inputPath,
            string pathSeparator,
            bool isDirectory,
            string expectedOutputPath)
        {
            var result = Tools.FixPathEnding(inputPath, pathSeparator, isDirectory);

            Assert.That(result, Is.EqualTo(expectedOutputPath));
        }

        [TestCase(@"c:/wrong/sep", @"\", @"c:\wrong\sep")]
        // ReSharper disable StringLiteralTypo
        [TestCase(@"c:\right\sep", @"\", @"c:\right\sep")]
        // ReSharper restore StringLiteralTypo
        [TestCase(@"\wrong\sep", @"/", @"/wrong/sep")]
        [TestCase(@"/right/sep", @"/", @"/right/sep")]
        public static void FixPathSeparators_ReturnsExpectedResult(string input, string pathSeparator, string expectedOutput)
        {
            var result = Tools.FixPathSeparators(input, pathSeparator);

            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        [TestCase(SourceType.Filesystem, @"c:\dir1\file.txt", false, "childDir", @"c:\dir1\childDir\")]
        [TestCase(SourceType.Filesystem, @"c:\dir1\", true, "childDir", @"c:\dir1\childDir\")]
        [TestCase(SourceType.TfsGit, @"/dir1/file.txt", false, "childDir", @"/dir1/childDir/")]
        [TestCase(SourceType.TfsGit, @"/dir1/", true, "childDir", @"/dir1/childDir/")]
        [TestCase(SourceType.TfsVc, @"/dir1/file.txt", false, "childDir", @"/dir1/childDir/")]
        [TestCase(SourceType.TfsVc, @"/dir1/", true, "childDir", @"/dir1/childDir/")]
        public static void GetChildDirectory_ReturnsExpectedResult(
            SourceType sourceType,
            string inputPath,
            bool isDirectory,
            string childDirectory,
            string expectedPath)
        {
            var inputInfo = sourceType == SourceType.TfsGit
                ? new SourceInformation(sourceType, inputPath, isDirectory, "repository", "branch")
                : new SourceInformation(sourceType, inputPath, isDirectory);

            var result = Tools.GetChildDirectoryInformation(inputInfo, childDirectory);

            Assert.Multiple(
                () =>
                {
                    // Main test
                    Assert.That(result.SourcePath, Is.EqualTo(expectedPath));

                    Assert.That(result.GitBranch, Is.EqualTo(inputInfo.GitBranch));
                    Assert.That(result.GitRepositoryName, Is.EqualTo(inputInfo.GitRepositoryName));
                    Assert.That(result.IsValid, Is.EqualTo(inputInfo.IsValid));
                    Assert.That(result.SourceType, Is.EqualTo(inputInfo.SourceType));
                    Assert.That(result.IsDirectory, Is.EqualTo(true));
                });
        }

        [TestCase(SourceType.Filesystem, @"c:\dir1\", true, "child.txt", @"c:\dir1\child.txt")]
        [TestCase(SourceType.Filesystem, @"c:\dir1\x.txt", false, "child.txt", @"c:\dir1\child.txt")]
        [TestCase(SourceType.TfsGit, @"/dir1/", true, "child.txt", @"/dir1/child.txt")]
        [TestCase(SourceType.TfsGit, @"/dir1/x.txt", false, "child.txt", @"/dir1/child.txt")]
        [TestCase(SourceType.TfsVc, @"/dir1/", true, "child.txt", @"/dir1/child.txt")]
        [TestCase(SourceType.TfsVc, @"/dir1/x.txt", false, "child.txt", @"/dir1/child.txt")]
        public static void GetChildFile_ReturnsExpectedResult(
            SourceType sourceType,
            string inputPath,
            bool isDirectory,
            string filename,
            string expectedPath)
        {
            var inputInfo = sourceType == SourceType.TfsGit
                ? new SourceInformation(sourceType, inputPath, isDirectory, "repository", "branch")
                : new SourceInformation(sourceType, inputPath, isDirectory);

            var result = Tools.GetChildFileInformation(inputInfo, filename);

            Assert.Multiple(
                () =>
                {
                    // Main test
                    Assert.That(result.SourcePath, Is.EqualTo(expectedPath));

                    Assert.That(result.GitBranch, Is.EqualTo(inputInfo.GitBranch));
                    Assert.That(result.GitRepositoryName, Is.EqualTo(inputInfo.GitRepositoryName));
                    Assert.That(result.IsValid, Is.EqualTo(inputInfo.IsValid));
                    Assert.That(result.SourceType, Is.EqualTo(inputInfo.SourceType));
                    Assert.That(result.IsDirectory, Is.EqualTo(false));
                });
        }

        [TestCase(SourceType.Filesystem, @"c:\dir1\file.txt", @"c:\dir1\")]
        [TestCase(SourceType.TfsGit, @"/dir1/file.txt", @"/dir1/")]
        [TestCase(SourceType.TfsVc, @"/dir1/file.txt", @"/dir1/")]
        public static void GetDirectory_ReturnsExpectedResult(SourceType sourceType, string inputPath, string expectedPath)
        {
            var inputInfo = sourceType == SourceType.TfsGit
                ? new SourceInformation(sourceType, inputPath, false, "repository", "branch")
                : new SourceInformation(sourceType, inputPath, false);

            var result = Tools.GetDirectoryInformation(inputInfo);

            Assert.Multiple(
                () =>
                {
                    // Main test
                    Assert.That(result.SourcePath, Is.EqualTo(expectedPath));

                    Assert.That(result.GitBranch, Is.EqualTo(inputInfo.GitBranch));
                    Assert.That(result.GitRepositoryName, Is.EqualTo(inputInfo.GitRepositoryName));
                    Assert.That(result.IsValid, Is.EqualTo(inputInfo.IsValid));
                    Assert.That(result.SourceType, Is.EqualTo(inputInfo.SourceType));
                    Assert.That(result.IsDirectory, Is.EqualTo(true));
                });
        }

        [TestCase("   ", DeployEnvironment.None)]
        [TestCase("", DeployEnvironment.None)]
        [TestCase("app.debug.config", DeployEnvironment.None)]
        [TestCase("appsettings.dev.json", DeployEnvironment.Development)]
        [TestCase("dataConfiguration.development.config", DeployEnvironment.Development)]
        [TestCase("web.lab.config", DeployEnvironment.Lab)]
        [TestCase(null, DeployEnvironment.None)]
        public static void GetEnvironmentFromSourcePath_ReturnsExpectedResult(string path, DeployEnvironment expectedResult)
        {
            var result = Tools.GetEnvironmentFromSourcePath(path);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase(SourceType.Filesystem, @"c:\dir1\dir2\file.txt", false, @"c:\dir1\")]
        [TestCase(SourceType.Filesystem, @"c:\dir1\dir2\", true, @"c:\dir1\")]
        [TestCase(SourceType.TfsGit, @"/dir1/dir2/file.txt", false, @"/dir1/")]
        [TestCase(SourceType.TfsGit, @"/dir1/dir2/", true, @"/dir1/")]
        [TestCase(SourceType.TfsGit, @"/dir1/dir2/file.txt", false, @"/dir1/")]
        [TestCase(SourceType.TfsGit, @"/dir1/dir2/", true, @"/dir1/")]
        public static void GetParentDirectory_ReturnsExpectedResult(
            SourceType sourceType,
            string inputPath,
            bool isDirectory,
            string expectedPath)
        {
            var inputInfo = sourceType == SourceType.TfsGit
                ? new SourceInformation(sourceType, inputPath, isDirectory, "repository", "branch")
                : new SourceInformation(sourceType, inputPath, isDirectory);

            var result = Tools.GetParentDirectoryInformation(inputInfo);

            Assert.Multiple(
                () =>
                {
                    // Main test
                    Assert.That(result.SourcePath, Is.EqualTo(expectedPath));

                    Assert.That(result.GitBranch, Is.EqualTo(inputInfo.GitBranch));
                    Assert.That(result.GitRepositoryName, Is.EqualTo(inputInfo.GitRepositoryName));
                    Assert.That(result.IsValid, Is.EqualTo(inputInfo.IsValid));
                    Assert.That(result.SourceType, Is.EqualTo(inputInfo.SourceType));
                    Assert.That(result.IsDirectory, Is.EqualTo(true));
                });
        }

        [Test]
        public static void FixPathSeparators_ReturnsExpectedResult([Values("-", "_")] string pathSeparator)
        {
            Assert.That(() => Tools.FixPathSeparators("abc", pathSeparator), Throws.ArgumentException);
        }

        [Test]
        public static void GetChildDirectory_ThrowsArgumentException([Values(null, "", " ")] string childDirectory)
        {
            var inputInfo = new SourceInformation(SourceType.Filesystem, @"c:\git", true);

            Assert.That(() => Tools.GetChildDirectoryInformation(inputInfo, childDirectory), Throws.ArgumentException);
        }
    }
}