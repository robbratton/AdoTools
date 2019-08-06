using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Moq;
using NLog;
using NUnit.Framework;
using AdoTools.Common.Entities;
using AdoTools.DependencyParser.Entities;
using AdoTools.Ado;
using AdoTools.Ado.Entities;
using AdoTools.Ado.SourceTools;

// ReSharper disable PossibleMultipleEnumeration

namespace AdoTools.DependencyParser.Tests
{
    [TestFixture]
    public class ParserGenericTests : TestBase
    {
        private static string WriteTestFiles()
        {
            var output = Path.GetTempPath() + Guid.NewGuid() + @"\project";
            Directory.CreateDirectory(output);

            foreach (var filename in new[]
            {
                "app.config",
                "packages.config",
                "web.config",
                "dataConfiguration.config",
                "packages.config"
            })
            {
                File.WriteAllText(output + @"\" + filename, Helpers.ReadEmbeddedResourceFile(filename));
            }

            return output;
        }

        private static void SetUpGitReal(
            out string personalAccessToken,
            out string organization,
            out string projectName)
        {
            personalAccessToken = AuthenticationHelper.GetPersonalAccessToken();
            organization = Organization;
            projectName = ProjectName;
        }

        private static void SetUpVcReal(
            out string personalAccessToken,
            out string organization,
            out string projectName)
        {
            personalAccessToken = AuthenticationHelper.GetPersonalAccessToken();
            organization = Organization;
            projectName = ProjectName;
        }

        private static void SetUpGitFake(
            out string personalAccessToken,
            out string organization,
            out string projectName)
        {
            personalAccessToken = "Junk PAT";
            organization = Organization;
            projectName = ProjectName;
        }

        private static void SetUpVcFake(
            out string personalAccessToken,
            out string organization,
            out string projectName)
        {
            personalAccessToken = "Junk PAT";
            organization = Organization;
            projectName = ProjectName;
        }

        private const string Organization = "upmcappsvcs";
        private const string ProjectName = "apollo";

        [Test]
        public void Constructor_Succeeds([Values] bool provideLogger, [Values(1, 2, 3)] int testCase,
            [Values(-1, 1)] int dop)
        {
            var performanceConfiguration =
                new PerformanceConfiguration {ParallelOptions = {MaxDegreeOfParallelism = dop}};

            Logger logger = null;

            if (provideLogger)
            {
                logger = _logger;
            }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (testCase)
            {
                case 1:
                {
                    SetUpGitFake(
                        out var personalAccessToken,
                        out var organization,
                        out var projectName);

                    var parserGeneric =
                        new ParserGeneric<GitItem>(
                            SourceType.TfsGit,
                            personalAccessToken,
                            organization,
                            projectName,
                            performanceConfiguration: performanceConfiguration,
                            logger: logger);
                    Assert.That(parserGeneric, Is.Not.Null);
                }

                    break;

                case 2:
                {
                    SetUpVcFake(out var serverUri, out var personalAccessToken, out var projectName);

                    var parserGeneric =
                        new ParserGeneric<TfvcItem>(
                            SourceType.TfsVc,
                            serverUri,
                            personalAccessToken,
                            projectName,
                            performanceConfiguration: performanceConfiguration,
                            logger: logger);
                    Assert.That(parserGeneric, Is.Not.Null);
                }

                    break;

                case 3:
                {
                    var parserGeneric =
                        new ParserGeneric<FileInfo>(
                            SourceType.Filesystem,
                            performanceConfiguration: performanceConfiguration,
                            logger: logger);
                    Assert.That(parserGeneric, Is.Not.Null);
                }

                    break;
            }
        }

        [Test]
        public static void ProcessConfigFiles_ReturnsExpectedResult(
            [Values("app.config", "web.config", "dataConfiguration.config", "appSettings.tok.json")]
            string resourceName)
        {
            var projectMetadata = new ProjectMetadata();
            var extension = Path.GetExtension(resourceName);
            var content = Helpers.ReadEmbeddedResourceFile(resourceName);

            ParserGeneric<FileInfo>.ProcessConfigFile(projectMetadata, resourceName, extension, content);

            switch (resourceName.ToLower())
            {
                case "app.config":
                    Assert.That(projectMetadata.AppSettings, Is.Not.Null);
                    Assert.That(projectMetadata.AppSettings.Count, Is.EqualTo(4));

                    break;
                case "web.config":
                    Assert.That(projectMetadata.AppSettings, Is.Not.Null);
                    Assert.That(projectMetadata.AppSettings.Count, Is.EqualTo(1));

                    break;
                case "dataconfiguration.config":
                    Assert.Multiple(() =>
                    {
                        Assert.That(projectMetadata.DatabaseInstances, Is.Not.Null);
                        Assert.That(projectMetadata.DatabaseInstances.Count, Is.EqualTo(10));

                        Assert.That(projectMetadata.DatabaseTypes, Is.Not.Null);
                        Assert.That(projectMetadata.DatabaseTypes.Count, Is.EqualTo(1));

                        Assert.That(projectMetadata.ConnectionStrings, Is.Not.Null);
                        Assert.That(projectMetadata.ConnectionStrings.Count, Is.EqualTo(11));
                    });

                    break;

                case "appsettings.tok.json":
                    Assert.Multiple(() =>
                    {
                        Assert.That(projectMetadata.DatabaseInstances, Is.Null);

                        Assert.That(projectMetadata.DatabaseTypes, Is.Null);

                        Assert.That(projectMetadata.ConnectionStrings, Is.Not.Null);
                        Assert.That(projectMetadata.ConnectionStrings.Count, Is.EqualTo(4));
                    });

                    break;
                default:
                    Assert.Fail("Test bug");

                    break;
            }

            // todo check contents of projectMetadata.
        }

        [Test]
        public void ProcessPackagesConfig_ReturnsExpectedResult([Values(-1, 1)] int dop)
        {
            var performanceConfiguration =
                new PerformanceConfiguration {ParallelOptions = {MaxDegreeOfParallelism = dop}};

            var directory = WriteTestFiles();
            var parser = new ParserGeneric<FileInfo>(
                SourceType.Filesystem,
                logger: _logger,
                performanceConfiguration: performanceConfiguration);

            var projectMetadata = new ProjectMetadata();
            var testSourceInformation = new SourceInformation(
                SourceType.Filesystem,
                directory + @"\SampleNew.csproj",
                false);
            parser.ProcessPackagesConfig(projectMetadata, testSourceInformation);

            Assert.That(projectMetadata.PackageReferences, Is.Not.Null);
            Assert.That(projectMetadata.PackageReferences.Count, Is.EqualTo(1));
        }

        [Test]
        [Category("Integration")]
        public void ProcessSource_Filesystem_ReturnsExpectedResult([Values(-1, 1)] int dop)
        {
            var performanceConfiguration =
                new PerformanceConfiguration {ParallelOptions = {MaxDegreeOfParallelism = dop}};

            var parser = new ParserGeneric<FileInfo>(
                SourceType.Filesystem,
                logger: _logger,
                performanceConfiguration: performanceConfiguration);

            var searchInformation =
                new SourceInformation(SourceType.Filesystem, @"c:\gh\AzureDevOpsLibrary", true);

            var result = parser.ProcessSource(searchInformation, (IEnumerable<string>) null);

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(
                () =>
                {
                    Assert.That(result.Count(), Is.GreaterThanOrEqualTo(1));
                    Assert.That(result.First().Projects, Is.Not.Null);
                    Assert.That(result.First().Projects.Count, Is.GreaterThanOrEqualTo(2));
                });
        }

        [Test]
        [Category("Integration")]
        [Timeout(300000)]
        public void ProcessSource_Git_ReturnsExpectedResult([Values(-1, 1)] int dop)
        {
            var performanceConfiguration =
                new PerformanceConfiguration {ParallelOptions = {MaxDegreeOfParallelism = dop}};

            SetUpGitReal(out var serverUri, out var personalAccessToken, out var projectName);

            var parser = new ParserGeneric<GitItem>(
                SourceType.TfsGit,
                serverUri,
                personalAccessToken,
                projectName,
                logger: _logger,
                performanceConfiguration: performanceConfiguration);
            var searchInformation =
                new SourceInformation(SourceType.TfsGit, "/", true, "Upmc.DevTools.Common", "develop");

            var result = parser.ProcessSource(searchInformation, (IEnumerable<string>) null);

            Assert.That(result, Is.Not.Null);

            Assert.Multiple(
                () =>
                {
                    Assert.That(result.Count(), Is.GreaterThanOrEqualTo(1), "result.Count");

                    Assert.That(result.First().ProjectMetadatas, Is.Not.Null, "projectMetadatas");
                    Assert.That(result.First().ProjectMetadatas.Count, Is.GreaterThanOrEqualTo(2),
                        "projectMetadatas count");

                    Assert.That(result.Count(sln => sln.ProjectMetadatas.Any(x => x.SourceInformation != null)),
                        Is.EqualTo(1), "projects with source information");
                });
        }

        [Test]
        [Category("Integration")]
        public void ProcessSource_TfVc_ReturnsExpectedResult([Values(-1, 1)] int dop)
        {
            var performanceConfiguration = new PerformanceConfiguration
                {ParallelOptions = {MaxDegreeOfParallelism = dop}};

            SetUpVcReal(out var serverUri, out var personalAccessToken,
                out var projectName);

            var parser = new ParserGeneric<TfvcItem>(
                SourceType.TfsVc,
                serverUri,
                personalAccessToken,
                projectName,
                logger: _logger,
                performanceConfiguration: performanceConfiguration);

            var searchInformation =
                new SourceInformation(SourceType.TfsVc, "$/Apollo/HP/Source/Tools/BranchChecker", true);

            var result = parser.ProcessSource(searchInformation, new[] {"/_"});

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(
                () =>
                {
                    Assert.That(result.Count(), Is.GreaterThanOrEqualTo(1), "result.Count");
                    Assert.That(result.First().ProjectMetadatas, Is.Not.Null, "projectMetadatas");
                    Assert.That(result.First().ProjectMetadatas.Count, Is.GreaterThanOrEqualTo(2),
                        "projectMetadatas count");
                    Assert.That(result.Count(sln => sln.ProjectMetadatas.Any(x => x.SourceInformation != null)),
                        Is.EqualTo(1), "projectMetadatas with source information");
                });
        }

        [Test]
        public static void TestConstructor_ShouldNotThrow()
        {
            Assert.Multiple(
                () =>
                {
                    Assert.That(() => new ParserGeneric<GitItem>(), Throws.Nothing);
                    Assert.That(() => new ParserGeneric<TfvcItem>(), Throws.Nothing);
                    Assert.That(() => new ParserGeneric<FileInfo>(), Throws.Nothing);
                });
        }

        [Test]
        [Ignore("This doesn't work yet")]
        public static void ExceptionThrown()
        {
            var sourceToolMock = new Mock<ISourceTool<GitItem>>();
            sourceToolMock.Setup(x => x.GetItems(It.IsAny<SourceInformation>()))
                .Returns(new List<GitItem> {
                    new GitItem(
                        "/", 
                        "objectId", 
                        GitObjectType.Commit, 
                        "commitID", 
                        1)});
            //sourceToolMock.Setup(x => x.GetItems(It.IsAny<SourceInformation>()))
            //    .Throws(new InvalidOperationException("Testing2"));
            sourceToolMock.Setup(x => x.GetItemContent(It.IsAny<SourceInformation>()))
                .Throws(new InvalidOperationException("Testing1"));

            var wasCalled = false;
            var parser = new ParserGeneric<GitItem>(sourceToolMock.Object);
            parser.ExceptionThrown += (sender, args) => { wasCalled = true; };

            var sourceInformation = new SourceInformation(
                SourceType.TfsGit, 
                "/", 
                true, 
                "AdoTools.DependencyParser", 
                "develop");

            parser.ProcessSource(sourceInformation, Array.Empty<string>());

            Assert.That(wasCalled, Is.True);
        }
    }
}