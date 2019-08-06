using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using NLog;
using NUnit.Framework;
using AdoTools.Common;
using AdoTools.Common.Extensions;
using AdoTools.Ado.Entities;
using AdoTools.Ado.SourceTools;

// ReSharper disable PossibleMultipleEnumeration

namespace AdoTools.Ado.Tests.SourceToolTests
{
    [TestFixture]
    public class GitSourceToolTests
    {
        [SetUp]
        public static void SetUp()
        {
            Logger = LogManager.GetCurrentClassLogger();
        }

        // ReSharper disable StringLiteralTypo
        [TestCase(
            null,
            null,
            null,
            TestData.VsTsApiUriStringFake
            + "/"
            + TestData.ProjectFake
            + "/_apis/git/repositories?api-version="
            + GitSourceTool.GitSourceToolApiVersion)]
        [TestCase(
            "",
            "",
            "",
            TestData.VsTsApiUriStringFake
            + "/"
            + TestData.ProjectFake
            + "/_apis/git/repositories?api-version="
            + GitSourceTool.GitSourceToolApiVersion)]
        [TestCase(
            "    ",
            "  ",
            "     ",
            TestData.VsTsApiUriStringFake
            + "/"
            + TestData.ProjectFake
            + "/_apis/git/repositories?api-version="
            + GitSourceTool.GitSourceToolApiVersion)]
        [TestCase(
            null,
            "Suffix1",
            null,
            TestData.VsTsApiUriStringFake
            + "/"
            + TestData.ProjectFake
            + "/_apis/git/repositories/Suffix1?api-version="
            + GitSourceTool.GitSourceToolApiVersion)]
        [TestCase(
            null,
            "Suffix1",
            "",
            TestData.VsTsApiUriStringFake
            + "/"
            + TestData.ProjectFake
            + "/_apis/git/repositories/Suffix1?api-version="
            + GitSourceTool.GitSourceToolApiVersion)]
        [TestCase(
            null,
            "Suffix1",
            "   ",
            TestData.VsTsApiUriStringFake
            + "/"
            + TestData.ProjectFake
            + "/_apis/git/repositories/Suffix1?api-version="
            + GitSourceTool.GitSourceToolApiVersion)]
        [TestCase(
            null,
            null,
            "query1=1",
            TestData.VsTsApiUriStringFake
            + "/"
            + TestData.ProjectFake
            + "/_apis/git/repositories?api-version=" + GitSourceTool.GitSourceToolApiVersion + "&query1=1")]
        [TestCase(
            null,
            "",
            "query1=1",
            TestData.VsTsApiUriStringFake
            + "/"
            + TestData.ProjectFake
            + "/_apis/git/repositories?api-version=" + GitSourceTool.GitSourceToolApiVersion + "&query1=1")]
        [TestCase(
            null,
            "   ",
            "query1=1",
            TestData.VsTsApiUriStringFake
            + "/"
            + TestData.ProjectFake
            + "/_apis/git/repositories?api-version=" + GitSourceTool.GitSourceToolApiVersion + "&query1=1")]
        [TestCase(
            "repo1",
            null,
            null,
            TestData.VsTsApiUriStringFake
            + "/"
            + TestData.ProjectFake
            + "/_apis/git/repositories/repo1?api-version="
            + GitSourceTool.GitSourceToolApiVersion)]
        [TestCase(
            "repo1",
            "",
            null,
            TestData.VsTsApiUriStringFake
            + "/"
            + TestData.ProjectFake
            + "/_apis/git/repositories/repo1?api-version="
            + GitSourceTool.GitSourceToolApiVersion)]
        [TestCase(
            "repo1",
            "   ",
            null,
            TestData.VsTsApiUriStringFake
            + "/"
            + TestData.ProjectFake
            + "/_apis/git/repositories/repo1?api-version="
            + GitSourceTool.GitSourceToolApiVersion)]
        [TestCase(
            "repo1",
            "Suffix1",
            "query1=1",
            TestData.VsTsApiUriStringFake
            + "/"
            + TestData.ProjectFake
            + "/_apis/git/repositories/repo1/Suffix1?api-version="
            + GitSourceTool.GitSourceToolApiVersion + "&query1=1")]
        // ReSharper restore StringLiteralTypo
        public static void MakeVsTsRepoUri_ReturnsExpectedOutput(
            string repositoryName,
            string pathSuffix,
            string querySuffix,
            string expectedResult)
        {
            var sourceTool = Helpers.SetUpFakeGitSourceTool();

            var result = sourceTool.MakeRepoVsTsUri(repositoryName, pathSuffix, querySuffix);

            StringAssert.AreEqualIgnoringCase(
                expectedResult,
                result
            );
        }

        // WARNING: This test needs to match the tags in the source repository.
        [Category("Integration")]
        [TestCase("Microservice-ConfigurationFiles", false)]
        [TestCase("Upmc.Services.Clients.Common", true)]
        public static void GetTags_Succeeds(string repo, bool expectTagsToExist)
        {
            // Arrange
            var sourceTool = Helpers.SetUpRealGitSourceTool();

            // Act
            var result = sourceTool.GetTags(repo);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any(), Is.EqualTo(expectTagsToExist));

            Console.WriteLine(result.DumpValues(0));
        }


        [Test]
        public static void Constructor_Succeeds([Values] bool includeLogger)
        {
            var vsTsTool = Helpers.SetUpFakeVsTsTool();

            if (includeLogger)
            {
                Assert.That(() => new GitSourceTool(vsTsTool, Logger), Throws.Nothing);
            }
            else
            {
                Assert.That(() => new GitSourceTool(vsTsTool), Throws.Nothing);
            }
        }

        [Test]
        public static void Constructor_Throws([Values] bool includeLogger)
        {
            if (includeLogger)
            {
                Assert.That(() => new GitSourceTool(null, Logger), Throws.ArgumentException);
            }
            else
            {
                Assert.That(() => new GitSourceTool(null), Throws.ArgumentException);
            }
        }

        [Category("Integration")]
        [Test]
        public static void GetBranches_Parallel_Succeeds()
        {
            // Arrange
            var sourceTool = Helpers.SetUpRealGitSourceTool();

            var items = new ushort[10];

            Parallel.ForEach(items, (item, state, index) =>
                {
                    var sw = Stopwatch.StartNew();

                    // Act
                    var result = sourceTool.GetBranches("Upmc.DevTools.Common");

                    // Assert
                    Assert.That(result, Is.Not.Null);
                    Assert.That(result.Count(), Is.GreaterThan(0));

                    Console.WriteLine($"Index {index}, Elapsed {sw.Elapsed}");

                    Console.WriteLine(result.DumpValues(0));
                }
            );
        }

        [Category("Integration")]
        [Test]
        public static void GetCommits_Succeeds_Nulls()
        {
            var sourceTool = Helpers.SetUpRealGitSourceTool();

            var result = sourceTool.GetCommits("Upmc.DevTools.Common", "develop");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.GreaterThan(0));
            Console.WriteLine($"Found {result.Count()} commits.");
        }

        [Category("Integration")]
        [Test]
        public static void GetCommits_Succeeds_One()
        {
            var sourceTool = Helpers.SetUpRealGitSourceTool();

            var result = sourceTool.GetCommits("Upmc.DevTools.Common", "develop", 0, 1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Console.WriteLine($"Found {result.Count()} commits.");

            Console.WriteLine(result.DumpValues(0));
        }

        [Category("Integration")]
        [Test]
        public static void GetCommits_Throws_WithBadBranch()
        {
            var sourceTool = Helpers.SetUpRealGitSourceTool();

            Assert.That(
                // ReSharper disable once StringLiteralTypo
                () => sourceTool.GetCommits("AdoTools.Ado", "SnaggleTooth"),
                Throws.Exception.TypeOf<VssServiceException>());
        }

        [Category("Integration")]
        [Test]
        public static void GetCommits_Throws_WithBadRepo()
        {
            var sourceTool = Helpers.SetUpRealGitSourceTool();

            Assert.That(
                // ReSharper disable once StringLiteralTypo
                () => sourceTool.GetCommits("SnaggleTooth", "develop"),
                Throws.Exception.TypeOf<VssServiceException>());
        }

        [Category("Integration")]
        [Test]
        public static void GetItem_Parallel_Succeeds()
        {
            // Arrange
            var sourceTool = Helpers.SetUpRealGitSourceTool();

            var sourceInformation = new SourceInformation(
                SourceType.TfsGit,
                "Upmc.DevTools.Common.sln",
                false,
                "Upmc.DevTools.Common",
                "develop");

            var items = new ushort[10];

            Parallel.ForEach(items, (item, state, index) =>
            {
                var sw = Stopwatch.StartNew();

                // Act
                var result = sourceTool.GetItem(sourceInformation);

                // Assert
                Assert.That(result, Is.Not.Null);

                Console.WriteLine($"Index {index}, Elapsed {sw.Elapsed}");

                var myHandlers = new VariableDumpHandlers();
                Console.WriteLine(result.DumpValues(0, handlers: myHandlers));
            });
        }

        [Category("Integration")]
        [Test]
        public static void GetItem_ReturnsNullWhenNotFound()
        {
            // Arrange
            var sourceTool = Helpers.SetUpRealGitSourceTool();

            var sourceInformation = new SourceInformation(
                SourceType.TfsGit,
                "/A/B/C/xxx.txt",
                false,
                "AdoTools.Common",
                "develop");

            // Act
            var result = sourceTool.GetItem(sourceInformation);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Category("Integration")]
        [Test]
        public static void GetItemContent_Parallel_Succeeds()
        {
            var items = new ushort[10];

            Parallel.ForEach(items, (item, state, index) =>
            {
                var sw = Stopwatch.StartNew();

                // Arrange
                var sourceTool = Helpers.SetUpRealGitSourceTool();

                var sourceInformation = new SourceInformation(
                    SourceType.TfsGit,
                    "Upmc.DevTools.Common.sln",
                    false,
                    "Upmc.DevTools.Common",
                    "develop");

                // Act
                var result = sourceTool.GetItemContent(sourceInformation);

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Length, Is.GreaterThan(0));

                Console.WriteLine($"Index {index}, Elapsed {sw.Elapsed}");
            });
        }

        [Category("Integration")]
        [Test]
        public static void GetItemContent_ReturnsNullWhenNotFound()
        {
            // Arrange
            var sourceTool = Helpers.SetUpRealGitSourceTool();

            var sourceInformation = new SourceInformation(
                SourceType.TfsGit,
                "/A/B/C/xxx.txt",
                false,
                "AdoTools.Common",
                "develop");

            // Act
            var result = sourceTool.GetItemContent(sourceInformation);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Category("Integration")]
        [Test]
        public static void GetItems_Succeeds()
        {
            // Arrange
            var sourceTool = Helpers.SetUpRealGitSourceTool();

            var sourceInformation = new SourceInformation(
                SourceType.TfsGit,
                "/",
                true,
                "Upmc.DevTools.Common",
                "develop");

            // Act
            var result = sourceTool.GetItems(sourceInformation);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.GreaterThan(2));

            Console.WriteLine(result.DumpValues(0));
        }

        [Category("Integration")]
        [Test]
        public static void GetItems_Throws_WhenNotFound()
        {
            // Arrange
            var sourceTool = Helpers.SetUpRealGitSourceTool();

            var sourceInformation = new SourceInformation(
                SourceType.TfsGit,
                @"\A\B\C",
                true,
                "AdoTools.Common",
                "Develop");

            // Act
            Assert.That(
                () => sourceTool.GetItems(sourceInformation),
                Throws.TypeOf<VssServiceException>());
        }

        [Category("Integration")]
        [Test]
        public void GetRepositories_Succeeds([Values] bool ignoreProvided)
        {
            // Arrange
            var sourceTool = Helpers.SetUpRealGitSourceTool();
            List<string> repositoriesToIgnore = null;

            if (ignoreProvided)
            {
                repositoriesToIgnore = new List<string> {"ToBeDeleted"};
            }

            // Act
            var result = sourceTool.GetRepositories(repositoriesToIgnore);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.GreaterThan(0));

            Console.WriteLine(result.DumpValues(0));
        }

        /// <summary>
        ///     Review log for cache information. There should be cache hits if the same repo is looked up multiple times.
        /// </summary>
        [Category("Integration")]
        [Test]
        public static void GetRepositoryId_Parallel_Succeeds()
        {
            // Arrange
            var sourceTool = Helpers.SetUpRealGitSourceTool();

            var repos = new[]
            {
                "Upmc.DevTools.Common",
                "Upmc.DevTools.Dependency.Parser",
                "Upmc.DevTools.VsParser",
                "Upmc.DevTools.Vsts"
            };

            var random = new Random(DateTime.Now.Millisecond);

            var items = new ushort[100];

            Parallel.ForEach(items, (item, state, index) =>
                {
                    var sw = Stopwatch.StartNew();

                    // Act
                    var repositoryName = repos[random.Next(3)];
                    var result = sourceTool.GetRepositoryId(repositoryName);

                    // Assert
                    Assert.That(result, Is.Not.Null);
                    Assert.That(result, Is.Not.EqualTo(Guid.Empty));

                    Console.WriteLine($"Repo {repositoryName}, Index {index}, Elapsed {sw.Elapsed}");
                }
            );
        }

        [Test]
        public void Map([Values] bool isFolder)
        {
            var sourceTool = Helpers.SetUpFakeGitSourceTool();

            var input = new GitItem("path", "objectId", GitObjectType.Blob, "commitId", 4) {IsFolder = isFolder};

            var result = sourceTool.Map(input, "repository", "branch");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.SourcePath, Is.EqualTo(input.Path));
            Assert.That(result.IsDirectory, Is.EqualTo(input.IsFolder));
        }

        [Test]
        public void UpdateSourceInformation_Succeeds([Values(true, false)] bool isDirectory)
        {
            var sourceTool = Helpers.SetUpFakeGitSourceTool();

            var sourceInfo = new SourceInformation(
                SourceType.TfsGit,
                "sourcePath",
                isDirectory,
                "gitRepo",
                "getBranch");
            var item = new GitItem
            {
                Path = "itemPath"
            };

            sourceTool.UpdateSourceInformation(sourceInfo, item);

            Assert.That(sourceInfo.SourcePath, Is.EqualTo(item.Path));
        }


        private static Logger Logger { get; set; }
    }
}